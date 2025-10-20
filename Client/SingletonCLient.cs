using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using DnsClient;
using Microsoft.IdentityModel.Tokens;
using MobileWallet.Desktop.API;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace MobileWallet.Desktop.Client;

public class TokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("id_token")]
    public string IdToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
}

public static class TokenManager
{
    public static string AccessToken { get; set; } = "";
    public static string RefreshToken { get; set; } = "";
    public static DateTime Expiry { get; set; } = DateTime.Now;
    public static string IdToken { get; set; } = "";
    public static string UserId { get; set; } = "";
    public static string UserName { get; set; } = "";
    public static string SessionId { get; set; } = "";
}

public static class HttpClientSingleton
{
    private static HttpClient? _httpClient;
    private static readonly object LOCK = new object();
    public static HttpClient Instance
    {
        get
        {
            if (_httpClient == null)
            {
                lock (LOCK)
                {
                    if (_httpClient == null)
                    {
                        _httpClient = new HttpClient(new AuthenticatedHttpClientHandler());
                        _httpClient.Timeout = TimeSpan.FromSeconds(120);
                        _httpClient.BaseAddress = new Uri(Global.BaseUrl);
                    }
                }
            }
            return _httpClient;
        }
    }
}

public class AppAuthClient
{
    public async Task LookUpBaseUrl()
    {
        try
        {
            var lookup = new LookupClient(IPAddress.Parse("1.1.1.1"));
            var url = Global.BaseUrl.Replace("https:", "").Replace("/", "");
            var result = await lookup.QueryAsync(url, QueryType.A);
            var ip = result.Answers.ARecords().FirstOrDefault()?.Address;
            App.AppLogger.Debug("IP Found :" + ip);
        }
        catch (Exception e)
        {
            App.AppLogger.Error(e, e.Message);
        }
    }

    public async Task<bool> Authenticate(string userName, string password, string otp = "")
    {
        try
        {
            App.AppLogger.Debug(DateTime.Now + $"launched authentications with user name : {userName} and password {password} ");

            if (string.IsNullOrEmpty(otp) && Global.UseOtp)
            {
                var client = new SmsClient(HttpClientSingleton.Instance);
                var result = await client.Sms_SendVerificationSmsForLoginAsync(
                    new SendVerificationSmsForLoginRequestModel()
                    {
                        Password = password,
                        Role = "ATM",
                        Username = userName,
                    }
                );
                if (result != null)
                {
                    return true;
                }
                else
                {
                    App.AppLogger.Error("SMS verification failed");
                    return false;
                }
            }
            var request = new HttpRequestMessage(HttpMethod.Post, Global.BaseUrl + "connect/token");
            var collection = new List<KeyValuePair<string, string>>
            {
                new("username", userName),
                new("grant_type", "password"),
                new("otp", otp),
                new("password", password),
                new("scope", "openid email profile offline_access roles"),
                new("role", "ATM"),
            };
            if (!Global.UseOtp)
            {
                collection.RemoveAt(2);
            }
            if (!Global.IsTest && otp == "000000") return false;
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await HttpClientSingleton.Instance.SendAsync(request);
            var resultJson = (await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                var tokeObj = JsonConvert.DeserializeObject<TokenResponse>(resultJson);
                if (tokeObj == null)
                {
                    App.AppLogger.Error("Server didn't respond with a jwt. Token is null");
                    return false;
                }
                TokenManager.Expiry = DateTime.Now.AddSeconds(tokeObj.ExpiresIn - 120);
                TokenManager.AccessToken = tokeObj.AccessToken;
                TokenManager.IdToken = tokeObj.IdToken;
                TokenManager.RefreshToken = tokeObj.RefreshToken;
                TokenManager.UserId =
                    JwtDecoder
                        .DecodeJwt(TokenManager.IdToken)
                        .FirstOrDefault(p => p.Type == "userId")
                        ?.Value ?? "";
                TokenManager.UserName =
                    JwtDecoder
                        .DecodeJwt(TokenManager.IdToken)
                        .FirstOrDefault(p => p.Type == "sub")
                        ?.Value ?? "";
                return true;
            }
            else
            {
                App.AppLogger.Error($"Server responded negatively to an authentication request. status code {response.StatusCode}");
                return false;
            }
        }
        catch (Exception e)
        {
            App.AppLogger.Error(e, e.Message);
            return false;
        }
    }

