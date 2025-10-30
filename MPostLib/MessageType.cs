// Decompiled with JetBrains decompiler
// Type: MPOST.MessageType
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("20FA3F2F-B93A-490a-81E7-17CE128F998C")]
  [ComVisible(true)]
  public enum MessageType
  {
    SetBezel,
    SetAssetNumber,
    EscrowStack,
    EscrowReturn,
    SoftReset,
    SetPUPExt,
    OTHER,
  }
}
