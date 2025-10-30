// Decompiled with JetBrains decompiler
// Type: MPOST.IAcceptorDownloadFinishEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("14087FC9-DDBB-40b8-92ED-5A7EA13EB592")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  public interface IAcceptorDownloadFinishEventArgs
  {
    bool Success { [DispId(1)] get; }
  }
}
