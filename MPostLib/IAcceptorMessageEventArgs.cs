// Decompiled with JetBrains decompiler
// Type: MPOST.IAcceptorMessageEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("59E738B5-3C93-4cd1-9868-28D5AA200B02")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  public interface IAcceptorMessageEventArgs
  {
    Message Msg { [DispId(1)] get; }
  }
}
