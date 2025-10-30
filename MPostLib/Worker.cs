// Decompiled with JetBrains decompiler
// Type: MPOST.Worker
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.IO;

namespace MobileWallet.Desktop.MPostLib
{
  internal class Worker
  {
    private static readonly int MAX_RETRIES = 10;
    private static readonly int POLL_PACE = 5;
    private static readonly byte CMD_OMNIBUS = 16;
    private Acceptor _acceptor;
    private bool _wasDisconnected;
    private bool _stopThread;
    private bool _bSendOneLastPoll;
    private Thread _thr;
    private bool blnLostComms;
    private bool bTimeoutOccured;

    internal Worker(Acceptor acceptor)
    {
      this._acceptor = acceptor;
      this._wasDisconnected = false;
      this._stopThread = false;
    }

    internal void stopThread(bool allowOneMorePoll)
    {
      this._stopThread = true;
      this._bSendOneLastPoll = allowOneMorePoll;
    }

    internal void startThread()
    {
      if (this.IsRunning)
        return;
      this._stopThread = false;
      this._thr = new Thread(new ThreadStart(this.run));
      this._thr.IsBackground = true;
      this._thr.Start();
    }

    internal bool IsRunning => this._thr != null && this._thr.IsAlive;

    internal bool commLost() => this.bTimeoutOccured || this.blnLostComms;

    private void run()
    {
      byte[] data = (byte[]) null;
      byte[] numArray = new byte[0];
      bool flag = false;
      int num1 = 0;
      long ticks = DateTime.Now.Ticks;
      long num2 = 0;
      while (!this._stopThread || this._bSendOneLastPoll && this._acceptor.Connected)
      {
        if (this._acceptor.IsInSoftResetWaitForReply)
          Thread.Sleep(200);
        else
          Thread.Sleep(25);
        if (!this._acceptor.getTransport().IsOpen)
          this.blnLostComms = true;
        if (this._acceptor.getTransport().IsOpen || this.blnLostComms)
        {
          TimeSpan timeSpan = TimeSpan.FromTicks(DateTime.Now.Ticks - ticks);
          if (timeSpan.TotalMilliseconds > (double) Acceptor.POWERUP_REARM_TIME)
            this._acceptor._raisePowerUpEvent = true;
          if (this.bTimeoutOccured)
          {
            timeSpan = TimeSpan.FromTicks(DateTime.Now.Ticks - ticks);
            if (timeSpan.TotalMilliseconds > (double) this._acceptor.DisconnectTimeout)
            {
              if (this._acceptor.DeviceState != State.Downloading && this._acceptor.DeviceState != State.DownloadRestart && this._acceptor._bDownloadSuccess && this._acceptor.Connected)
              {
                this._acceptor.setConnected(false);
                this._acceptor.RaiseDisconnectedEvent();
                this._wasDisconnected = true;
              }
              ticks = DateTime.Now.Ticks;
            }
          }
          Message message1;
          if (this._acceptor._cmdQueue.Count > 0)
          {
            message1 = this._acceptor._cmdQueue.Dequeue();
            num2 = DateTime.Now.Ticks;
          }
          else
            message1 = (Message) null;
          for (int index = 0; index <= Worker.MAX_RETRIES + 1; ++index)
          {
            try
            {
              if (index > 0)
                Thread.Sleep(25);
              if (index == Worker.MAX_RETRIES)
              {
                this.blnLostComms = true;
                if (message1 != null)
                {
                  this._acceptor.RaiseErrorWhileSendingMessageEvent(message1);
                  break;
                }
                break;
              }
              Message message2;
              if (message1 != null && message1.IsSynchronous)
              {
                timeSpan = TimeSpan.FromTicks(DateTime.Now.Ticks - num2);
                if (timeSpan.TotalMilliseconds > 1000.0)
                {
                  this._acceptor._replyQueue.Enqueue(new byte[0]);
                  message2 = (Message) null;
                  break;
                }
              }
              if (data != null)
                this._acceptor.getTransport().sendData(data);
              else if (message1 != null)
              {
                num1 = 0;
                if (message1.Payload == null || message1.Payload.Length == 0)
                {
                  this._acceptor.RaiseInvalidCommandEvent();
                  if (message1.IsSynchronous)
                    this._acceptor._replyQueue.Enqueue(new byte[0]);
                  message2 = (Message) null;
                  break;
                }
                data = message1.Payload;
                this._acceptor.getTransport().sendData(data);
                if (message1.IsNoReplyExpected)
                {
                  this._acceptor.getTransport().logCommandAndReply(this._acceptor.getTransport().GetCurrentCommand, new byte[0], true);
                  data = (byte[]) null;
                  break;
                }
                if (message1.IsSynchronous)
                  data = (byte[]) null;
              }
              else
              {
                if (this._acceptor.SupressStandardPoll)
                {
                  ticks = DateTime.Now.Ticks;
                  num1 = 0;
                  break;
                }
                ++num1;
                if (num1 >= Worker.POLL_PACE)
                {
                  num1 = 0;
                  if (this._bSendOneLastPoll)
                    flag = true;
                  data = this._acceptor.constructOmnibusCommand(4, Worker.CMD_OMNIBUS, 1);
                  this._acceptor.getTransport().sendData(data);
                }
                else
                  break;
              }
              byte[] reply = this._acceptor.getTransport().receive();
              ticks = DateTime.Now.Ticks;
              this.bTimeoutOccured = false;
              if (this._wasDisconnected)
              {
                this._wasDisconnected = false;
                if (!this._acceptor.connectorThreadIsRunning() && ((int) reply[2] & 112) != 80)
                  this._acceptor.setConnected(true);
              }
              if (this._acceptor.IsInSoftResetWaitForReply)
                this._acceptor.IsInSoftResetWaitForReply = false;
              this._acceptor.setReplyAcked(this._acceptor.getTransport().getReplyAcked(reply));
              if (message1 != null && message1.IsSynchronous)
                this._acceptor._replyQueue.Enqueue(reply);
              else
                this._acceptor.processReply(reply);
              if (flag && message1 == null)
              {
                flag = false;
                this._bSendOneLastPoll = false;
              }
              this.blnLostComms = false;
              data = (byte[]) null;
              break;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                App.AppLogger.Error(ex,ex.Message);
              Thread.Sleep(1000);
              this.bTimeoutOccured = true;
              this._acceptor.log(string.Format("Exception with Port. Attempting Reset. - {0}", (object) ex.Message));
              this._acceptor.getTransport().resetPort(ticks);
              this.blnLostComms = !this._acceptor.getTransport().IsOpen;
            }
            catch (TimeoutException ex)
            {
              Thread.Sleep(1000);
              this.bTimeoutOccured = true;
              if (!this._acceptor.getTransport().IsOpen)
                this.blnLostComms = true;
              this._acceptor.getTransport().resetPort(ticks);
              this._acceptor.log(string.Format("Timeout Exception : {0}", (object) ex.Message));
              if (ex.InnerException != null)
                this._acceptor.log(string.Format("Timeout Exception - INNER : {0}", (object) ex.InnerException.Message));
              this.blnLostComms = !this._acceptor.getTransport().IsOpen;
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex,ex.Message);
              this._acceptor.log(string.Format("Unknown Exception : {0}", (object) ex.Message));
              if (ex.InnerException != null)
                this._acceptor.log(string.Format("Unknown Exception - INNER : {0}", (object) ex.InnerException.Message));
            }
          }
        }
      }
      this._acceptor._cmdQueue.Clear();
      this._acceptor.getTransport().close();
    }
  }
}
