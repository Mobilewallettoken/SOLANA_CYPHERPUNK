// Decompiled with JetBrains decompiler
// Type: MPOST.IBill_API
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("584ABEDD-D0FB-419f-933E-D9AFAFEBD5E6")]
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  public interface IBill_API
  {
    [DispId(1)]
    string Country { get; }

    [DispId(2)]
    double Value { get; }

    [DispId(3)]
    string Type { get; }

    [DispId(4)]
    string Series { get; }

    [DispId(5)]
    string Compatibility { get; }

    [DispId(6)]
    string Version { get; }

    [DispId(7)]
    string ValueString { get; }

    [DispId(100)]
    string ToString();
  }
}
