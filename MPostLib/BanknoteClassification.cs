// Decompiled with JetBrains decompiler
// Type: MPOST.BanknoteClassification
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("87712F13-9F50-45C0-871E-AAD8D280FAE1")]
  [ComVisible(true)]
  public enum BanknoteClassification
  {
    Disabled,
    UnidentifiedBanknote,
    SuspectedCounterfeit,
    SuspectedZeroValueNote,
    GenuineBanknote,
  }
}
