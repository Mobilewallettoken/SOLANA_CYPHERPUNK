// Decompiled with JetBrains decompiler
// Type: MPOST.CashBoxCleanlinessEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("1180f8a1-4692-11e1-b86c-0800200c9a66")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.None)]
  public class CashBoxCleanlinessEventArgs : EventArgs, ICashBoxCleanlinessEventArgs
  {
    private CashBoxCleanlinessEnum _value;

    public CashBoxCleanlinessEventArgs(CashBoxCleanlinessEnum value) => this._value = value;

    public CashBoxCleanlinessEnum Value => this._value;
  }
}
