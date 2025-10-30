// Decompiled with JetBrains decompiler
// Type: MPOST.AcceptorDownloadFinishEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("CA1AFF99-71FE-423f-8C2D-9E50FBA1D9B8")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.None)]
  public class AcceptorDownloadFinishEventArgs : EventArgs, IAcceptorDownloadFinishEventArgs
  {
    private bool success;

    public AcceptorDownloadFinishEventArgs(bool success) => this.success = success;

    public bool Success => this.success;
  }
}
