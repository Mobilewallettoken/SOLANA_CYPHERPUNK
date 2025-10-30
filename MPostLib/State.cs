// Decompiled with JetBrains decompiler
// Type: MPOST.State
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("2CCC8E5A-B181-492c-94F6-AE6DEEADAA4C")]
  [ComVisible(true)]
  public enum State
  {
    Disconnected,
    Connecting,
    PupEscrow,
    Idling,
    Accepting,
    Escrow,
    Stacking,
    Stacked,
    Returning,
    Returned,
    Rejected,
    Jammed,
    Stalled,
    Failed,
    CalibrateStart,
    Calibrating,
    DownloadStart,
    DownloadRestart,
    Downloading,
  }
}
