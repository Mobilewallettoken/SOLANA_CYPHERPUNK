// Decompiled with JetBrains decompiler
// Type: MPOST.Barcode
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("1D1859D3-5EB3-4383-B959-EB5E0889C903")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.None)]
  public class Barcode : IBarcode_API, IDocument
  {
    private string barcodeValue;

    public string BarcodeValue => this.barcodeValue;

    public string ValueString => this.BarcodeValue;

    public override string ToString() => string.Format("{0}", (object) this.BarcodeValue);

    public Barcode(string barcodeValue) => this.barcodeValue = barcodeValue;
  }
}