    public async Task<bool> RefreshToken(string refreshToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Global.BaseUrl + "connect/token");
            var collection = new List<KeyValuePair<string, string>>
            {
                new("scope", "openid email profile offline_access roles"),
                new("role", "ATM"),
                new("grant_type", "refresh_token"),
                new("refresh_token", refreshToken),
            };
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await HttpClientSingleton.Instance.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var resultJson = (await response.Content.ReadAsStringAsync());
                var tokeObj = JsonConvert.DeserializeObject<TokenResponse>(resultJson);
                if (tokeObj == null || string.IsNullOrEmpty(tokeObj.AccessToken))
                {
                    return false;
                }
                TokenManager.Expiry = DateTime.Now.AddSeconds(tokeObj.ExpiresIn - 120);
                TokenManager.AccessToken = tokeObj.AccessToken;
                TokenManager.RefreshToken = tokeObj.RefreshToken;
                TokenManager.IdToken = tokeObj.IdToken;
                TokenManager.UserId =
                    JwtDecoder
                        .DecodeJwt(TokenManager.IdToken)
                        .FirstOrDefault(p => p.Type == "userId")
                        ?.Value ?? "";
                TokenManager.UserName =
                    JwtDecoder
                        .DecodeJwt(TokenManager.IdToken)
                        .FirstOrDefault(p => p.Type == "sub")
                        ?.Value ?? "";
                return true;
            }
            else
            {
                var timer = DateTime.Now;
                App.AppLogger.Error($"{timer} : Request for new access token failed");
                return false;
            }
        }
        catch (Exception e)
        {
            App.AppLogger.Error(e, e.Message);
            return false;
        }
    }
}

public class AuthenticatedHttpClientHandler : DelegatingHandler
{
    public AuthenticatedHttpClientHandler()
    {
        InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var accessToken = TokenManager.AccessToken;
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        request.RequestUri = new Uri(
            request.RequestUri.AbsoluteUri.Replace("https://localhost:5001/", Global.BaseUrl)
        );
        var response = await base.SendAsync(request, cancellationToken);

        // If token expired or unauthorized, refresh the token and retry
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var isTokenRefreshed = await new AppAuthClient().RefreshToken(
                TokenManager.RefreshToken
            );
            if (isTokenRefreshed)
            {
                // Retry the request with the new token
                accessToken = TokenManager.AccessToken;
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    accessToken
                );
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        if (!response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
        }
        return response;
    }
}

public class JwtDecoder
{
    public static IList<Claim> DecodeJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        // Check if the token is valid and in the JWT format
        if (handler.CanReadToken(token))
        {
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.ToList();
            /* Decode Header*/
            // var header = jwtToken.Header;
            // Console.WriteLine("Header:");
            // foreach (var item in header)
            // {
            //     Console.WriteLine($"{item.Key}: {item.Value}");
            // }

            // // Decode Payload (Claims)
            // var payload = jwtToken.Payload;
            // Console.WriteLine("\nPayload:");
            // foreach (var claim in payload)
            // {
            //     Console.WriteLine($"{claim.Key}: {claim.Value}");
            // }

            // // Optionally: Serialize payload to JSON string
            // string jsonPayload = JsonConvert.SerializeObject(payload);
            // Console.WriteLine("\nSerialized Payload (JSON):\n" + jsonPayload);
        }
        else
        {
            Console.WriteLine("The token is not in a readable format.");
        }
        return new List<Claim>();
    }
}
