namespace MobileWallet.Desktop.Atm;

using System;
using System.Collections.Generic;

public class CassetteDTO
{
    public int No { get; set; }
    public string Status { get; set; }
    public string Id { get; set; }
}


public class CassetteStatusDecoder
{
    public static List<CassetteDTO> DecodeCassetteStatus(string response)
    {
        var cassettes = new List<CassetteDTO>();
        App.AppLogger.Debug($"response : {response}");

        try
        {
            // Extract the overall response status code (first character)
            // Start decoding the cassettes
            int index = 1; // Start after the response status code
            for (int i = 0; i < 4; i++)
            {
                // Extract cassette details
                int cassetteNo = int.Parse(response[index].ToString());
                char cassetteStatusChar = response[index + 1];
                if (response.Length <= index + 2 + 5)
                {
                    continue;
                }
                string cassetteId = response.Substring(index + 2, 5);

                // Map cassette status to a description
                string cassetteStatusDescription = DisplayResult(cassetteStatusChar);

                // Add cassette information to the DTO list
                cassettes.Add(
                    new CassetteDTO
                    {
                        No = cassetteNo,
                        Status = cassetteStatusDescription,
                        Id = cassetteId,
                    }
                );

                // Move to the next cassette (8 characters per cassette)
                index += 7;
            }
        }
        catch (Exception ex)
        {
            _ = App.LogError(ex.Message, error: ex.StackTrace);
        }
        App.AppLogger.Debug("Finished decoding cassette status ");

        return cassettes;
    }

    private static string DisplayResult(char response)
    {
        string? result = response switch
        {
            '0' => "No Issues",
            '1' => "Low Level",
            '2' => "Empty Cassette",
            '3' => "Machine not opened",
            '4' => "Rejected Notes",
            '5' => "Diverter Failure",
            '6' => "Failure to feed",
            '7' => "Transmission Error",
            '8' => "Illeg Com or Com Seq",
            '9' => "Jam in note qualifier",
            ':' => "NC not press or prop ins",
            '<' => "No notes retracted",
            '?' => "RV not Pres or Prop Ins",
            '@' => "Delivery Failure",
            'A' => "Reject Failure",
            'B' => "Too many notes req",
            'C' => "Jam in note feeder transport",
            'D' => "Reject vault almost full",
            'E' => "Cassette internal failure",
            'F' => "Main Motor Failure",
            'G' => "Rejected Cheque",
            'I' => "Note Qualifier Faulty",
            'J' => "NF exit sensor failure",
            'K' => "Shutter Failure",
            'M' => "Notes in bundle output unit",
            'N' => "Communications Timeout",
            'P' => "Shutter Failure",
            'Q' => "Cassette Not Identified",
            'W' => "Error in throat",
            '[' => "Sensor Error",
            '\'' => "NMD Internal Failure",
            'a' => "Cassette Lock Faulty",
            'b' => "Error in note stacking area",
            'c' => "Module need service",
            'e' => "No Message to resend",
            'p' => "Cassette Out Error",
            _ => "Unknown Status",
        };
        return result ?? "Unknown";
    }
}
