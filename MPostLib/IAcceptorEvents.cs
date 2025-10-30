// Decompiled with JetBrains decompiler
// Type: MPOST.IAcceptorEvents
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("96CD7E9C-E83A-4c52-AA32-6834CAA33018")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  public interface IAcceptorEvents
  {
    [DispId(1)]
    void OnCalibrateFinish(object sender, EventArgs e);

    [DispId(2)]
    void OnCalibrateProgress(object sender, EventArgs e);

    [DispId(3)]
    void OnCalibrateStart(object sender, EventArgs e);

    [DispId(4)]
    void OnCashBoxCleanlinessDetected(object sender, CashBoxCleanlinessEventArgs e);

    [DispId(5)]
    void OnCashBoxAttached(object sender, EventArgs e);

    [DispId(6)]
    void OnCashBoxRemoved(object sender, EventArgs e);

    [DispId(7)]
    void OnCheated(object sender, EventArgs e);

    [DispId(8)]
    void OnConnected(object sender, EventArgs e);

    [DispId(9)]
    void OnDisconnected(object sender, EventArgs e);

    [DispId(10)]
    void OnDownloadFinish(object sender, AcceptorDownloadFinishEventArgs e);

    [DispId(11)]
    void OnDownloadProgress(object sender, AcceptorDownloadEventArgs e);

    [DispId(12)]
    void OnDownloadRestart(object sender, EventArgs e);

    [DispId(13)]
    void OnDownloadStart(object sender, AcceptorDownloadEventArgs e);

    [DispId(14)]
    void OnSendMessageFailure(object sender, AcceptorMessageEventArgs e);

    [DispId(15)]
    void OnEscrow(object sender, EventArgs e);

    [DispId(16)]
    void OnFailureCleared(object sender, EventArgs e);

    [DispId(17)]
    void OnFailureDetected(object sender, EventArgs e);

    [DispId(18)]
    void OnInvlaidCommand(object sender, EventArgs e);

    [DispId(19)]
    void OnJamCleared(object sender, EventArgs e);

    [DispId(20)]
    void OnJamDetected(object sender, EventArgs e);

    [DispId(21)]
    void OnNoteRetrieved(object sender, EventArgs e);

    [DispId(22)]
    void OnPauseCleared(object sender, EventArgs e);

    [DispId(23)]
    void OnPauseDetected(object sender, EventArgs e);

    [DispId(24)]
    void OnPowerUpComplete(object sender, EventArgs e);

    [DispId(25)]
    void OnPowerUp(object sender, EventArgs e);

    [DispId(26)]
    void OnPupEscrow(object sender, EventArgs e);

    [DispId(27)]
    void OnRejected(object sender, EventArgs e);

    [DispId(28)]
    void OnReturned(object sender, EventArgs e);

    [DispId(29)]
    void OnStacked(object sender, EventArgs e);

    [DispId(30)]
    void OnStackerFullCleared(object sender, EventArgs e);

    [DispId(31)]
    void OnStackerFull(object sender, EventArgs e);

    [DispId(32)]
    void OnStallCleared(object sender, EventArgs e);

    [DispId(33)]
    void OnStallDetected(object sender, EventArgs e);

    [DispId(34)]
    void OnClearAuditComplete(object sender, ClearAuditEventArgs e);

    [DispId(35)]
    void OnStackedWithDocInfo(object sender, StackedEventArgs e);
  }
}
