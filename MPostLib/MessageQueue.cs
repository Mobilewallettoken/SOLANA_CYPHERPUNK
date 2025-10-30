// Decompiled with JetBrains decompiler
// Type: MPOST.MessageQueue
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.Collections;

namespace MobileWallet.Desktop.MPostLib
{
  public class MessageQueue
  {
    private Queue rootQueue;
    private Queue synchQueue;

    public MessageQueue()
    {
      this.rootQueue = new Queue();
      this.synchQueue = Queue.Synchronized(this.rootQueue);
    }

    public int Count => this.synchQueue.Count;

    public void PostMessage(
      byte[] payload,
      bool isSynchronous,
      string description,
      bool noReplyExpected)
    {
      this.synchQueue.Enqueue((object) new Message(payload, isSynchronous, description, noReplyExpected));
    }

    public Message GetMessage() => this.Count > 0 ? this.synchQueue.Dequeue() as Message : (Message) null;

    public void Clear() => this.synchQueue.Clear();
  }
}
