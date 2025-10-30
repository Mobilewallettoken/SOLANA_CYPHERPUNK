// Decompiled with JetBrains decompiler
// Type: MPOST.Bill
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("BFF7163E-28CC-4f1b-B1D6-28C38A36567B")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.None)]
  public class Bill : IBill_API, IDocument
  {
    private string billCountry;
    private double billValue;
    private string billType;
    private string billSeries;
    private string billComp;
    private string billVers;

    public Bill()
    {
      this.billCountry = "***";
      this.billValue = 0.0;
      this.billType = "*";
      this.billSeries = "*";
      this.billComp = "*";
      this.billVers = "*";
    }

    public Bill(
      string country,
      double value,
      string type,
      string series,
      string compatibility,
      string version)
    {
      this.billCountry = country;
      this.billValue = value;
      this.billType = type;
      this.billSeries = series;
      this.billComp = compatibility;
      this.billVers = version;
    }

    public string Country
    {
      get => this.billCountry;
      internal set => this.billCountry = value;
    }

    public double Value
    {
      get => this.billValue;
      internal set => this.billValue = value;
    }

    public string Type
    {
      get => this.billType;
      internal set => this.billType = value;
    }

    public string Series
    {
      get => this.billSeries;
      internal set => this.billSeries = value;
    }

    public string Compatibility
    {
      get => this.billComp;
      internal set => this.billComp = value;
    }

    public string Version
    {
      get => this.billVers;
      internal set => this.billVers = value;
    }

    public override string ToString() => string.Format("{0} {1} {2} {3} {4} {5}", (object) this.billCountry, (object) this.billValue, (object) this.billType, (object) this.billSeries, (object) this.billComp, (object) this.billVers);

    public string ValueString => string.Format("{0} {1} {2} {3} {4} {5}", (object) this.billCountry, (object) this.billValue, (object) this.billType, (object) this.billSeries, (object) this.billComp, (object) this.billVers);
  }
}
