// Decompiled with JetBrains decompiler
// Type: MPOST.EBDS_SerialPort
// Assembly: MPOST, Version=3.0.9.1, Culture=neutral, PublicKeyToken=null
// MVID: F8C15F18-2FEE-452E-9333-842EC1A1B67F
// Assembly location: C:\Users\hhala\Downloads\MPOST.dll

using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;

namespace MobileWallet.Desktop.MPostLib
{
    internal class EBDS_SerialPort
    {
        private static readonly byte STX = 2;
        private static readonly byte ETX = 3;
        private static readonly byte ACK_MASK = 1;
        private static readonly int RESET_PORT_DURATION_MS = 5000;
        private static readonly int RESET_PORT_SLEEP_TIME_MS = 500;
        private byte[] _bArrCurrentCommand = new byte[0];
        private byte[] _bArrPreviousCommand;
        private byte[] _bArrPreviousReply;
        private DateTime _dtSent;
        private byte _bAckToggle;
        private int _iNakCount;
        private int _iIdenticalCommandAndReplyCount;
        private SerialPort _port;
        private byte[] _bArrBuffer = new byte[256];
        private int _iRead;
        private int _iWrite;
        private byte[] _arrBuffer = new byte[256];
        private volatile bool _bStopPortReading;
        private Acceptor _acceptor;
        private object _objLogLock = new object();

        public byte[] GetCurrentCommand => this._bArrCurrentCommand;

        public int BaudRate
        {
            get => this._port?.BaudRate ?? 0;
            set
            {
                this._port.BaudRate = value;
                if (!this._port.IsOpen)
                    return;
                this._port.DiscardInBuffer();
            }
        }

        public int Databits
        {
            get => this._port.DataBits;
            set
            {
                this._port.DataBits = value;
                if (!this._port.IsOpen)
                    return;
                this._port.DiscardInBuffer();
            }
        }

        public StopBits StopBits
        {
            get => this._port.StopBits;
            set
            {
                this._port.StopBits = value;
                if (!this._port.IsOpen)
                    return;
                this._port.DiscardInBuffer();
            }
        }

        public Parity Parity
        {
            get => this._port.Parity;
            set
            {
                this._port.Parity = value;
                if (!this._port.IsOpen)
                    return;
                this._port.DiscardInBuffer();
            }
        }

        public string PortName
        {
            get
            {
                if (this._port == null)
                    return "Not Initialized";
                return this._port.IsOpen ? this._port.PortName : "Not Connected";
            }
            set
            {
                if (this._port.IsOpen)
                    throw new Exception("Port is Open: Cannot change port name");
                this._port.PortName = value;
            }
        }

        public bool IsOpen => this._port != null && this._port.IsOpen;

        public eProtocolLevel ProtocolLevel { get; private set; }

        internal void stopPortReading() => this._bStopPortReading = true;

        public EBDS_SerialPort(Acceptor acceptor)
        {
            this._acceptor = acceptor;
            this._port = new SerialPort();
            this._port.BaudRate = 9600;
            this._port.DataBits = 7;
            this._port.StopBits = StopBits.One;
            this._port.Parity = Parity.Even;
            this._port.ReadTimeout = 500;
            this._port.WriteTimeout = 500;
        }

        public static string[] listPorts() => SerialPort.GetPortNames();

        public bool openPort(string portName)
        {
            if (this._port.IsOpen)
                this._port.Close();
            this._port.PortName = portName;
            this._port.Open();
            this._bStopPortReading = false;
            this.ReadAsync();
            return this._port.IsOpen;
        }

        public void close()
        {
            this._port.Close();
            Dispose();
        }

