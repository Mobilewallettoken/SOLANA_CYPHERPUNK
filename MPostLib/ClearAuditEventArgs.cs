// Decompiled with JetBrains decompiler
// Type: MPOST.ClearAuditEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("45FF7E79-4AB8-4074-92FB-386CA82C0638")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.None)]
  public class ClearAuditEventArgs : EventArgs, IClearAuditEventArgs
  {
    private bool success;

    public ClearAuditEventArgs(bool success) => this.success = success;

    public bool Success => this.success;
  }
}
