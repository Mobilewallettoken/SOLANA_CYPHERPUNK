// Decompiled with JetBrains decompiler
// Type: MPOST.AcceptorMessageEventArgs
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Runtime.InteropServices;

namespace MobileWallet.Desktop.MPostLib
{
  [Guid("278DAB6B-04ED-4a25-A76F-32AAD4DB6B9B")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.None)]
  public class AcceptorMessageEventArgs : EventArgs, IAcceptorMessageEventArgs
  {
    private Message msg;

    public AcceptorMessageEventArgs(Message msg) => this.msg = msg;

    public Message Msg => this.msg;
  }
}