        public void resetPort(long timeoutStartCountTicks)
        {
            double num1 =
                (double)this._acceptor.DisconnectTimeout
                - TimeSpan.FromTicks(DateTime.Now.Ticks - timeoutStartCountTicks).TotalMilliseconds;
            int num2 = 0;
            int num3 =
                num1 >= (double)EBDS_SerialPort.RESET_PORT_DURATION_MS
                    ? EBDS_SerialPort.RESET_PORT_DURATION_MS
                    : Convert.ToInt32(num1);
            this._acceptor.log(string.Format("Reset port limit - {0} ms.", (object)num3));
            try
            {
                this.close();
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex,ex.Message);
                this._acceptor.log(
                    string.Format(
                        "Exception during Port Close - {0} - {1}",
                        (object)ex.GetType().ToString(),
                        (object)ex.Message
                    )
                );
            }
            while (num2 < num3)
            {
                num2 += EBDS_SerialPort.RESET_PORT_SLEEP_TIME_MS;
                Thread.Sleep(EBDS_SerialPort.RESET_PORT_SLEEP_TIME_MS);
                foreach (string listPort in EBDS_SerialPort.listPorts())
                {
                    if (this._port != null && listPort == this._port.PortName)
                    {
                        try
                        {
                            this._port.Open();
                            return;
                        }
                        catch (Exception ex)
                        {
                App.AppLogger.Error(ex,ex.Message);
                            this._acceptor.log(
                                string.Format(
                                    "Exception during Port Re-Open - {0} - {1}",
                                    (object)ex.GetType().ToString(),
                                    (object)ex.Message
                                )
                            );
                            return;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void sendData(byte[] data)
        {
            this.flushInputStream();
            if (data == null)
                throw new Exception("Message Payload is null");
            int length = data.Length + 4;
            byte[] numArray = new byte[length];
            numArray[0] = EBDS_SerialPort.STX;
            numArray[1] = (byte)length;
            Array.Copy((Array)data, 0, (Array)numArray, 2, data.Length);
            numArray[2] |= this._bAckToggle;
            numArray[length - 2] = EBDS_SerialPort.ETX;
            numArray[length - 1] = this.calcCRC(numArray);
            this._bArrCurrentCommand = numArray;
            this._dtSent = DateTime.Now;
            this._port.Write(numArray, 0, numArray.Length);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] receive()
        {
            byte[] reply = new byte[0];
            int num1 = 250;
            bool flag1 = false;
            byte num2 = 0;
            byte num3 = 0;
            int index1 = 0;
            int num4 = 0;
            if (this._acceptor.IsInSoftResetWaitForReply)
            {
                this._acceptor.IsInSoftResetWaitForReply = false;
                Thread.Sleep(1000);
                this.flushInputStream();
            }
            if (
                this._acceptor.DeviceState == State.Downloading
                || this._acceptor.DeviceState == State.DownloadRestart
                || this._acceptor.DeviceState == State.DownloadStart
            )
                num1 = 1000;
            DateTime now = DateTime.Now;
            long ticks = now.Ticks;
            while (!flag1)
            {
                now = DateTime.Now;
                if ((now.Ticks - ticks) / 10000L > (long)num1)
                {
                    this.flushInputStream();
                    this.logCommandAndReply(this._bArrCurrentCommand, reply, false);
                    throw new TimeoutException("Did not receive reply in time");
                }
                if (this._iRead == this._iWrite)
                {
                    Thread.Sleep(25);
                }
                else
                {
                    try
                    {
                        byte length = this._bArrBuffer[this._iRead++];
                        switch (num4)
                        {
                            case 0:
                                if ((int)length == (int)EBDS_SerialPort.STX)
                                {
                                    ++num4;
                                    continue;
                                }
                                continue;
                            case 1:
                                reply = new byte[(int)length];
                                byte[] numArray1 = reply;
                                int index2 = index1;
                                int num5 = index2 + 1;
                                int stx = (int)EBDS_SerialPort.STX;
                                numArray1[index2] = (byte)stx;
                                byte[] numArray2 = reply;
                                int index3 = num5;
                                index1 = index3 + 1;
                                int num6 = (int)length;
                                numArray2[index3] = (byte)num6;
                                num3 = num2 = length;
                                ++num4;
                                continue;
                            case 2:
                                if (index1 < (int)num2 - 2)
                                {
                                    if (index1 == (int)num2 - 3)
                                        ++num4;
                                    num3 ^= length;
                                    reply[index1++] = length;
                                    continue;
                                }
                                continue;
                            case 3:
                                reply[index1++] =
                                    (int)length == (int)EBDS_SerialPort.ETX
                                        ? EBDS_SerialPort.ETX
                                        : throw new Exception(
                                            "Invalid Response - Expected ETX. Got: "
                                                + length.ToString()
                                        );
                                ++num4;
                                continue;
                            case 4:
                                reply[index1] =
                                    (int)length == (int)num3
                                        ? length
                                        : throw new Exception(
                                            "Invalid Response - Bad Checksum. Expected: "
                                                + num3.ToString()
                                                + " Got: "
                                                + length.ToString()
                                        );
                                flag1 = true;
                                continue;
                            default:
                                continue;
                        }
                    }
                    catch (Exception ex)
                    {
                App.AppLogger.Error(ex,ex.Message);
                        this.flushInputStream();
                        this.logCommandAndReply(this._bArrCurrentCommand, reply, false);
                        throw new Exception("Invalid Operation", ex);
                    }
                }
            }
            if (reply.Length == this._bArrCurrentCommand.Length)
            {
                bool flag2 = true;
                if (reply.Length == 6 && ((int)reply[2] & 240) == 80)
                {
                    flag2 = false;
                }
                else
                {
                    for (int index4 = 0; index4 < reply.Length; ++index4)
                    {
                        if ((int)reply[index4] != (int)this._bArrCurrentCommand[index4])
                        {
                            flag2 = false;
                            break;
                        }
                    }
                }
                if (flag2)
                {
                    this.logCommandAndReply(this._bArrCurrentCommand, reply, true);
                    throw new Exception("Echo Detected");
                }
            }
            this.logCommandAndReply(this._bArrCurrentCommand, reply, false);
            return reply;
        }

        public bool getReplyAcked(byte[] reply)
        {
            if (reply.Length < 3)
                return false;
            if (((int)reply[2] & (int)EBDS_SerialPort.ACK_MASK) == (int)this._bAckToggle)
            {
                this._bAckToggle ^= (byte)1;
                this._iNakCount = 0;
                return true;
            }
            ++this._iNakCount;
            if (this._iNakCount == 8)
            {
                this._bAckToggle ^= (byte)1;
                this._iNakCount = 0;
            }
            return false;
        }

        private byte calcCRC(byte[] b)
        {
            byte num = 0;
            for (int index = 1; index < (int)b[1] - 2; ++index)
                num ^= b[index];
            return num;
        }

        private void ReadAsync()
        {
            if (_port == null)
            {
                return;
            }
            while (this._port is { IsOpen: false })
            {
                Thread.Sleep(100);
                if (this._bStopPortReading)
                    return;
            }
            try
            {
                if (this._port == null)
                {
                    return;
                }
                this._port.BaseStream.BeginRead(
                    this._arrBuffer,
                    0,
                    this._arrBuffer.Length,
                    (AsyncCallback)(
                        ar =>
                        {
                            try
                            {
                                int num = this._port.BaseStream.EndRead(ar);
                                for (int index = 0; index < num; ++index)
                                    this._bArrBuffer[this._iWrite++] = this._arrBuffer[index];
                            }
                            catch (InvalidOperationException ex)
                            {
                                this._acceptor.log(
                                    string.Format(
                                        "{0}. {1} : {2}",
                                        (object)"Failed to read bytes - Was stream closed?",
                                        (object)ex.GetType(),
                                        (object)ex.Message
                                    )
                                );
                            }
                            catch (Exception ex)
                            {
                App.AppLogger.Error(ex,ex.Message);
                                this._acceptor.log(
                                    string.Format(
                                        "{0}. {1} : {2}",
                                        (object)"Failed to read bytes",
                                        (object)ex.GetType(),
                                        (object)ex.Message
                                    )
                                );
                            }
                            this.ReadAsync();
                        }
                    ),
                    (object)null
                );
            }
            catch (Exception ex)
            {
                this._acceptor.log(
                    string.Format(
                        "{0}. {1} : {2}",
                        (object)"Failed to read bytes (Outer)",
                        (object)ex.GetType(),
                        (object)ex.Message
                    )
                );
                this.ReadAsync();
            }
        }

        private void flushInputStream()
        {
            this._iRead = 0;
            this._iWrite = 0;
        }

        public void SetProtocolLevel(eProtocolLevel level)
        {
            if (level == this.ProtocolLevel)
                return;
            switch (level)
            {
                case eProtocolLevel.STANDARD:
                    this.BaudRate = 9600;
                    this.Databits = 7;
                    this.Parity = Parity.Even;
                    break;
                case eProtocolLevel.FAST_DOWNLOAD_SERIAL:
                    this.BaudRate = 38400;
                    this.Databits = 8;
                    this.Parity = Parity.None;
                    break;
            }
            this.ProtocolLevel = level;
        }

        internal void flushIdenticalCommands()
        {
            lock (this._objLogLock)
            {
                if (this._iIdenticalCommandAndReplyCount > 0)
                    this._acceptor.log(
                        string.Format(
                            "    : {0} transactions identical to previous.",
                            (object)this._iIdenticalCommandAndReplyCount
                        )
                    );
                this._iIdenticalCommandAndReplyCount = 0;
            }
        }

        public void logCommandAndReply(byte[] command, byte[] reply, bool wasEchoDiscarded)
        {
            if (!this._acceptor.DebugLog)
                return;
            lock (this._objLogLock)
            {
                bool flag1 = true;
                for (int index = 0; index < command.Length - 1; ++index)
                {
                    if (
                        this._bArrPreviousCommand == null
                        || index >= this._bArrPreviousCommand.Length
                    )
                    {
                        flag1 = false;
                        break;
                    }
                    if (index != 2)
                    {
                        if ((int)command[index] != (int)this._bArrPreviousCommand[index])
                        {
                            flag1 = false;
                            break;
                        }
                    }
                    else if (
                        ((int)command[index] & 126) != ((int)this._bArrPreviousCommand[index] & 126)
                    )
                    {
                        flag1 = false;
                        break;
                    }
                }
                bool flag2 = true;
                if (reply.Length != 0)
                {
                    for (int index = 0; index < reply.Length - 1; ++index)
                    {
                        if (
                            this._bArrPreviousReply == null
                            || index >= this._bArrPreviousReply.Length
                        )
                        {
                            flag2 = false;
                            break;
                        }
                        if (index != 2)
                        {
                            if ((int)reply[index] != (int)this._bArrPreviousReply[index])
                            {
                                flag2 = false;
                                break;
                            }
                        }
                        else if (
                            ((int)reply[index] & 126) != ((int)this._bArrPreviousReply[index] & 126)
                        )
                        {
                            flag2 = false;
                            break;
                        }
                    }
                }
                else
                    flag2 = false;
                if (flag1 & flag2)
                {
                    ++this._iIdenticalCommandAndReplyCount;
                }
                else
                {
                    if (this._iIdenticalCommandAndReplyCount > 0)
                        this._acceptor.log(
                            string.Format(
                                "    : {0} transactions identical to previous.",
                                (object)this._iIdenticalCommandAndReplyCount
                            )
                        );
                    this._iIdenticalCommandAndReplyCount = 0;
                    StringBuilder stringBuilder1 = new StringBuilder("SEND: ");
                    foreach (byte num in command)
                    {
                        stringBuilder1.Append(num.ToString("X2"));
                        stringBuilder1.Append(' ');
                    }
                    this._acceptor.log(stringBuilder1.ToString(), this._dtSent);
                    StringBuilder stringBuilder2 = new StringBuilder("RECV: ");
                    if (reply.Length != 0)
                    {
                        foreach (byte num in reply)
                        {
                            stringBuilder2.Append(num.ToString("X2"));
                            stringBuilder2.Append(' ');
                        }
                    }
                    else if (!wasEchoDiscarded)
                        stringBuilder2.Append("NO REPLY");
                    else
                        stringBuilder2.Append("ECHO DISCARDED");
                    this._acceptor.log(stringBuilder2.ToString());
                    this._bArrPreviousReply = reply;
                    this._bArrPreviousCommand = command;
                }
            }
        }

        public void Dispose()
        {
            this._port?.Dispose();
            this._port = null;
        }
    }
}
