// Decompiled with JetBrains decompiler
// Type: MPOST.ICoupon_API
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("D4D08CA4-7BCB-4d81-AC0F-3EC6A7EB76A1")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  public interface ICoupon_API
  {
    [DispId(1)]
    int OwnerID { get; }

    [DispId(2)]
    double Value { get; }

    [DispId(3)]
    string ValueString { get; }

    [DispId(100)]
    string ToString();
  }
}
