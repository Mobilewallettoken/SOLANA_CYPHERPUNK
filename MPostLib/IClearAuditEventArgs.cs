// Decompiled with JetBrains decompiler
// Type: MPOST.IClearAuditEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("BB25F531-7C58-475f-8748-4ECE9F58307A")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  public interface IClearAuditEventArgs
  {
    bool Success { [DispId(1)] get; }
  }
}
