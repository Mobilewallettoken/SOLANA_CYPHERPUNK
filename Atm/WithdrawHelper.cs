using ITL;
using MobileWallet.Desktop.API;
using MobileWallet.Desktop.Client;
using MobileWallet.Desktop.Extensions;

namespace MobileWallet.Desktop.Atm;

public class WithdrawHelperV2 { }

public abstract class WithdrawHelper
{
    private const int V = 0;

    public static async Task<bool> IsAvailableCurrencyV2(int amount)
    {
        List<DenominationRecordDto> list = App
            .CashDevice.currencyAssignment.Select(p => new DenominationRecordDto()
            {
                CassetteNo = p.Channel.ToString(),
                Denomination = (int)p.Value / 100,
                Count = (int)p.Stored,
            })
            .ToList();
        list = list.OrderByDescending(p => p.Denomination).ToList();
        var notes = GetNotesV2(list, amount);
        return notes.Any(p => p.Count > 0);
    }

    public static List<DenominationRecordDto> GetNotesV2(
        List<DenominationRecordDto> list,
        int remaining
    )
    {
        App.AppLogger.Debug($"{DateTime.Now} : Started Get notes");
        if (!list.Any())
        {
            return new List<DenominationRecordDto>();
        }

        var changedListed = list.DeepCopy() ?? [];
        var notes = list.DeepCopy() ?? [];
        notes.ForEach(p =>
        {
            p.Count = 0;
        });
        for (int d = 0; d <= remaining / changedListed[6].Denomination; d++)
        {
            for (int c = 0; c <= remaining / changedListed[5].Denomination; c++)
            {
                for (int b = 0; b <= remaining / changedListed[4].Denomination; b++)
                {
                    for (int a = 0; a <= remaining / changedListed[3].Denomination; a++)
                    {
                        for (int i = 0; i <= remaining / changedListed[2].Denomination; i++) // For 2
                        {
                            for (int j = 0; j <= remaining / changedListed[1].Denomination; j++) // For 5
                            {
                                for (int k = 0; k <= remaining / changedListed[0].Denomination; k++) // For 10
                                {
                                    // Calculate the total
                                    int total =
                                        (i * changedListed[2].Denomination)
                                        + (j * changedListed[1].Denomination)
                                        + (k * changedListed[0].Denomination)
                                        + (a * changedListed[3].Denomination)
                                        + (b * changedListed[4].Denomination)
                                        + (c * changedListed[5].Denomination)
                                        + (d * changedListed[6].Denomination);

                                    // If the total matches the target amount, print the combination
                                    if (total == remaining)
                                    {
                                        if (
                                            changedListed[2].Count >= i
                                            && changedListed[1].Count >= j
                                            && changedListed[0].Count >= k
                                            && changedListed[3].Count >= a
                                            && changedListed[4].Count >= b
                                            && changedListed[5].Count >= c
                                            && changedListed[6].Count >= d
                                        )
                                        {
                                            notes[0].Count = k;
                                            notes[1].Count = j;
                                            notes[2].Count = i;
                                            notes[3].Count = a;
                                            notes[4].Count = b;
                                            notes[5].Count = c;
                                            notes[6].Count = d;
                                            return notes.ToList();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return notes.ToList();
    }

    public static async Task<bool> IsAvailableCurrency(int amount, List<CassetteDTO> cassettes)
    {
        var transactionClient = new DenominationClient(HttpClientSingleton.Instance);
        var response = (await transactionClient.Denomination_GetDenominationByAtmAsync());
        List<DenominationRecordDto> list = response.Data?.ToList() ?? [];

        App.AppLogger.Debug($"{System.Text.Json.JsonSerializer.Serialize<List<DenominationRecordDto>>(list)}");
        list = list.Where(p => p.CassetteNo != "4").OrderByDescending(p => p.Denomination).ToList();
        var notes = GetNotes(list, amount, cassettes);
        return notes.Any(p => p.Count > 0);
    }

    public static List<DenominationRecordDto> GetNotes(
        List<DenominationRecordDto> list,
        int remaining, 
        List<CassetteDTO> cassettes
    )
    {
        /*
            The GetNotes() Function returns a list notes, containing the number of each notes to be used.
            it also checks if that number of notes are available in the corresponding cassette.
            remaining is the amount to be broken into the various notes. e.g remaining could be 7000
            */
        if (!list.Any())
        {
            return new List<DenominationRecordDto>();
        }
        App.AppLogger.Debug($"{System.Text.Json.JsonSerializer.Serialize<List<CassetteDTO>>(cassettes)}");
        var changedListed = list.DeepCopy() ?? []; //this produces a true copy of the list;
        foreach (var denominationRecordDto in changedListed)
        {
            var cassette = cassettes.FirstOrDefault(p =>
                p.No == int.Parse(denominationRecordDto.CassetteNo)
            );
            if (cassette == null)
            {
                denominationRecordDto.Count = 0;
            }
            else if (cassette.Status != "No Issues" || cassette.Status != "Low Level")
            {
                denominationRecordDto.Count = 0;
            }
        }
        var notes = list.DeepCopy() ?? [];
        notes.ForEach(p =>
        {
            p.Count = 0;
        });
        // for (int i = 0; i <= remaining / changedListed[2].Denomination; i++) // For 2
        // {
        //     for (int j = 0; j <= remaining / changedListed[1].Denomination; j++) // For 5
        //     {
        //         for (int k = 0; k <= remaining / changedListed[0].Denomination; k++) // For 10
        //         {
        //             // Calculate the total
        //             int total =
        //                 (i * changedListed[2].Denomination)
        //                 + (j * changedListed[1].Denomination)
        //                 + (k * changedListed[0].Denomination);

        //             // If the total matches the target amount, print the combination
        //             if (total == remaining)
        //             {
        //                 if (
        //                     changedListed[2].Count >= i
        //                     && changedListed[1].Count >= j
        //                     && changedListed[0].Count >= k
        //                 )
        //                 {
        //                     notes[0].Count = k;
        //                     notes[1].Count = j;
        //                     notes[2].Count = i;
        //                     App.AppLogger.Debug($"{System.Text.Json.JsonSerializer.Serialize<List<DenominationRecordDto>>(notes.ToList())}");
        //                     return notes.ToList();
        //                 }
        //             }
        //         }
        //     }
        // }

        foreach (var deno in notes)
        {
            deno.Count = remaining / deno.Denomination;
            remaining %= deno.Denomination;
        }
        // if (remaining > 0)
        // {
        //     notes.Add(
        //         new DenominationRecordDto
        //         {
        //             CassetteNo = 0.ToString(),
        //             Denomination = 500,
        //             Count = remaining/500
        //         }
        //     );
        // }
        App.AppLogger.Debug($"{System.Text.Json.JsonSerializer.Serialize<List<DenominationRecordDto>>(notes.ToList())}");
        App.AppLogger.Debug("Reached Here");
        //since 2k cassette is not availabel, setting its count to 0. same with 500 cassette
        // notes[2].Count = 0;
        // notes[3].Count = 0;
        App.AppLogger.Debug($"{System.Text.Json.JsonSerializer.Serialize<List<DenominationRecordDto>>(notes.ToList())}");

        return notes.ToList();
    }

    public static async Task<List<ReceiptItem>> Withdraw(int amount, List<CassetteDTO> cassettes) //chetu code
    {
        var client = new DenominationClient(HttpClientSingleton.Instance);
        var receiptData = new List<ReceiptItem>(); //chetu code
        try
        {
            if (!Global.UseHardware || App.Dispenser.Initialize())
            {
                var remaining = amount;
                var list = (await client.Denomination_GetDenominationByAtmAsync()).Data.ToList();
                list = list.Where(p => p.CassetteNo != "4")
                    .OrderByDescending(p => p.Denomination)
                    .ToList();
                var notes = GetNotes(list, remaining, cassettes);
                string? error = null;
                if (Global.UseHardware)
                {
                    var isError = true;
                    var tryCount = 0;
                    while (tryCount <= 1)
                    {
                        var cassette1 = notes.First(p => p.CassetteNo == "1").Count.ToString();
                        var cassette2 = notes.First(p => p.CassetteNo == "2").Count.ToString();
                        var cassette3 = notes.First(p => p.CassetteNo == "3").Count.ToString();
                        var cassette4 = "0";
                        if (!Global.DisableFinalWithdraw)
                        {
                            error = App.Dispenser.move_forward(
                                cassette1,
                                cassette2,
                                cassette3,
                                cassette4
                            );
                        }

                        _ = App.LogError(
                            $"Called Move Forward on Withdraw: {cassette1},{cassette2},{cassette3},{cassette4} with Result: "
                                + error,
                            LogType.Withdraw,
                            null
                        );
                        _ = App.TrackAtmRealTime(
                            new UpdateAtmRealTimeRequestModel()
                            {
                                WithdrawMoveForwardStatus = error ?? "",
                            }
                        );
                        if (error != null && error != "Low Level")
                        {
                            await HandleError(error);
                            tryCount++;
                            isError = true;
                            continue;
                        }
                        else
                        {
                            isError = false;
                        }
                        error = App.Dispenser.deliver_notes();
                        _ = App.TrackAtmRealTime(
                            new UpdateAtmRealTimeRequestModel()
                            {
                                WithdrawDeliverNotesStatus = error ?? "",
                            }
                        );
                        _ = App.LogError(
                            $"Called Deliver Notes with Result: " + error,
                            LogType.Withdraw,
                            null
                        );
                        if (error != null)
                        {
                            isError = true;
                            await HandleError(error);
                        }
                        else
                        {
                            isError = false;
                        }
                        tryCount++;
                        if (isError == false)
                        {
                            break;
                        }
                    }
                    if (isError)
                    {
                        _ = App.LogError("ATM Error", LogType.Error, error);
                        return receiptData;
                    }
                }
                receiptData.Clear();
                list = list.OrderBy(p => p.Denomination).ToList();
                // After the successful withdrawal, populate the receiptData list
                for (var index = 0; index < notes.Count; index++)
                {
                    var noteCount = notes[index];
                    var indexOfList = list.IndexOf(
                        list.First(p => p.CassetteNo == noteCount.CassetteNo)
                    );
                    var denomination = list[indexOfList].Denomination;
                    var itemAmount = denomination * noteCount.Count;
                    list[indexOfList].Count -= noteCount.Count;
                    receiptData.Add(
                        new ReceiptItem
                        {
                            Item = denomination,
                            Quantity = noteCount.Count,
                            Amount = itemAmount,
                        }
                    );
                }
                _ = App.TrackAtmRealTime(
                    new UpdateAtmRealTimeRequestModel()
                    {
                        WithdrawDeliverNotesStatus = "",
                        WithdrawMoveForwardStatus = "",
                    }
                );
                await client.Denomination_SaveDenominationsAsync(
                    new SaveDenominationRecordsRequestModel() { Records = list }
                );
                return receiptData;
            }
            else
            {
                return receiptData;
            }
        }
        catch (Exception ex)
        {
            App.AppLogger.Error(ex, ex.Message);
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            if (Global.UseHardware) { }
        }
        return receiptData;
    }

    public static async Task HandleError(string message)
    {
        _ = App.LogError("ERROR From Shutter: " + message, LogType.Withdraw, null);
        if (message.Contains("Shutter"))
        {
            App.Dispenser.StopShutter();
            _ = App.LogError("Called Stop Shutter", LogType.Withdraw, null);
            App.Dispenser.StartShutter();
            _ = App.LogError("Called Start Shutter", LogType.Withdraw, null);
        }
        App.Dispenser.Reset();
        _ = App.LogError("Called Reset", LogType.Withdraw, null);
        App.Dispenser.OpenCassette();
        _ = App.LogError("Called OpenCassette", LogType.Withdraw, null);
        App.Dispenser.ReadCassette();
        _ = App.LogError("Called ReadCassette", LogType.Withdraw, null);
    }
}
