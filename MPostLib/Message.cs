// Decompiled with JetBrains decompiler
// Type: MPOST.Message
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

namespace MobileWallet.Desktop.MPostLib
{
  public class Message
  {
    private byte[] _payload;
    private bool _isSynchronous;
    private string _description;
    private bool _noReplyExpected;

    public Message(byte[] payload, bool isSynchronous, string description, bool noReplyExpected)
    {
      this._payload = payload;
      this._isSynchronous = isSynchronous;
      this._description = description;
      this._noReplyExpected = noReplyExpected;
    }

    public bool IsSynchronous => this._isSynchronous;

    public byte[] Payload => this._payload;

    public string Description => this._description;

    public bool IsNoReplyExpected => this._noReplyExpected;
  }
}
