// Decompiled with JetBrains decompiler
// Type: MPOST.IDownloadEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("0405095D-C1DD-4c87-930F-06CBC595FEF2")]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  [ComVisible(true)]
  public interface IDownloadEventArgs
  {
    int SectorCount { [DispId(1)] get; }
  }
}
