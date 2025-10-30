// Decompiled with JetBrains decompiler
// Type: MPOST.IBarcode_API
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("AB8FBFF9-1320-4CF2-9F81-4495573E6845")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  public interface IBarcode_API
  {
    [DispId(1)]
    string BarcodeValue { get; }

    [DispId(2)]
    string ValueString { get; }

    [DispId(100)]
    string ToString();
  }
}
