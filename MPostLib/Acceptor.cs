using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileWallet.Desktop.MPostLib
{
    [Guid("688AF04C-C7BB-4b99-A337-11AD12D54992")]
    [ComSourceInterfaces(typeof(IAcceptorEvents))]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class Acceptor : IAcceptor_API
    {
        private static readonly string VERSION = "V3.91, 283795391";
        private const byte CMD_OMNIBUS = 16;
        private const byte CMD_CALIBRATE = 64;
        private const byte CMD_FLASH_DOWNLOAD = 80;
        private const byte CMD_AUXILIARY = 96;
        private const byte CMD_EXPANDED = 112;
        private const byte CMD_AUX_QUERY_SOFTWARE_CRC = 0;
        private const byte CMD_AUX_QUERY_CASHBOX_TOTAL = 1;
        private const byte CMD_AUX_QUERY_DEVICE_RESETS = 2;
        private const byte CMD_AUX_CLEAR_CASHBOX_TOTAL = 3;
        private const byte CMD_AUX_QUERY_ACCEPTOR_TYPE = 4;
        private const byte CMD_AUX_QUERY_ACCEPTOR_SN = 5;
        private const byte CMD_AUX_QUERY_ACCEPTOR_BPN = 6;
        private const byte CMD_AUX_QUERY_ACCEPTOR_APN = 7;
        private const byte CMD_AUX_QUERY_ACCEPTOR_VN = 8;
        private const byte CMD_AUX_QUERY_ACCEPTOR_VPN = 9;
        private const byte CMD_AUX_QUERY_ACCEPTOR_AUDIT_LTT = 10;
        private const byte CMD_AUX_QUERY_ACCEPTOR_AUDIT_QPM = 11;
        private const byte CMD_AUX_QUERY_ACCEPTOR_AUDIT_PM = 12;
        private const byte CMD_AUX_QUERY_DEVICE_CAPABILITIES = 13;
        private const byte CMD_AUX_QUERY_ACCEPTOR_AID = 14;
        private const byte CMD_AUX_QUERY_ACCEPTOR_VID = 15;
        private const byte CMD_AUX_QUERY_BNF_STATUS = 16;
        private const byte CMD_AUX_SET_BEZEL = 17;
        private const byte CMD_AUX_QUERY_ASSET_NUMBER = 21;
        private const byte CMD_AUX_SET_CUSTOMER_CONFIGURATION_OPTION = 37;
        private const byte CMD_AUX_QUERY_CUSTOMER_CONFIGURATION_OPTION = 38;
        public const string C_R_N =
            "Copyright © 2007-2012 CPI - The information contained here-in is the property of CPI and is not to be disclosed or used without prior written permission of CPI. This copyright extends to all media in which this information may be preserved including magnetic storage, computer print-out or visual display.";
        internal static readonly int COMMUNICATION_DISCONNECT_TIMEOUT = 30000;
        internal static readonly int POWERUP_REARM_TIME = 2000;
        private static readonly double[] COUPON_VALUES = new double[8]
        {
            0.0,
            1.0,
            2.0,
            5.0,
            10.0,
            20.0,
            50.0,
            100.0,
        };
        private EBDS_SerialPort _transport;
        private bool _connected;
        private bool _isInFailedState;
        private bool _autoStack;
        private bool _enableAcceptance;
        private State _deviceState;
        private bool _docTypeWasSetOnEscrow;
        private bool _suppressStandardPoll;
        private bool _replyAcked;
        private bool _isVeryFirstPoll = true;
        private bool _inSoftResetWaitForReply;
        private bool _capabilitiesProcessed;
        private int _disconnectTimeout = Acceptor.COMMUNICATION_DISCONNECT_TIMEOUT;
        private bool _bDisabledWhileAccpeting;
        private BNFErrorStatus _bnfErrorStatus;
        private Worker _worker;
        private Thread _openThread;
        private Thread _flashDownloadThread;
        private volatile bool _stopOpenThread;
        private volatile bool _stopFlashDownloadThread;
        private int _transactionTimeout = 50;
        private int _downloadTimeout = 250;
        private string _port;
        private PowerUp _powerUp;
        private string _barCode;
        private Bill _bill;
        private Coupon _coupon;
        private DocumentType _docType;
        private bool _debugLog;
        private string _debugLogPath;
        private StreamWriter _logWriter;
        private object _objLogLock = new object();
        private List<Bill> _lstBillTypes = new List<Bill>();
        private List<bool> _lstBillTypeEnables = new List<bool>();
        private List<Bill> _lstBillValues = new List<Bill>();
        private List<bool> _lstBillValueEnables = new List<bool>();
        private Orientation _escrowOrientation;
        private BanknoteClassification _banknoteClassification;
        private bool _enableBookmarks;
        private bool _highSecurity;
        private OrientationControl _orientationCtl;
        private OrientationControl _orientationCtlExt;
        private bool _enableNoPush;
        private bool _enableBarCodes;
        private bool _expandedNoteReporting;
        private bool _enableCouponExt;
        private bool _cheated;
        private bool _isDeviceJammed;
        private bool _cashBoxFull;
        private bool _cashBoxAttached;
        private bool _devicePaused;
        private bool _isPoweredUp;
        private bool _isQueryDeviceCapabilitiesSupported;
        private int _deviceModel;
        private int _deviceRevision;
        internal Queue<Message> _cmdQueue = new Queue<Message>();
        internal Queue<byte[]> _replyQueue = new Queue<byte[]>();
        private bool _raiseConnectedEvent;
        private bool _raiseDisconnectedEvent;
        private bool _raiseEscrowEvent = true;
        private bool _raisePUPEscrowEvent = true;
        private bool _raiseStackedEvent;
        private bool _raiseReturnedEvent;
        private bool _raiseRejectedEvent;
        private bool _raiseCheatedEvent;
        private bool _raiseStackerFullEvent = true;
        private bool _raiseStackerFullClearedEvent;
        private bool _raiseCalibrateProgressEvent = true;
        private bool _raiseCalibrateFinishEvent;
        private bool _raiseDownloadRestartEvent = true;
        private bool _raisePauseDetectedEvent = true;
        private bool _raisePauseClearedEvent;
        private bool _raiseStallDetectedEvent = true;
        private bool _raiseStallClearedEvent;
        private bool _raiseJamDetectedEvent = true;
        private bool _raiseJamClearedEvent;
        internal bool _raisePowerUpEvent = true;
        private bool _raisePowerUpCompleteEvent;
        private bool _raiseCashBoxAttachedEvent;
        private bool _raiseCashBoxRemovedEvent = true;
        private bool _raiseFailureDetectedEvent = true;
        private bool _raiseFailureClearedEvent;
        private bool _capAdvBookmark;
        private bool _capApplicationID;
        private bool _capApplicationPN;
        private bool _capAssetNumber;
        private bool _capAudit;
        private bool _capBarCodes;
        private bool _capBarCodesExt;
        private bool _capBNFStatus;
        private bool _capBookmark;
        private bool _capBootPN;
        private bool _capCalibrate;
        private bool _capCashBoxTotal;
        private bool _capClearAudit;
        private bool _capCouponExt;
        private bool _capDevicePaused;
        private bool _capDeviceSoftReset;
        private bool _capDeviceType;
        private bool _capDeviceResets;
        private bool _capDeviceSerialNumber;
        private bool _capEasiTrax;
        private bool _capEscrowTimeout;
        private bool _capFlashDownload;
        private bool _capNoPush;
        private bool _capNoteRetrieved;
        private bool _capOrientationExt;
        private bool _capPupExt;
        private bool _capSetBezel;
        private bool _capTestDoc;
        private bool _capVariantID;
        private bool _capVariantPN;
        private bool _bUseFastDownload;
        private int _currentPacket;
        private DownloadFileParser _dlFileParser;
        private bool _autoDownloadRestart;
        internal bool _bDownloadSuccess = true;
        private bool _billBeforeConnected;

        internal bool IsInSoftResetWaitForReply
        {
            get => this._inSoftResetWaitForReply;
            set => this._inSoftResetWaitForReply = value;
        }

        internal bool SupressStandardPoll => this._suppressStandardPoll;

        internal void setConnected(bool value)
        {
            if (!this._connected & value)
                this._raiseConnectedEvent = true;
            if (!value)
                this._deviceState = State.Disconnected;
            this._connected = value;
        }

        internal EBDS_SerialPort getTransport() => _transport;

        internal bool connectorThreadIsRunning() =>
            this._openThread != null && this._openThread.IsAlive;

        internal void setReplyAcked(bool replyAcked) => this._replyAcked = replyAcked;

        private void init()
        {
            this._capabilitiesProcessed = false;
            this._deviceModel = int.MinValue;
            this._deviceRevision = int.MinValue;
            this._cmdQueue = new Queue<Message>();
            this._replyQueue = new Queue<byte[]>();
            this._raiseConnectedEvent = false;
            this._raiseDisconnectedEvent = false;
            this._raiseEscrowEvent = true;
            this._raisePUPEscrowEvent = true;
            this._raiseStackedEvent = false;
            this._raiseReturnedEvent = false;
            this._raiseRejectedEvent = false;
            this._raiseCheatedEvent = false;
            this._raiseStackerFullEvent = true;
            this._raiseStackerFullClearedEvent = false;
            this._raiseCalibrateProgressEvent = true;
            this._raiseCalibrateFinishEvent = false;
            this._raiseDownloadRestartEvent = true;
            this._raisePauseDetectedEvent = true;
            this._raisePauseClearedEvent = false;
            this._raiseStallDetectedEvent = true;
            this._raiseStallClearedEvent = false;
            this._raiseJamDetectedEvent = true;
            this._raiseJamClearedEvent = false;
            this._raisePowerUpEvent = true;
            this._raisePowerUpCompleteEvent = false;
            this._raiseCashBoxAttachedEvent = false;
            this._raiseCashBoxRemovedEvent = true;
            this._raiseFailureDetectedEvent = true;
            this._raiseFailureClearedEvent = false;
        }

        private void queryDeviceCapabilities()
        {
            if (!this._isQueryDeviceCapabilitiesSupported)
                return;
            byte[] numArray = this.sendSynchronousCommand(
                new byte[4] { (byte)96, (byte)0, (byte)0, (byte)13 },
                "QryDevCaps command"
            );
            if (numArray.Length != 11)
                return;
            this._capPupExt = ((uint)numArray[3] & 1U) > 0U;
            this._capOrientationExt = ((uint)numArray[3] & 2U) > 0U;
            if (((int)numArray[3] & 4) != 0)
            {
                this._capApplicationID = true;
                this._capVariantID = true;
            }
            else
            {
                this._capApplicationID = false;
                this._capVariantID = false;
            }
            this._capBNFStatus = ((uint)numArray[3] & 8U) > 0U;
            this._capTestDoc = ((uint)numArray[3] & 16U) > 0U;
            this._capSetBezel = ((uint)numArray[3] & 32U) > 0U;
            this._capEasiTrax = ((uint)numArray[3] & 64U) > 0U;
            this._capNoteRetrieved = ((uint)numArray[4] & 1U) > 0U;
            this._capAdvBookmark = ((uint)numArray[4] & 2U) > 0U;
            this._capClearAudit = ((uint)numArray[4] & 8U) > 0U;
        }

        private void raiseEvents()
        {
            if (this._isPoweredUp && this._raisePowerUpEvent)
                this.RaisePowerUpEvent();
            if (this._isVeryFirstPoll)
            {
                this._isVeryFirstPoll = false;
            }
            else
            {
                if (this._raiseConnectedEvent)
                    this.RaiseConnectedEvent();
                if (!this.Connected)
                    return;
                if (!this._isPoweredUp && this._raisePowerUpCompleteEvent)
                {
                    if (this._raiseStackedEvent)
                        this.RaiseStackedEvent();
                    this.RaisePowerUpCompleteEvent();
                }
                if (!this._isPoweredUp && this._cashBoxAttached && this._raiseCashBoxAttachedEvent)
                    this.RaiseCashBoxAttachedEvent();
                if (this._deviceState == State.Escrow)
                {
                    if (this._isPoweredUp && this._raisePUPEscrowEvent)
                        this.RaisePUPEscrowEvent();
                    else if (
                        this._raiseEscrowEvent
                        && !this._autoStack
                        && this._connected
                        && !this._bDisabledWhileAccpeting
                    )
                        this.RaiseEscrowEvent();
                    this._bDisabledWhileAccpeting = false;
                }
                if (this._raiseStackedEvent)
                    this.RaiseStackedEvent();
                if (this._raiseReturnedEvent)
                    this.RaiseReturnedEvent();
                if (this._raiseRejectedEvent)
                {
                    this._bDisabledWhileAccpeting = false;
                    this.RaiseRejectedEvent();
                }
                if (this.DeviceState == State.Stalled && this._raiseStallDetectedEvent)
                    this.RaiseStallDetectedEvent();
                if (this.DeviceState != State.Stalled && this._raiseStallClearedEvent)
                    this.RaiseStallClearedEvent();
                if (this._cashBoxFull && this._raiseStackerFullEvent)
                    this.RaiseStackerFullEvent();
                if (!this._isPoweredUp && !this._cashBoxFull && this._raiseStackerFullClearedEvent)
                    this.RaiseStackerFullClearedEvent();
                if (this._cheated && this._raiseCheatedEvent)
                    this.RaiseCheatedEvent();
                if (!this._cashBoxAttached && this._raiseCashBoxRemovedEvent)
                    this.RaiseCashBoxRemovedEvent();
                if (this._devicePaused && this._raisePauseDetectedEvent)
                    this.RaisePauseDetectedEvent();
                if (!this._devicePaused && this._raisePauseClearedEvent)
                    this.RaisePauseClearedEvent();
                if (this._isDeviceJammed && this._raiseJamDetectedEvent)
                    this.RaiseJamDetectedEvent();
                if (!this._isPoweredUp && !this._isDeviceJammed && this._raiseJamClearedEvent)
                    this.RaiseJamClearedEvent();
                if (this._raiseCalibrateFinishEvent)
                    this.RaiseCalibrateFinishedEvent();
                if (this._isInFailedState && this._raiseFailureDetectedEvent)
                    this.RaiseFailureDetectedEvent();
                if (this._isPoweredUp || this._isInFailedState || !this._raiseFailureClearedEvent)
                    return;
                this.RaiseFailureClearedEvent();
            }
        }

        private void verifyPropertyIsAllowed(bool capabilityFlag, string propertyName)
        {
            if (!this.Connected)
                throw new InvalidOperationException(
                    string.Format(
                        "Calling {0} not allowed when not connected.",
                        (object)propertyName
                    )
                );
            if (!capabilityFlag)
                throw new InvalidOperationException(
                    string.Format("Device does not support {0}.", (object)propertyName)
                );
            if (this.DeviceState == State.DownloadStart || this.DeviceState == State.Downloading)
                throw new InvalidOperationException(
                    string.Format(
                        "Calling {0} not allowed during flash download.",
                        (object)propertyName
                    )
                );
            if (this.DeviceState == State.CalibrateStart || this.DeviceState == State.Calibrating)
                throw new InvalidOperationException(
                    string.Format(
                        "Calling {0} not allowed during calibration.",
                        (object)propertyName
                    )
                );
        }

        private void verifyConnected(string functionName)
        {
            if (!this.Connected)
                throw new InvalidOperationException(
                    string.Format(
                        "Calling {0} not allowed when not connected.",
                        (object)functionName
                    )
                );
        }

        internal byte[] constructOmnibusCommand(int payloadLength, byte controlCode, int data0Index)
        {
            byte[] numArray = new byte[payloadLength];
            numArray[0] = controlCode;
            if (
                this.EnableBookmarks
                && this.EnableAcceptance
                && this.DeviceState != State.Calibrating
            )
                numArray[0] |= (byte)32;
            byte num1 = 0;
            if (this.EnableAcceptance && this.DeviceState != State.Calibrating)
            {
                if (this._expandedNoteReporting)
                    num1 |= (byte)127;
                else if (this._lstBillTypeEnables.Count == 0)
                {
                    num1 |= (byte)127;
                }
                else
                {
                    for (int index = 0; index < this._lstBillTypeEnables.Count; ++index)
                    {
                        int num2 = 1 << index;
                        if (this._lstBillTypeEnables[index])
                            num1 |= (byte)num2;
                    }
                }
            }
            byte num3 = 0;
            if (this.HighSecurity)
                num3 |= (byte)2;
            switch (this.OrientationCtl)
            {
                case OrientationControl.FourWay:
                    num3 |= (byte)12;
                    break;
                case OrientationControl.TwoWay:
                    num3 |= (byte)4;
                    break;
            }
            byte num4 = (byte)((uint)num3 | 16U);
            byte num5 = 0;
            if (this.EnableNoPush)
                num5 |= (byte)1;
            if (
                this.EnableBarCodes
                && this.EnableAcceptance
                && this.DeviceState != State.Calibrating
            )
                num5 |= (byte)2;
            switch (this.DevicePowerUp)
            {
                case PowerUp.B:
                    num5 |= (byte)4;
                    break;
                case PowerUp.C:
                    num5 |= (byte)8;
                    break;
            }
            if (this._expandedNoteReporting)
                num5 |= (byte)16;
            if (this.EnableCouponExt && this.CapCouponExt)
                num5 |= (byte)32;
            numArray[data0Index] = num1;
            numArray[data0Index + 1] = num4;
            numArray[data0Index + 2] = num5;
            return numArray;
        }

        private void sendAsynchronousCommand(byte[] payload, string description) =>
            this.sendAsynchronousCommand(payload, description, false);

        private void sendAsynchronousCommand(
            byte[] payload,
            string description,
            bool noReplyExpected
        ) => this._cmdQueue.Enqueue(new Message(payload, false, description, noReplyExpected));

        private byte[] sendSynchronousCommand(byte[] payload, string description)
        {
            this._cmdQueue.Enqueue(new Message(payload, true, description, false));
            long ticks = DateTime.Now.Ticks;
            while (
                this._replyQueue.Count == 0
                && new TimeSpan(DateTime.Now.Ticks - ticks).TotalMilliseconds < 5000.0
            )
                Thread.Sleep(10);
            return this._replyQueue.Count != 0
                ? this._replyQueue.Dequeue()
                : throw new Exception(
                    "Unexpected timeout detected in sendSynchronousCommand function"
                );
        }

        private byte[] sendSynchronousCommandForDownload(byte[] payload, string description)
        {
            this._cmdQueue.Enqueue(new Message(payload, true, description, false));
            long ticks = DateTime.Now.Ticks;
            while (
                this._replyQueue.Count == 0
                && new TimeSpan(DateTime.Now.Ticks - ticks).TotalMilliseconds < 1000.0
            )
                Thread.Sleep(10);
            return this._replyQueue.Count != 0 ? this._replyQueue.Dequeue() : new byte[0];
        }

        private void setUpBillTable()
        {
            this.clearBillTable();
            if (this._expandedNoteReporting)
                this.retrieveBillTable();
            else
                this.buildHardCodedBillTable();
            this.buildBillValues();
        }

        private void clearBillTable()
        {
            this._lstBillTypes.Clear();
            this._lstBillTypeEnables.Clear();
            this._lstBillValues.Clear();
            this._lstBillValueEnables.Clear();
        }

        private void retrieveBillTable()
        {
            int num1 = 1;
            int num2 = 0;
            label_1:
            byte[] payload = this.constructOmnibusCommand(6, (byte)112, 2);
            payload[1] = (byte)2;
            payload[5] = (byte)num1;
            while (num2 < 3)
            {
                byte[] reply = this.sendSynchronousCommand(
                    payload,
                    string.Format("QryBillData: {0}", (object)num1)
                );
                if (reply.Length != 30)
                {
                    ++num2;
                    Thread.Sleep(100);
                }
                else if (((int)reply[2] & 112) != 112 || reply[3] != (byte)2)
                {
                    ++num2;
                    goto label_1;
                }
                else
                {
                    this.processExtendedOmnibusExpandedNoteReply(reply);
                    this.raiseEvents();
                    if (reply[10] != (byte)0)
                    {
                        this._lstBillTypes.Add(this.parseBillData(reply, 10, true));
                        ++num1;
                        num2 = 0;
                        goto label_1;
                    }
                    else
                    {
                        for (int index = 0; index < this._lstBillTypes.Count; ++index)
                            this._lstBillTypeEnables.Add(true);
                        return;
                    }
                }
            }
            throw new Exception(
                string.Format("Error Retrieving Bill Information for Index: {0}.", (object)num1)
            );
        }

        private void buildHardCodedBillTable()
        {
            int deviceModel = this._deviceModel;
            if (deviceModel <= 20)
            {
                if (deviceModel <= 12)
                {
                    if (deviceModel == 1 || deviceModel == 12)
                        ;
                }
                else if (deviceModel != 15)
                {
                    if (deviceModel == 20)
                        ;
                }
                else
                {
                    this._lstBillTypes.Add(new Bill());
                    this._lstBillTypes.Add(new Bill());
                    this._lstBillTypes.Add(new Bill("AUD", 5.0, "*", "*", "*", "*"));
                    this._lstBillTypes.Add(new Bill("AUD", 10.0, "*", "*", "*", "*"));
                    this._lstBillTypes.Add(new Bill("AUD", 20.0, "*", "*", "*", "*"));
                    this._lstBillTypes.Add(new Bill("AUD", 50.0, "*", "*", "*", "*"));
                    this._lstBillTypes.Add(new Bill("AUD", 100.0, "*", "*", "*", "*"));
                    goto label_24;
                }
            }
            else if (deviceModel <= 77)
            {
                if ((uint)(deviceModel - 30) > 1U)
                {
                    switch (deviceModel - 65)
                    {
                        case 0:
                            this._lstBillTypes.Add(new Bill("AUD", 5.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("AUD", 10.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("AUD", 20.0, "*", "*", "*", "*"));
                            goto label_24;
                        case 1:
                            this._lstBillTypes.Add(new Bill("RUR", 10.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("RUR", 50.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("RUR", 100.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("RUR", 500.0, "*", "*", "*", "*"));
                            goto label_24;
                        case 2:
                            this._lstBillTypes.Add(new Bill());
                            this._lstBillTypes.Add(new Bill());
                            this._lstBillTypes.Add(new Bill("CAD", 5.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("CAD", 10.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("CAD", 20.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("CAD", 50.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("CAD", 100.0, "*", "*", "*", "*"));
                            goto label_24;
                        case 3:
                            this._lstBillTypes.Add(new Bill("EUR", 5.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("EUR", 10.0, "*", "*", "*", "*"));
                            goto label_24;
                        case 6:
                            this._lstBillTypes.Add(new Bill());
                            this._lstBillTypes.Add(new Bill("ARS", 2.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("ARS", 5.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("ARS", 10.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("ARS", 20.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("ARS", 50.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("ARS", 100.0, "*", "*", "*", "*"));
                            goto label_24;
                        case 12:
                            this._lstBillTypes.Add(new Bill("MXP", 20.0, "*", "*", "*", "*"));
                            this._lstBillTypes.Add(new Bill("MXP", 50.0, "*", "*", "*", "*"));
                            goto label_24;
                    }
                }
            }
            else if (deviceModel != 80)
            {
                if (deviceModel == 87)
                {
                    if (this._deviceRevision < 18)
                        this._lstBillTypes.Add(new Bill("BRL", 1.0, "*", "*", "*", "*"));
                    this._lstBillTypes.Add(new Bill("BRL", 2.0, "*", "*", "*", "*"));
                    this._lstBillTypes.Add(new Bill("BRL", 5.0, "*", "*", "*", "*"));
                    this._lstBillTypes.Add(new Bill("BRL", 10.0, "*", "*", "*", "*"));
                    this._lstBillTypes.Add(new Bill("BRL", 20.0, "*", "*", "*", "*"));
                    this._lstBillTypes.Add(new Bill("BRL", 50.0, "*", "*", "*", "*"));
                    this._lstBillTypes.Add(new Bill("BRL", 100.0, "*", "*", "*", "*"));
                    goto label_24;
                }
            }
            else
            {
                this._lstBillTypes.Add(new Bill("USD", 1.0, "*", "*", "*", "*"));
                this._lstBillTypes.Add(new Bill("USD", 2.0, "*", "*", "*", "*"));
                this._lstBillTypes.Add(new Bill("USD", 5.0, "*", "*", "*", "*"));
                this._lstBillTypes.Add(new Bill("USD", 10.0, "*", "*", "*", "*"));
                this._lstBillTypes.Add(new Bill("USD", 20.0, "*", "*", "*", "*"));
                goto label_24;
            }
            this._lstBillTypes.Add(new Bill("USD", 1.0, "*", "*", "*", "*"));
            this._lstBillTypes.Add(new Bill("USD", 2.0, "*", "*", "*", "*"));
            this._lstBillTypes.Add(new Bill("USD", 5.0, "*", "*", "*", "*"));
            this._lstBillTypes.Add(new Bill("USD", 10.0, "*", "*", "*", "*"));
            this._lstBillTypes.Add(new Bill("USD", 20.0, "*", "*", "*", "*"));
            this._lstBillTypes.Add(new Bill("USD", 50.0, "*", "*", "*", "*"));
            this._lstBillTypes.Add(new Bill("USD", 100.0, "*", "*", "*", "*"));
            label_24:
            for (int index = 0; index < this._lstBillTypes.Count; ++index)
                this._lstBillTypeEnables.Add(this._lstBillTypes[index].Value > 0.0);
        }

        private void buildBillValues()
        {
            int num = 0;
            for (int index1 = 0; index1 < this._lstBillTypes.Count; ++index1)
            {
                bool flag = false;
                for (int index2 = 0; index2 < num; ++index2)
                {
                    if (
                        this._lstBillTypes[index1].Value == this._lstBillValues[index2].Value
                        && this._lstBillTypes[index1].Country == this._lstBillValues[index2].Country
                    )
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    this._lstBillValues.Add(
                        new Bill(
                            this._lstBillTypes[index1].Country,
                            this._lstBillTypes[index1].Value,
                            "*",
                            "*",
                            "*",
                            "*"
                        )
                    );
                    ++num;
                    this._lstBillValueEnables.Add(this._lstBillTypes[index1].Value > 0.0);
                }
            }
        }

        private Bill parseBillData(byte[] reply, int extDataIndex, bool forBillTable)
        {
            Bill billData = new Bill();
            if (reply.Length != 30)
                return billData;
            UTF8Encoding utF8Encoding = new UTF8Encoding();
            int num1 = (int)reply[extDataIndex];
            try
            {
                billData.Country = utF8Encoding.GetString(reply, extDataIndex + 1, 3);
                if (billData.Country == "")
                    billData.Country = "***";
                string s = utF8Encoding.GetString(reply, extDataIndex + 4, 3);
                billData.Value = double.Parse(s);
                int num2 = (int)reply[extDataIndex + 7];
                int num3 = int.Parse(utF8Encoding.GetString(reply, extDataIndex + 8, 2));
                if (num2 == 43)
                {
                    for (int index = 1; index <= num3; ++index)
                        billData.Value *= 10.0;
                }
                else
                {
                    for (int index = 1; index <= num3; ++index)
                        billData.Value /= 10.0;
                }
                this._docTypeWasSetOnEscrow = this.DeviceState == State.Escrow;
            }
            catch (FormatException ex)
            {
                billData.Value = 0.0;
                if (this.DeviceState == State.Stacked)
                    this._docType = DocumentType.NoValue;
                this._docTypeWasSetOnEscrow = false;
            }
            if (!forBillTable && !this.Connected)
                this._billBeforeConnected = true;
            if (!forBillTable)
                this._docType = billData.Value <= 0.0 ? DocumentType.NoValue : DocumentType.Bill;
            else if (!this._billBeforeConnected)
                this._docType = DocumentType.NoValue;
            this._docTypeWasSetOnEscrow = this._deviceState == State.Escrow;
            switch (reply[extDataIndex + 10])
            {
                case 0:
                    this._escrowOrientation = Orientation.RightUp;
                    break;
                case 1:
                    this._escrowOrientation = Orientation.RightDown;
                    break;
                case 2:
                    this._escrowOrientation = Orientation.LeftUp;
                    break;
                case 3:
                    this._escrowOrientation = Orientation.LeftDown;
                    break;
            }
            Encoding ascii = Encoding.ASCII;
            billData.Type =
                reply[extDataIndex + 11] != (byte)0
                    ? ascii.GetString(reply, extDataIndex + 11, 1)
                    : "*";
            billData.Series =
                reply[extDataIndex + 12] != (byte)0
                    ? ascii.GetString(reply, extDataIndex + 12, 1)
                    : "*";
            billData.Compatibility =
                reply[extDataIndex + 13] != (byte)0
                    ? ascii.GetString(reply, extDataIndex + 13, 1)
                    : "*";
            billData.Version =
                reply[extDataIndex + 14] != (byte)0
                    ? ascii.GetString(reply, extDataIndex + 14, 1)
                    : "*";
            switch (reply[extDataIndex + 15])
            {
                case 0:
                    this._banknoteClassification = BanknoteClassification.Disabled;
                    break;
                case 1:
                    this._banknoteClassification = BanknoteClassification.UnidentifiedBanknote;
                    break;
                case 2:
                    this._banknoteClassification = BanknoteClassification.SuspectedCounterfeit;
                    break;
                case 3:
                    this._banknoteClassification = BanknoteClassification.SuspectedZeroValueNote;
                    break;
                case 4:
                    this._banknoteClassification = BanknoteClassification.GenuineBanknote;
                    break;
            }
            return billData;
        }

        internal void processReply(byte[] reply)
        {
            if (reply.Length < 5 || reply.Length != (int)reply[1])
                return;
            int num = (int)reply[2];
            if ((num & 112) == 32)
                this.processStandardOmnibusReply(reply);
            if ((num & 112) == 80)
            {
                this._deviceState = State.DownloadRestart;
                this._capFlashDownload = true;
                if (this._raiseDownloadRestartEvent)
                    this.RaiseDownloadRestartEvent();
            }
            if ((num & 112) == 112)
            {
                switch (reply[3])
                {
                    case 1:
                        this.processExtendedOmnibusBarCodeReply(reply);
                        break;
                    case 2:
                        this.processExtendedOmnibusExpandedNoteReply(reply);
                        if (
                            this.DeviceState == State.Escrow
                            || this.DeviceState == State.Stacked && !this._docTypeWasSetOnEscrow
                        )
                        {
                            DocumentType docType = this._docType;
                            Bill billData = this.parseBillData(reply, 10, false);
                            if (billData.Value != 0.0)
                                this._bill = billData;
                            else if (docType != DocumentType.None)
                                this._docType = docType;
                            if (this.CapOrientationExt)
                            {
                                if (this.OrientationCtlExt == OrientationControl.OneWay)
                                {
                                    if (this.EscrowOrientation != Orientation.RightUp)
                                    {
                                        this._raiseEscrowEvent = false;
                                        this.EscrowReturn();
                                        break;
                                    }
                                    break;
                                }
                                if (
                                    this.OrientationCtlExt == OrientationControl.TwoWay
                                    && this.EscrowOrientation != Orientation.RightUp
                                    && this.EscrowOrientation != Orientation.LeftUp
                                )
                                {
                                    this._raiseEscrowEvent = false;
                                    this.EscrowReturn();
                                    break;
                                }
                                break;
                            }
                            break;
                        }
                        break;
                    case 4:
                        this.processExtendedOmnibusExpandedCouponReply(reply);
                        break;
                    case 11:
                        this.processData4(reply[8]);
                        this.processData0(reply[4], reply[5]);
                        this.processData1(reply[5]);
                        this.processData2(reply[6]);
                        this.processData3(reply[7]);
                        this.processData5(reply[9]);
                        this.raiseEvents();
                        if (reply.Length == 13 && reply[10] == (byte)127)
                        {
                            this.RaiseNoteRetrievedEvent();
                            break;
                        }
                        break;
                    case 16:
                        this.processData4(reply[8]);
                        this.processData0(reply[4], reply[5]);
                        this.processData1(reply[5]);
                        this.processData2(reply[6]);
                        this.processData3(reply[7]);
                        this.processData5(reply[9]);
                        if (reply.Length == 13 && (reply[10] == (byte)17 || reply[10] == (byte)16))
                        {
                            this.RaiseCashboxCleanlinessEvent(reply[10]);
                            break;
                        }
                        break;
                    case 28:
                        this.processData4(reply[8]);
                        this.processData0(reply[4], reply[5]);
                        this.processData1(reply[5]);
                        this.processData2(reply[6]);
                        this.processData3(reply[7]);
                        this.processData5(reply[9]);
                        break;
                    case 29:
                        this.processData4(reply[8]);
                        this.processData0(reply[4], reply[5]);
                        this.processData1(reply[5]);
                        this.processData2(reply[6]);
                        this.processData3(reply[7]);
                        this.processData5(reply[9]);
                        if (reply.Length == 13 && (reply[10] == (byte)17 || reply[10] == (byte)16))
                        {
                            this.RaiseClearAuditEvent(reply[10] == (byte)17);
                            break;
                        }
                        break;
                }
                this.raiseEvents();
            }
            if (
                this.DeviceState == State.Escrow
                && this._raiseEscrowEvent
                && this.Connected
                && (this.AutoStack || !this.EnableAcceptance)
            )
            {
                this._raiseEscrowEvent = false;
                if (this.EnableAcceptance)
                    this.EscrowStack();
                else
                    this.EscrowReturn();
            }
            if (this.DeviceState == State.Escrow || this.DeviceState == State.Stacking)
                return;
            this._docTypeWasSetOnEscrow = false;
        }

        internal void processExtendedOmnibusBarCodeReply(byte[] reply)
        {
            if (reply.Length != 40)
                return;
            this.processData4(reply[8]);
            this.processData0(reply[4], reply[5]);
            this.processData1(reply[5]);
            this.processData2(reply[6]);
            this.processData3(reply[7]);
            this.processData5(reply[9]);
            if (this.DeviceState != State.Escrow)
                return;
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 10; index < 38 && reply[index] != (byte)40; ++index)
                stringBuilder.Append((char)reply[index]);
            this._barCode = stringBuilder.ToString();
            this._docType = DocumentType.Barcode;
        }

        internal void processExtendedOmnibusExpandedNoteReply(byte[] reply)
        {
            if (reply.Length != 30)
                return;
            this.processData4(reply[8]);
            this.processData0(reply[4], reply[5]);
            this.processData1(reply[5]);
            this.processData2(reply[6]);
            this.processData3(reply[7]);
            this.processData5(reply[9]);
        }

        internal void processExtendedOmnibusExpandedCouponReply(byte[] reply)
        {
            if (reply.Length != 18)
                return;
            this.processData4(reply[8]);
            this.processData0(reply[4], reply[5]);
            this.processData1(reply[5]);
            this.processData2(reply[6]);
            this.processData3(reply[7]);
            this.processData5(reply[9]);
            if (
                this.DeviceState != State.Escrow
                && (this.DeviceState != State.Stacked || this._docTypeWasSetOnEscrow)
            )
                return;
            int num1 =
                (((int)reply[10] & 15) << 12)
                + (((int)reply[11] & 15) << 8)
                + (((int)reply[12] & 15) << 4)
                + ((int)reply[13] & 15);
            double num2 = Acceptor.COUPON_VALUES[num1 & 7];
            this._coupon = new Coupon((num1 & 65528) >> 3, num2);
            this._docType = DocumentType.Coupon;
            this._docTypeWasSetOnEscrow = this.DeviceState == State.Escrow;
        }

        internal void processStandardOmnibusReply(byte[] reply)
        {
            if (reply.Length != 11)
                return;
            this.processData4(reply[7]);
            this.processData0(reply[3], reply[4]);
            this.processData1(reply[4]);
            this.processData2(reply[5]);
            this.processData3(reply[6]);
            this.processData5(reply[8]);
            this.raiseEvents();
        }

        private void processData0(byte data0, byte data1)
        {
            if (data0 == (byte)0)
            {
                this._deviceState = State.Idling;
            }
            else
            {
                if (
                    ((int)data0 & 81) == 1
                    && ((int)data1 & 2) == 0
                    && this.DeviceState != State.Calibrating
                    && this.DeviceState != State.CalibrateStart
                )
                    this._deviceState = State.Idling;
                if (((int)data0 & 2) != 0)
                {
                    if (!this.EnableAcceptance)
                        this._bDisabledWhileAccpeting = true;
                    if (
                        this.DeviceState != State.Calibrating
                        && this.DeviceState != State.CalibrateStart
                    )
                        this._deviceState = State.Accepting;
                }
                if (((int)data0 & 4) != 0)
                    this._deviceState = State.Escrow;
                else if (!this._raiseEscrowEvent)
                    this._raiseEscrowEvent = true;
                if (((int)data0 & 8) != 0)
                    this._deviceState = State.Stacking;
                if (((int)data0 & 16) != 0 && this._deviceState != State.Stacked)
                {
                    this._deviceState = State.Stacked;
                    this._raiseStackedEvent = true;
                }
                if (((int)data0 & 32) != 0)
                    this._deviceState = State.Returning;
                if (((int)data0 & 64) == 0 || this._deviceState == State.Returned)
                    return;
                this._deviceState = State.Returned;
                this._bill = new Bill();
                this._docType = DocumentType.NoValue;
                this._raiseReturnedEvent = true;
            }
        }

        private void processData1(byte data1)
        {
            if (((int)data1 & 1) != 0)
            {
                this._cheated = true;
                this._raiseCheatedEvent = true;
            }
            else
                this._cheated = false;
            if (((int)data1 & 2) != 0 && this._deviceState != State.Rejected)
            {
                this._deviceState = State.Rejected;
                this._bill = new Bill();
                this._docType = DocumentType.NoValue;
                this._raiseRejectedEvent = true;
            }
            if (((int)data1 & 4) != 0)
            {
                if (this.DeviceState != State.Jammed)
                {
                    this._bill = new Bill();
                    this._docType = DocumentType.NoValue;
                }
                this._deviceState = State.Jammed;
                this._isDeviceJammed = true;
                this._raiseJamClearedEvent = true;
            }
            else
            {
                this._isDeviceJammed = false;
                this._raiseJamDetectedEvent = true;
            }
            if (((int)data1 & 8) != 0)
            {
                this._cashBoxFull = true;
                this._raiseStackerFullClearedEvent = true;
            }
            else
            {
                this._cashBoxFull = false;
                this._raiseStackerFullEvent = true;
            }
            this._cashBoxAttached = ((uint)data1 & 16U) > 0U;
            if (!this._cashBoxAttached)
            {
                this._docType = DocumentType.NoValue;
                this._raiseCashBoxAttachedEvent = true;
            }
            else
                this._raiseCashBoxRemovedEvent = true;
            if (((int)data1 & 32) != 0)
            {
                this._devicePaused = true;
                this._raisePauseClearedEvent = true;
            }
            else
            {
                this._devicePaused = false;
                this._raisePauseDetectedEvent = true;
            }
            if (((int)data1 & 64) != 0)
            {
                this._deviceState = State.Calibrating;
                if (!this._raiseCalibrateProgressEvent)
                    return;
                this.RaiseCalibrateProgressEvent();
            }
            else
            {
                if (this.DeviceState != State.Calibrating)
                    return;
                this._raiseCalibrateFinishEvent = true;
                this._deviceState = State.Idling;
            }
        }

        private void processData2(byte data2)
        {
            if (!this._expandedNoteReporting)
            {
                int num = ((int)data2 & 56) >> 3;
                if (num > 0)
                {
                    if (
                        this.DeviceState == State.Escrow
                        || this.DeviceState == State.Stacked && !this._docTypeWasSetOnEscrow
                    )
                    {
                        if (this.BillTypes.Length == 0)
                            this.setUpBillTable();
                        this._bill = this._lstBillTypes[num - 1];
                        this._docType = DocumentType.Bill;
                        this._docTypeWasSetOnEscrow = this.DeviceState == State.Escrow;
                    }
                }
                else
                {
                    if (this.DeviceState == State.Stacked || this.DeviceState == State.Escrow)
                    {
                        this._bill = new Bill();
                        this._docType = DocumentType.NoValue;
                    }
                    this._docTypeWasSetOnEscrow = false;
                }
            }
            else if (
                this.DeviceState == State.Stacked
                && this.DocType == DocumentType.Bill
                && (this.Bill == null || this.Bill.Value == 0.0)
                && this.Bill == null
            )
                this._bill = new Bill();
            if (((int)data2 & 1) != 0)
            {
                this._isPoweredUp = true;
                this._raiseEscrowEvent = this._raisePowerUpEvent;
            }
            else
            {
                this._raisePowerUpEvent = true;
                if (!this._isVeryFirstPoll)
                    this._isPoweredUp = false;
            }
            if (((int)data2 & 2) != 0)
                this.RaiseInvalidCommandEvent();
            if (((int)data2 & 4) != 0)
            {
                this._deviceState = State.Failed;
                this._isInFailedState = true;
            }
            else
            {
                this._isInFailedState = false;
                this._raiseFailureDetectedEvent = true;
            }
        }

        private void processData3(byte data3)
        {
            if (((int)data3 & 1) != 0)
            {
                this._deviceState = State.Stalled;
                this._raiseStallClearedEvent = true;
            }
            else
                this._raiseStallDetectedEvent = true;
            if (((int)data3 & 2) != 0)
            {
                this._deviceState = State.DownloadRestart;
                if (this._raiseDownloadRestartEvent)
                    this.RaiseDownloadRestartEvent();
            }
            if (((int)data3 & 8) != 0)
                this._capBarCodesExt = true;
            if (((int)data3 & 16) != 0)
                this._isQueryDeviceCapabilitiesSupported = true;
            else
                this._isQueryDeviceCapabilitiesSupported = false;
        }

        private void processData4(byte data4)
        {
            if (this._capabilitiesProcessed)
                return;
            if (data4 != (byte)0)
                this._capabilitiesProcessed = true;
            this._deviceModel = (int)data4 & (int)sbyte.MaxValue;
            char deviceModel = (char)this._deviceModel;
            this._capApplicationPN = deviceModel == 'T' || deviceModel == 'U';
            this._capAssetNumber = deviceModel == 'T' || deviceModel == 'U';
            this._capAudit = deviceModel == 'T' || deviceModel == 'U';
            this._capBarCodes =
                deviceModel == 'T'
                || deviceModel == 'U'
                || this._deviceModel == 15
                || this._deviceModel == 23;
            this._capBookmark = true;
            this._capBootPN = deviceModel == 'T' || deviceModel == 'U';
            this._capCalibrate = true;
            this._capCashBoxTotal =
                deviceModel == 'A'
                || deviceModel == 'B'
                || deviceModel == 'C'
                || deviceModel == 'D'
                || deviceModel == 'G'
                || deviceModel == 'M'
                || deviceModel == 'P'
                || deviceModel == 'W'
                || deviceModel == 'X';
            this._capCouponExt = deviceModel == 'P' || deviceModel == 'X';
            this._capDevicePaused =
                deviceModel == 'P' || deviceModel == 'X' || this._deviceModel == 31;
            this._capDeviceSoftReset =
                deviceModel == 'A'
                || deviceModel == 'B'
                || deviceModel == 'C'
                || deviceModel == 'D'
                || deviceModel == 'G'
                || deviceModel == 'M'
                || deviceModel == 'P'
                || deviceModel == 'T'
                || deviceModel == 'U'
                || deviceModel == 'W'
                || deviceModel == 'X'
                || this._deviceModel == 31;
            this._capDeviceType = deviceModel == 'T' || deviceModel == 'U';
            this._capDeviceResets =
                deviceModel == 'A'
                || deviceModel == 'B'
                || deviceModel == 'C'
                || deviceModel == 'D'
                || deviceModel == 'G'
                || deviceModel == 'M'
                || deviceModel == 'P'
                || deviceModel == 'T'
                || deviceModel == 'U'
                || deviceModel == 'W'
                || deviceModel == 'X';
            this._capDeviceSerialNumber = deviceModel == 'T' || deviceModel == 'U';
            this._capFlashDownload = true;
            this._capEscrowTimeout = deviceModel == 'T' || deviceModel == 'U';
            this._capNoPush =
                deviceModel == 'P'
                || deviceModel == 'X'
                || this._deviceModel == 31
                || this._deviceModel == 23;
            this._capVariantPN = deviceModel == 'T' || deviceModel == 'U';
            this._expandedNoteReporting = deviceModel == 'T' || deviceModel == 'U';
        }

        private void processData5(byte data5)
        {
            if (
                this.DeviceModel < 23
                || this.DeviceModel == 30
                || this.DeviceModel == 31
                || this.DeviceModel == 74
                || this.DeviceModel == 84
                || this.DeviceModel == 85
            )
                this._deviceRevision = (int)data5 & (int)sbyte.MaxValue;
            else
                this._deviceRevision = ((int)data5 & 15) + ((int)data5 & 112) / 16 * 10;
        }

        private byte getPupExtValueCode(PupExt pupExt)
        {
            switch (pupExt)
            {
                case PupExt.OutOfService:
                    return 0;
                case PupExt.Return:
                    return 2;
                case PupExt.StackNoCredit:
                    return 1;
                case PupExt.Stack:
                    return 3;
                case PupExt.WaitNoCredit:
                    return 4;
                case PupExt.Wait:
                    return 5;
                default:
                    return 0;
            }
        }

        private void openLogFile()
        {
            string path = string.Format(
                "{0}\\MPOST_Log_{1}.txt",
                (object)this.DebugLogPath,
                (object)this.DevicePortName
            );
            if (this._logWriter != null)
                this._logWriter.Close();
            this._logWriter = File.AppendText(path);
            this._logWriter.AutoFlush = true;
            this.log(
                "--------------------------------------------------------------------------------"
            );
            this.log(
                string.Format(
                    "M/POST version {0} log opened {1}.",
                    (object)this.APIVersion,
                    (object)DateTime.Now.ToShortDateString()
                )
            );
        }

        private void closeLogFile()
        {
            _transport.flushIdenticalCommands();
            this.log("Log closed.");
            lock (this._objLogLock)
            {
                if (this._logWriter != null)
                    this._logWriter.Close();
                this._logWriter = (StreamWriter)null;
            }
        }

        internal void log(string message) => this.log(message, DateTime.Now);

        internal void log(string message, DateTime time)
        {
            lock (this._objLogLock)
            {
                if (this._logWriter == null)
                    return;
                try
                {
                    this._logWriter.Write(
                        string.Format(
                            "{0}.{1}: ",
                            (object)
                                time.ToString(
                                    "T",
                                    (IFormatProvider)DateTimeFormatInfo.InvariantInfo
                                ),
                            (object)time.Millisecond.ToString().PadLeft(3, '0')
                        )
                    );
                    this._logWriter.WriteLine(message);
                }
                catch { }
            }
        }

        internal void log(Exception ex) =>
            this.log(
                string.Format(
                    "{0} : {1}",
                    (object)ex.GetType().ToString().ToUpper(),
                    (object)ex.Message
                )
            );

        private void setupDownload(string fileName)
        {
            byte[] numArray1 = new byte[0];
            this._suppressStandardPoll = true;
            this._stopFlashDownloadThread = false;
            this._raiseDownloadRestartEvent = false;
            this._currentPacket = 0;
            try
            {
                DateTime now;
                if (!this._autoDownloadRestart)
                {
                    this._bUseFastDownload = false;
                    _transport.SetProtocolLevel(eProtocolLevel.STANDARD);
                }
                else
                {
                    Thread.Sleep(1000);
                    EBDS_SerialPort transport = _transport;
                    now = DateTime.Now;
                    long ticks = now.Ticks;
                    transport.resetPort(ticks);
                }
                Thread.Sleep(1000);
                this._cmdQueue.Clear();
                this._replyQueue.Clear();
                byte[] payload1 = new byte[4] { (byte)16, (byte)0, (byte)0, (byte)0 };
                now = DateTime.Now;
                long ticks1 = now.Ticks;
                while (!this._stopFlashDownloadThread)
                {
                    now = DateTime.Now;
                    if (
                        new TimeSpan(now.Ticks - ticks1).TotalMilliseconds
                        <= (double)Acceptor.COMMUNICATION_DISCONNECT_TIMEOUT
                    )
                    {
                        byte[] numArray2 = this.sendSynchronousCommand(
                            payload1,
                            "Poll before download"
                        );
                        if (numArray2.Length != 0)
                        {
                            if (((int)numArray2[2] & 112) == 32)
                            {
                                byte[] payload2 = new byte[4]
                                {
                                    (byte)80,
                                    (byte)0,
                                    (byte)0,
                                    (byte)0,
                                };
                                now = DateTime.Now;
                                long ticks2 = now.Ticks;
                                while (!this._stopFlashDownloadThread)
                                {
                                    now = DateTime.Now;
                                    if (
                                        new TimeSpan(now.Ticks - ticks2).TotalMilliseconds
                                        <= (double)Acceptor.COMMUNICATION_DISCONNECT_TIMEOUT
                                    )
                                    {
                                        byte[] numArray3 = this.sendSynchronousCommand(
                                            payload2,
                                            "Download command"
                                        );
                                        if (numArray3.Length != 11 || ((int)numArray3[6] & 7) != 2)
                                        {
                                            if (
                                                numArray3.Length == 9
                                                && ((int)numArray3[2] & 240) == 80
                                            )
                                            {
                                                this._currentPacket =
                                                    (((int)numArray3[3] & 15) << 12)
                                                        + (((int)numArray3[4] & 15) << 8)
                                                        + (((int)numArray3[5] & 15) << 4)
                                                        + (((int)numArray3[6] & 15) + 1)
                                                    & (int)ushort.MaxValue;
                                                goto label_18;
                                            }
                                        }
                                        else
                                            goto label_18;
                                    }
                                    else
                                        break;
                                }
                                throw new Exception(
                                    "Download Setup: Sending Start Download Command Failed"
                                );
                            }
                            this._currentPacket =
                                (((int)numArray2[3] & 15) << 12)
                                    + (((int)numArray2[4] & 15) << 8)
                                    + (((int)numArray2[5] & 15) << 4)
                                    + (((int)numArray2[6] & 15) + 1)
                                & (int)ushort.MaxValue;
                            label_18:
                            if (
                                _transport.ProtocolLevel == eProtocolLevel.STANDARD
                                && !this._autoDownloadRestart
                            )
                            {
                                this._dlFileParser = new DownloadFileParser(fileName, true);
                                for (int index = 0; index < 5 || !this._bUseFastDownload; ++index)
                                {
                                    this._bUseFastDownload = this.SetupFastDownload();
                                    if (!this._bUseFastDownload)
                                        Thread.Sleep(TimeSpan.FromSeconds(0.5));
                                }
                                if (!this._bUseFastDownload)
                                {
                                    this._dlFileParser = new DownloadFileParser(fileName, false);
                                    numArray1 = this.sendSynchronousCommand(
                                        new byte[2] { (byte)80, (byte)1 },
                                        "Set Baud command"
                                    );
                                    this.log("Download start - 9600 bps");
                                }
                            }
                            else if (
                                _transport.ProtocolLevel == eProtocolLevel.STANDARD
                                && this._autoDownloadRestart
                            )
                            {
                                _transport.SetProtocolLevel(eProtocolLevel.STANDARD);
                                numArray1 = this.sendSynchronousCommand(
                                    new byte[2] { (byte)80, (byte)1 },
                                    "Set Baud command"
                                );
                            }
                            else if (
                                _transport.ProtocolLevel == eProtocolLevel.FAST_DOWNLOAD_SERIAL
                                && this._autoDownloadRestart
                            )
                                this._bUseFastDownload = this.SetupFastDownload();
                            if (!this._autoDownloadRestart)
                                this.RaiseDownloadStartEvent(this._dlFileParser.FinalPacketNumber);
                            this._autoDownloadRestart = false;
                            this._deviceState = State.DownloadStart;
                            return;
                        }
                    }
                    else
                        break;
                }
                throw new Exception("Download Setup: Sending Initial Poll Failed");
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex,ex.Message);
                this.log(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private bool SetupFastDownload()
        {
            byte[] numArray = this.sendSynchronousCommand(
                new byte[2] { (byte)80, (byte)3 },
                "Set Baud command"
            );
            if (numArray.Length != 0)
            {
                if (numArray.Length == 6 && numArray[3] == (byte)3)
                {
                    this._bUseFastDownload = true;
                    this.log("Flash Download - Set Baud ACK - 38400 bps");
                    _transport.SetProtocolLevel(eProtocolLevel.FAST_DOWNLOAD_SERIAL);
                }
                else
                {
                    this.log("Flash Download - Set Baud NAK");
                    if (numArray.Length == 9)
                    {
                        this._currentPacket =
                            (((int)numArray[3] & 15) << 12)
                                + (((int)numArray[4] & 15) << 8)
                                + (((int)numArray[5] & 15) << 4)
                                + (((int)numArray[6] & 15) + 1)
                            & (int)ushort.MaxValue;
                        ++this._currentPacket;
                    }
                }
            }
            else
                this.log("Flash Download - Set Baud NULL Response");
            return this._bUseFastDownload;
        }

        private void RaiseCalibrateFinishedEvent()
        {
            this.log("EVNT: CalibrateFinish");
            this._raiseCalibrateFinishEvent = false;
            if (this.OnCalibrateFinish == null)
                return;
            foreach (
                CalibrateFinishEventHandler invocation in this.OnCalibrateFinish.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseCalibrateProgressEvent()
        {
            this.log("EVNT: CalibrateProgress");
            this._raiseCalibrateProgressEvent = false;
            if (this.OnCalibrateProgress == null)
                return;
            foreach (
                CalibrateProgressEventHandler invocation in this.OnCalibrateProgress.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseCalibrateStartEvent()
        {
            this.log("EVNT: CalibrateStart");
            if (this.OnCalibrateStart == null)
                return;
            foreach (
                CalibrateStartEventHandler invocation in this.OnCalibrateStart.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseCashBoxAttachedEvent()
        {
            this.log("EVNT: Cash Box Attached");
            this._raiseCashBoxAttachedEvent = false;
            if (this.OnCashBoxAttached == null)
                return;
            foreach (
                CashBoxAttachedEventHandler invocation in this.OnCashBoxAttached.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseCashboxCleanlinessEvent(byte eventValue)
        {
            CashBoxCleanlinessEnum boxCleanlinessEnum;
            switch (eventValue)
            {
                case 16:
                    boxCleanlinessEnum = CashBoxCleanlinessEnum.CleaningRequired;
                    break;
                case 17:
                    boxCleanlinessEnum = CashBoxCleanlinessEnum.CleaningRecommended;
                    break;
                default:
                    return;
            }
            this.log(
                string.Format(
                    "EVNT: Cashbox Cleanliness - {0}",
                    (object)boxCleanlinessEnum.ToString()
                )
            );
            if (this.OnCashBoxCleanlinessDetected == null)
                return;
            foreach (
                CashBoxCleanlinessEventHandler invocation in this.OnCashBoxCleanlinessDetected.GetInvocationList()
            )
                invocation.Invoke(
                    (object)this,
                    new CashBoxCleanlinessEventArgs(boxCleanlinessEnum)
                );
        }

        private void RaiseCashBoxRemovedEvent()
        {
            this.log("EVNT: Cash Box Removed");
            this._raiseCashBoxRemovedEvent = false;
            if (this.OnCashBoxRemoved == null)
                return;
            foreach (
                CashBoxRemovedEventHandler invocation in this.OnCashBoxRemoved.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseCheatedEvent()
        {
            this.log("EVNT: Cheated");
            this._raiseCheatedEvent = false;
            if (this.OnCheated == null)
                return;
            foreach (CheatedEventHandler invocation in this.OnCheated.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseClearAuditEvent(bool success)
        {
            this.log(string.Format("EVNT: Clear Audit Complete, Success = {0}", (object)success));
            if (this.OnClearAuditComplete == null)
                return;
            foreach (
                ClearAuditEventHandler invocation in this.OnClearAuditComplete.GetInvocationList()
            )
                invocation.Invoke((object)this, new ClearAuditEventArgs(success));
        }

        internal void RaiseConnectedEvent()
        {
            this.log("EVNT: Connected");
            this._raiseConnectedEvent = false;
            this._raiseDisconnectedEvent = true;
            if (this.OnConnected == null)
                return;
            foreach (ConnectedEventHandler invocation in this.OnConnected.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        internal void RaiseDisconnectedEvent()
        {
            this.log("EVNT: Disconnected");
            this._raiseDisconnectedEvent = false;
            this._raiseFailureDetectedEvent = true;
            this._raiseJamDetectedEvent = true;
            this._raiseStackerFullEvent = true;
            this._raiseEscrowEvent = true;
            this._raiseCashBoxRemovedEvent = true;
            if (this.OnDisconnected == null)
                return;
            foreach (DisconnectedEventHandler invocation in this.OnDisconnected.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseDownloadFinishEvent(bool success)
        {
            this.log(string.Format("EVNT: Download Finish, Success = {0}", (object)success));
            if (this.OnDownloadFinish != null)
            {
                foreach (
                    DownloadFinishEventHandler invocation in this.OnDownloadFinish.GetInvocationList()
                )
                    invocation.Invoke((object)this, new AcceptorDownloadFinishEventArgs(success));
            }
            this._bDownloadSuccess = success;
            this._suppressStandardPoll = false;
            this._deviceState = State.Idling;
        }

        private void RaiseDownloadProgressEvent(int sectorCount)
        {
            this.log(string.Format("EVNT: Download Progress, Sector = {0}", (object)sectorCount));
            if (this.OnDownloadProgress == null)
                return;
            foreach (
                DownloadProgressEventHandler invocation in this.OnDownloadProgress.GetInvocationList()
            )
                invocation.Invoke((object)this, new AcceptorDownloadEventArgs(sectorCount));
        }

        private void RaiseDownloadRestartEvent()
        {
            this.log("EVNT: Download Restart");
            this._raiseDownloadRestartEvent = false;
            if (this.OnDownloadRestart == null)
                return;
            foreach (
                DownloadRestartEventHandler invocation in this.OnDownloadRestart.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseDownloadStartEvent(int sectorCount)
        {
            this.log(
                string.Format("EVNT: Download Start, Total Sectors = {0}", (object)sectorCount)
            );
            if (this.OnDownloadStart == null)
                return;
            foreach (
                DownloadStartEventHandler invocation in this.OnDownloadStart.GetInvocationList()
            )
                invocation.Invoke((object)this, new AcceptorDownloadEventArgs(sectorCount));
        }

        internal void RaiseErrorWhileSendingMessageEvent(Message message)
        {
            if (this.DeviceState == State.Disconnected)
                return;
            this.log(string.Format("EVNT: Send Message Error in {0}", (object)message.Description));
            if (this.OnSendMessageFailure == null)
                return;
            foreach (
                ErrorOnSendMessageEventHandler invocation in this.OnSendMessageFailure.GetInvocationList()
            )
                invocation.Invoke((object)this, new AcceptorMessageEventArgs(message));
        }

        private void RaiseEscrowEvent()
        {
            this.log(string.Format("EVNT: Escrow: {0}", (object)this._docType));
            this._raiseEscrowEvent = false;
            this._raisePUPEscrowEvent = false;
            if (this.OnEscrow == null)
                return;
            foreach (EscrowEventHandler invocation in this.OnEscrow.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseFailureClearedEvent()
        {
            this.log("EVNT: Device Failure Cleared");
            this._raiseFailureClearedEvent = false;
            this._raiseFailureDetectedEvent = true;
            if (this.OnFailureCleared == null)
                return;
            foreach (
                FailureClearedEventHandler invocation in this.OnFailureCleared.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseFailureDetectedEvent()
        {
            this.log("EVNT: Device Failure Detected");
            this._raiseFailureDetectedEvent = false;
            this._raiseFailureClearedEvent = true;
            if (this.OnFailureDetected == null)
                return;
            foreach (
                FailureDetectedEventHandler invocation in this.OnFailureDetected.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        internal void RaiseInvalidCommandEvent()
        {
            this.log("EVNT: Invalid Command");
            if (this.OnInvalidCommand == null)
                return;
            this.OnInvalidCommand((object)this, new EventArgs());
        }

        private void RaiseJamClearedEvent()
        {
            this.log("EVNT: Jam Cleared");
            this._raiseJamClearedEvent = false;
            if (this.OnJamCleared == null)
                return;
            foreach (JamClearedEventHandler invocation in this.OnJamCleared.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseJamDetectedEvent()
        {
            this.log("EVNT: Jam Detected");
            this._raiseJamDetectedEvent = false;
            if (this.OnJamDetected == null)
                return;
            foreach (JamDetectedEventHandler invocation in this.OnJamDetected.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseNoteRetrievedEvent()
        {
            this.log(string.Format("EVNT: Note Retrieved"));
            if (this.OnNoteRetrieved == null)
                return;
            foreach (
                NoteRetrievedEventHandler invocation in this.OnNoteRetrieved.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaisePauseClearedEvent()
        {
            this.log(string.Format("EVNT: Pause Cleared"));
            this._raisePauseClearedEvent = false;
            if (this.OnPauseCleared == null)
                return;
            foreach (PauseClearedEventHandler invocation in this.OnPauseCleared.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaisePauseDetectedEvent()
        {
            this.log(string.Format("EVNT: Pause Detected"));
            this._raisePauseDetectedEvent = false;
            if (this.OnPauseDetected == null)
                return;
            foreach (
                PauseDetectedEventHandler invocation in this.OnPauseDetected.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaisePowerUpCompleteEvent()
        {
            this.log("EVNT: Power Up Complete");
            this._raisePowerUpCompleteEvent = false;
            if (this.OnPowerUpComplete == null)
                return;
            foreach (
                PowerUpCompleteEventHandler invocation in this.OnPowerUpComplete.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaisePowerUpEvent()
        {
            this.log("EVNT: Power Up");
            this._raisePowerUpEvent = false;
            this._raisePUPEscrowEvent = true;
            this._raisePowerUpCompleteEvent = true;
            if (this.OnPowerUp != null)
            {
                foreach (PowerUpEventHandler invocation in this.OnPowerUp.GetInvocationList())
                    invocation.Invoke((object)this, new EventArgs());
            }
            if (!this._capNoteRetrieved)
                return;
            byte[] payload = this.constructOmnibusCommand(6, (byte)112, 2);
            payload[1] = (byte)11;
            payload[5] = (byte)1;
            this.sendAsynchronousCommand(payload, "Enable Note Retrieved Event");
        }

        private void RaisePUPEscrowEvent()
        {
            this.log(string.Format("EVNT: PUP Escrow: {0}", (object)this._docType));
            this._raisePUPEscrowEvent = false;
            this._raiseEscrowEvent = false;
            if (this.OnPUPEscrow == null)
                return;
            foreach (PUPEscrowEventHandler invocation in this.OnPUPEscrow.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseRejectedEvent()
        {
            this.log("EVNT: Rejected");
            this._raiseRejectedEvent = false;
            if (this.OnRejected == null)
                return;
            foreach (RejectedEventHandler invocation in this.OnRejected.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseReturnedEvent()
        {
            this.log("EVNT: Returned");
            this._raiseReturnedEvent = false;
            if (this.OnReturned == null)
                return;
            foreach (ReturnedEventHandler invocation in this.OnReturned.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseStackedEvent()
        {
            this.log(string.Format("EVNT: Stacked"));
            this.log(string.Format("EVNT: StackedWithDocInfo: {0}", (object)this._docType));
            this._raiseStackedEvent = false;
            if (this.OnStacked != null)
            {
                foreach (StackedEventHandler invocation in this.OnStacked.GetInvocationList())
                    invocation.Invoke((object)this, new EventArgs());
            }
            if (this.OnStackedWithDocInfo == null)
                return;
            foreach (
                StackedWithDocInfoEventHandler invocation in this.OnStackedWithDocInfo.GetInvocationList()
            )
                invocation.Invoke(
                    (object)this,
                    new StackedEventArgs(this._docType, this.getDocument())
                );
        }

        private void RaiseStackerFullClearedEvent()
        {
            this.log("EVNT: Stacker Full Cleared");
            this._raiseStackerFullClearedEvent = false;
            if (this.OnStackerFullCleared == null)
                return;
            foreach (
                StackerFullClearedEventHandler invocation in this.OnStackerFullCleared.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseStackerFullEvent()
        {
            this.log(string.Format("EVNT: Stacker Full"));
            this._raiseStackerFullEvent = false;
            if (this.OnStackerFull == null)
                return;
            foreach (StackerFullEventHandler invocation in this.OnStackerFull.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseStallClearedEvent()
        {
            this.log("EVNT: Stall Cleared");
            this._raiseStallClearedEvent = false;
            if (this.OnStallCleared == null)
                return;
            foreach (StallClearedEventHandler invocation in this.OnStallCleared.GetInvocationList())
                invocation.Invoke((object)this, new EventArgs());
        }

        private void RaiseStallDetectedEvent()
        {
            this.log("EVNT: Stall Detected");
            this._raiseStallDetectedEvent = false;
            if (this.OnStallDetected == null)
                return;
            foreach (
                StallDetectedEventHandler invocation in this.OnStallDetected.GetInvocationList()
            )
                invocation.Invoke((object)this, new EventArgs());
        }

        private void THR_OpenThread()
        {
            byte[] numArray = new byte[0];
            int num = 0;
            while (!this._stopOpenThread)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    long ticks = now.Ticks;
                    while (num < 2)
                    {
                        Thread.Sleep(100);
                        if (this._stopOpenThread)
                        {
                            _transport.close();
                            return;
                        }
                        byte[] payload = new byte[4] { (byte)16, (byte)0, (byte)16, (byte)16 };
                        this._cmdQueue.Clear();
                        byte[] reply = this.sendSynchronousCommand(payload, "Initial Poll");
                        if (reply.Length != 0)
                        {
                            ++num;
                            this.processReply(reply);
                        }
                        now = DateTime.Now;
                        if (
                            new TimeSpan(now.Ticks - ticks).TotalMilliseconds
                            > (double)Acceptor.COMMUNICATION_DISCONNECT_TIMEOUT
                        )
                            throw new Exception(
                                "Timeout. Could not connect in the given amount of time."
                            );
                    }
                    this.queryDeviceCapabilities();
                    if (
                        this.DeviceState != State.DownloadRestart
                        && this.DeviceState != State.DownloadStart
                    )
                    {
                        this.setUpBillTable();
                        if (this._capNoteRetrieved)
                        {
                            byte[] payload = this.constructOmnibusCommand(6, (byte)112, 2);
                            payload[1] = (byte)11;
                            payload[5] = (byte)1;
                            this.sendAsynchronousCommand(payload, "Enable Note Retrieved Event");
                        }
                        this._connected = true;
                        this.RaiseConnectedEvent();
                        this._billBeforeConnected = false;
                        this.LogDeviceInfo();
                        break;
                    }
                    this._connected = true;
                    break;
                }
                catch (Exception ex)
                {
                    App.AppLogger.Error(ex,ex.Message);
                    this.log("Failed to Open Connection: " + ex.Message);
                    this.RaiseDisconnectedEvent();
                }
            }
        }

        private void LogDeviceInfo()
        {
            string str1 = string.Empty;
            string str2 = string.Empty;
            if (this._logWriter == null)
                return;
            try
            {
                while (string.IsNullOrEmpty(str1))
                {
                    if (this.CapApplicationPN)
                        str1 = this.ApplicationPN;
                }
                while (string.IsNullOrEmpty(str2) || str2.Equals(str1))
                {
                    if (this.CapVariantPN)
                        str2 = this.VariantPN;
                }
                this.log(
                    "SCN application version '" + str1 + "'. SCN bill variant '" + str2 + "'."
                );
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex,ex.Message);
                this.LogDeviceInfo();
            }
        }

        private void THR_FlashDownloadThread(object filepath)
        {
            string fileName = filepath as string;
            int num1 = 0;
            byte[] numArray = new byte[0];
            DateTime localTime = DateTime.UtcNow.ToLocalTime();
            try
            {
                this.log(
                    string.Format(
                        "Starting Flash Download at {0}",
                        (object)localTime.ToString("HH:mm:ss:fff")
                    )
                );
                this._autoDownloadRestart = false;
                this._bDownloadSuccess = true;
                this.setupDownload(fileName);
                while (!this._stopFlashDownloadThread)
                {
                    byte[] data = this._dlFileParser.GetData(this._currentPacket);
                    try
                    {
                        this._cmdQueue.Clear();
                        numArray = this.sendSynchronousCommandForDownload(
                            data,
                            string.Format("Download Packet: {0}", (object)this._currentPacket)
                        );
                    }
                    catch (Exception ex)
                    {
                        App.AppLogger.Error(ex,ex.Message);
                    }
                    if (numArray.Length == (this._bUseFastDownload ? 7 : 9))
                    {
                        int num2;
                        if (this._bUseFastDownload)
                        {
                            num2 = (int)numArray[3] + ((int)numArray[4] << 8);
                            if (num2 == (int)ushort.MaxValue)
                                num2 = -1;
                        }
                        else
                        {
                            num2 =
                                (((int)numArray[3] & 15) << 12)
                                    + (((int)numArray[4] & 15) << 8)
                                    + (((int)numArray[5] & 15) << 4)
                                    + ((int)numArray[6] & 15)
                                & (int)ushort.MaxValue;
                            if (num2 == (int)ushort.MaxValue)
                            {
                                num2 = -1;
                                Thread.Sleep(1000);
                            }
                        }
                        if (this._replyAcked)
                        {
                            if (num2 == this._currentPacket)
                            {
                                this._deviceState = State.Downloading;
                                this.RaiseDownloadProgressEvent(this._currentPacket);
                                num1 = 0;
                                ++this._currentPacket;
                            }
                            else
                            {
                                this.log(
                                    string.Format(
                                        "Possible Comm mixup - Perhaps a redundant message was left in queue. Download Packet {0} was ACK'd with last successful packet of {1}",
                                        (object)this._currentPacket,
                                        (object)num2
                                    )
                                );
                                ++num1;
                                if (num1 >= 5)
                                {
                                    num1 = 0;
                                    this.log("DownloadProgressReported: 5 Consecutive Mixups");
                                    this._autoDownloadRestart = true;
                                    this.setupDownload(fileName);
                                }
                            }
                        }
                        else
                        {
                            this.log(
                                string.Format(
                                    "Download Packet {0} was Nak'd. Last successful packet was {1}",
                                    (object)this._currentPacket,
                                    (object)num2
                                )
                            );
                            this._currentPacket = num2 + 1;
                            ++num1;
                            if (num1 >= 5)
                            {
                                num1 = 0;
                                this.log("DownloadProgressReported: 5 Consecutive NAKs");
                                this._autoDownloadRestart = true;
                                this.setupDownload(fileName);
                            }
                        }
                    }
                    else
                    {
                        ++num1;
                        if (num1 >= 5)
                        {
                            num1 = 0;
                            this.log("DownloadProgressReported: 5 Consecutive Nulls");
                            this._autoDownloadRestart = true;
                            this.setupDownload(fileName);
                        }
                    }
                    if (this._currentPacket >= this._dlFileParser.FinalPacketNumber)
                    {
                        DateTime dateTime = DateTime.UtcNow;
                        dateTime = dateTime.ToLocalTime();
                        this.log(
                            string.Format(
                                "Completed Flash Download after {0}",
                                (object)dateTime.Subtract(localTime).ToString()
                            )
                        );
                        _transport.SetProtocolLevel(eProtocolLevel.STANDARD);
                        Thread.Sleep(15000);
                        dateTime = DateTime.Now;
                        long ticks = dateTime.Ticks;
                        byte[] reply = new byte[0];
                        while (reply.Length == 0)
                        {
                            if (this._stopFlashDownloadThread)
                            {
                                this._deviceState = State.Idling;
                                this._suppressStandardPoll = false;
                                return;
                            }
                            byte[] payload = new byte[4] { (byte)16, (byte)0, (byte)16, (byte)0 };
                            this._cmdQueue.Clear();
                            reply = this.sendSynchronousCommand(payload, "Initial Poll");
                            dateTime = DateTime.Now;
                            if (
                                new TimeSpan(dateTime.Ticks - ticks).TotalMilliseconds
                                > (double)Acceptor.COMMUNICATION_DISCONNECT_TIMEOUT
                            )
                            {
                                if (this._raiseDisconnectedEvent)
                                    this.RaiseDisconnectedEvent();
                                dateTime = DateTime.Now;
                                ticks = dateTime.Ticks;
                            }
                        }
                        this._deviceState = State.Idling;
                        this._suppressStandardPoll = false;
                        this.processReply(reply);
                        this.queryDeviceCapabilities();
                        this.setUpBillTable();
                        this.RaiseDownloadFinishEvent(reply.Length == 11);
                        this._connected = true;
                        if (this.CapNoteRetrieved)
                        {
                            byte[] payload = this.constructOmnibusCommand(6, (byte)112, 2);
                            payload[1] = (byte)11;
                            payload[5] = (byte)1;
                            this.sendAsynchronousCommand(payload, "Enable Note Retrieved Event");
                        }
                        this.RaiseConnectedEvent();
                        return;
                    }
                }
                this.log("DownloadProgressReported: Cancelling Download Thread");
                this.RaiseDownloadFinishEvent(false);
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex,ex.Message);
                this.log(
                    string.Format(
                        "DownloadProgressReported: Download Failed - {0}",
                        (object)ex.Message
                    )
                );
                this.RaiseDownloadFinishEvent(false);
            }
        }

        public event CalibrateFinishEventHandler OnCalibrateFinish;

        public event CalibrateProgressEventHandler OnCalibrateProgress;

        public event CalibrateStartEventHandler OnCalibrateStart;

        public event CashBoxAttachedEventHandler OnCashBoxAttached;

        public event CashBoxCleanlinessEventHandler OnCashBoxCleanlinessDetected;

        public event CashBoxRemovedEventHandler OnCashBoxRemoved;

        public event CheatedEventHandler OnCheated;

        public event ClearAuditEventHandler OnClearAuditComplete;

        public event ConnectedEventHandler OnConnected;

        public event DisconnectedEventHandler OnDisconnected;

        public event DownloadFinishEventHandler OnDownloadFinish;

        public event DownloadProgressEventHandler OnDownloadProgress;

        public event DownloadRestartEventHandler OnDownloadRestart;

        public event DownloadStartEventHandler OnDownloadStart;

        public event ErrorOnSendMessageEventHandler OnSendMessageFailure;

        public event EscrowEventHandler OnEscrow;

        public event FailureClearedEventHandler OnFailureCleared;

        public event FailureDetectedEventHandler OnFailureDetected;

        public event InvalidCommandEventHandler OnInvalidCommand;

        public event JamClearedEventHandler OnJamCleared;

        public event JamDetectedEventHandler OnJamDetected;

        public event NoteRetrievedEventHandler OnNoteRetrieved;

        public event PauseClearedEventHandler OnPauseCleared;

        public event PauseDetectedEventHandler OnPauseDetected;

        public event PowerUpCompleteEventHandler OnPowerUpComplete;

        public event PowerUpEventHandler OnPowerUp;

        public event PUPEscrowEventHandler OnPUPEscrow;

        public event RejectedEventHandler OnRejected;

        public event ReturnedEventHandler OnReturned;

        public event StackedEventHandler OnStacked;

        public event StackedWithDocInfoEventHandler OnStackedWithDocInfo;

        public event StackerFullClearedEventHandler OnStackerFullCleared;

        public event StackerFullEventHandler OnStackerFull;

        public event StallClearedEventHandler OnStallCleared;

        public event StallDetectedEventHandler OnStallDetected;

        public bool CapAdvBookmark => this._capAdvBookmark;

        public bool CapApplicationID => this._capApplicationID;

        public bool CapApplicationPN => this._capApplicationPN;

        public bool CapAssetNumber => this._capAssetNumber;

        public bool CapAudit => this._capAudit;

        public bool CapBarCodes => this._capBarCodes;

        public bool CapBarCodesExt => this._capBarCodesExt;

        public bool CapBNFStatus => this._capBNFStatus;

        public bool CapBookmark => this._capBookmark;

        public bool CapBootPN => this._capBootPN;

        public bool CapCalibrate => this._capCalibrate;

        public bool CapCashBoxTotal => this._capCashBoxTotal;

        public bool CapClearAudit => this._capClearAudit;

        public bool CapCouponExt => this._capCouponExt;

        public bool CapDevicePaused => this._capDevicePaused;

        public bool CapDeviceSoftReset => this._capDeviceSoftReset;

        public bool CapDeviceType => this._capDeviceType;

        public bool CapDeviceResets => this._capDeviceResets;

        public bool CapDeviceSerialNumber => this._capDeviceSerialNumber;

        public bool CapEasitrax => this._capEasiTrax;

        public bool CapEscrowTimeout => this._capEscrowTimeout;

        public bool CapFlashDownload => this._capFlashDownload;

        public bool CapNoPush => this._capNoPush;

        public bool CapNoteRetrieved => this._capNoteRetrieved;

        public bool CapOrientationExt => this._capOrientationExt;

        public bool CapPupExt => this._capPupExt;

        public bool CapSetBezel => this._capSetBezel;

        public bool CapTestDoc => this._capTestDoc;

        public bool CapVariantID => this._capVariantID;

        public bool CapVariantPN => this._capVariantPN;

        public string ApplicationID
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapApplicationID, nameof(ApplicationID));
                byte[] bytes = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)14 },
                    "QryApplicationID"
                );
                return bytes.Length == 14 ? new UTF8Encoding().GetString(bytes, 3, 9) : "";
            }
        }

        public string ApplicationPN
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapApplicationPN, nameof(ApplicationPN));
                byte[] bytes = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)7 },
                    "QryApplicationPN"
                );
                return bytes.Length == 14 ? new UTF8Encoding().GetString(bytes, 3, 9) : "";
            }
        }

        public string AssetNumber
        {
            get
            {
                this.verifyConnected("QryAssetNumber");
                this.verifyPropertyIsAllowed(this.CapEasitrax, nameof(AssetNumber));
                byte[] bytes = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)21 },
                    "QryAssetNumber"
                );
                try
                {
                    return new UTF8Encoding().GetString(bytes, 3, 16);
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                this.verifyConnected("SetAssetNumber");
                this.verifyPropertyIsAllowed(this.CapEasitrax, nameof(AssetNumber));
                if (this.DeviceState != State.Idling && this.DeviceState != State.Failed)
                    throw new InvalidOperationException(
                        "Setting the AssetNumber is only allowed when idle or failed."
                    );
                if (value.Length > 16)
                    throw new InvalidDataException("Asset Number is too long.");
                byte[] payload = this.constructOmnibusCommand(21, (byte)112, 2);
                payload[1] = (byte)5;
                int index;
                for (index = 0; index < value.Length; ++index)
                    payload[index + 5] = (byte)value[index];
                for (; index < 16; ++index)
                    payload[index + 5] = (byte)32;
                this.sendAsynchronousCommand(payload, "SetAssetNumber");
            }
        }

        public int[] AuditLifeTimeTotals
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapAudit, nameof(AuditLifeTimeTotals));
                byte[] numArray = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)10 },
                    "QryAuditLifeTimeTotals"
                );
                if (numArray.Length < 13 || (numArray.Length - 5) % 8 != 0)
                    return new int[0];
                int length = ((int)numArray[1] - 5) / 8;
                int[] auditLifeTimeTotals = new int[length];
                for (int index = 0; index < length; ++index)
                    auditLifeTimeTotals[index] =
                        (((int)numArray[8 * index + 3] & 15) << 28)
                        + (((int)numArray[8 * index + 4] & 15) << 24)
                        + (((int)numArray[8 * index + 5] & 15) << 20)
                        + (((int)numArray[8 * index + 6] & 15) << 16)
                        + (((int)numArray[8 * index + 7] & 15) << 12)
                        + (((int)numArray[8 * index + 8] & 15) << 8)
                        + (((int)numArray[8 * index + 9] & 15) << 4)
                        + ((int)numArray[8 * index + 10] & 15);
                return auditLifeTimeTotals;
            }
        }

        public int[] AuditPerformance
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapAudit, nameof(AuditPerformance));
                byte[] numArray = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)12 },
                    "QryAuditPerformance"
                );
                if (numArray.Length < 9 || (numArray.Length - 5) % 4 != 0)
                    return new int[0];
                int length = ((int)numArray[1] - 5) / 4;
                int[] auditPerformance = new int[length];
                for (int index = 0; index < length; ++index)
                    auditPerformance[index] =
                        (((int)numArray[4 * index + 3] & 15) << 12)
                        + (((int)numArray[4 * index + 4] & 15) << 8)
                        + (((int)numArray[4 * index + 5] & 15) << 4)
                        + ((int)numArray[4 * index + 6] & 15);
                return auditPerformance;
            }
        }

        public int[] AuditQP
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapAudit, nameof(AuditQP));
                byte[] numArray = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)11 },
                    "QryAuditQP"
                );
                if (numArray.Length < 9 || (numArray.Length - 5) % 4 != 0)
                    return new int[0];
                int length = ((int)numArray[1] - 5) / 4;
                int[] auditQp = new int[length];
                for (int index = 0; index < length; ++index)
                    auditQp[index] =
                        (((int)numArray[4 * index + 3] & 15) << 12)
                        + (((int)numArray[4 * index + 4] & 15) << 8)
                        + (((int)numArray[4 * index + 5] & 15) << 4)
                        + ((int)numArray[4 * index + 6] & 15);
                return auditQp;
            }
        }

        public bool AutoStack
        {
            get => this._autoStack;
            set
            {
                this.verifyConnected(nameof(AutoStack));
                this._autoStack = value;
                if (!this._autoStack || this.DeviceState != State.Escrow)
                    return;
                this.EscrowStack();
            }
        }

        public string BarCode
        {
            get
            {
                if (this.DocType != DocumentType.Barcode)
                    throw new InvalidOperationException(
                        "The BarCode property is not valid when DocType != DocumentType.Barcode"
                    );
                return this._barCode;
            }
        }

        public Bill Bill
        {
            get
            {
                if (this.DocType != DocumentType.Bill)
                    throw new InvalidOperationException(
                        "The Bill property is not valid when DocType != DocumentType.Bill"
                    );
                return this._bill;
            }
        }

        public Bill[] BillTypes => this._lstBillTypes.ToArray();

        [Obsolete(
            "BillTypeEnables property has been deprecated. Please use Get/SetBillTypeEnables",
            false
        )]
        public bool[] BillTypeEnables
        {
            get => this._lstBillTypeEnables.ToArray();
            set => this.SetBillTypeEnables(ref value);
        }

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BOOL)]
        public bool[] GetBillTypeEnables() => this._lstBillTypeEnables.ToArray();

        public void SetBillTypeEnables(
            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BOOL)]
                ref bool[] billTypeEnables
        )
        {
            if (!this.Connected)
                throw new InvalidOperationException(
                    "Calling BillTypeEnables not allowed when not connected."
                );
            if (this.DeviceState != State.Idling)
                throw new InvalidOperationException(
                    "Calling BillTypeEnables not allowed when not idle."
                );
            if (billTypeEnables.Length != this._lstBillTypes.Count)
                throw new InvalidOperationException(
                    "BillTypeEnables.Length must match BillTypes.Length."
                );
            this._lstBillTypeEnables.Clear();
            this._lstBillTypeEnables.AddRange((IEnumerable<bool>)billTypeEnables);
            if (!this._expandedNoteReporting)
                return;
            byte[] payload =
                this._lstBillTypeEnables.Count > 50
                    ? this.constructOmnibusCommand(26, (byte)112, 2)
                    : this.constructOmnibusCommand(15, (byte)112, 2);
            payload[1] = (byte)3;
            for (int index = 0; index < this._lstBillTypeEnables.Count; ++index)
            {
                int num1 = index / 7;
                int num2 = 1 << index % 7;
                if (this._lstBillTypeEnables[index])
                    payload[5 + num1] |= (byte)num2;
            }
            this.sendAsynchronousCommand(payload, "SetBillEnables");
        }

        public Bill[] BillValues => this._lstBillValues.ToArray();

        [Obsolete(
            "BillValueEnables property has been deprecated. Please use Get/SetBillValueEnables",
            false
        )]
        public bool[] BillValueEnables
        {
            get => this._lstBillValueEnables.ToArray();
            set => this.SetBillValueEnables(ref value);
        }

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BOOL)]
        public bool[] GetBillValueEnables() => this._lstBillValueEnables.ToArray();

        public void SetBillValueEnables(
            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BOOL)]
                ref bool[] billValueEnables
        )
        {
            if (!this.Connected)
                throw new InvalidOperationException(
                    "Calling BillValueEnables not allowed when not connected."
                );
            if (billValueEnables.Length != this._lstBillValues.Count)
                throw new InvalidOperationException(
                    "BillValueEnables.Length must match BillValues.Length."
                );
            this._lstBillValueEnables.Clear();
            this._lstBillValueEnables.AddRange((IEnumerable<bool>)billValueEnables);
            bool[] billTypeEnables = this.GetBillTypeEnables();
            for (int index1 = 0; index1 < this._lstBillValueEnables.Count; ++index1)
            {
                for (int index2 = 0; index2 < this._lstBillTypes.Count; ++index2)
                {
                    if (
                        this.BillTypes[index2].Value == this.BillValues[index1].Value
                        && this.BillTypes[index2].Country == this.BillValues[index1].Country
                    )
                        billTypeEnables[index2] = this._lstBillValueEnables[index1];
                }
            }
            this.SetBillTypeEnables(ref billTypeEnables);
        }

        public BNFStatus BNFStatus
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapBNFStatus, nameof(BNFStatus));
                byte[] numArray = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)16 },
                    "QryBNFStatus"
                );
                this._bnfErrorStatus = BNFErrorStatus.NoError;
                if (numArray.Length != 11)
                    return BNFStatus.Unknown;
                if (numArray[3] == (byte)0)
                    return BNFStatus.NotAttached;
                if (numArray[4] == (byte)0)
                    return BNFStatus.OK;
                if (
                    Enum.IsDefined(
                        typeof(BNFErrorStatus),
                        (object)int.Parse(numArray[5].ToString())
                    )
                )
                    this._bnfErrorStatus = (BNFErrorStatus)numArray[5];
                return BNFStatus.Error;
            }
        }

        public BNFErrorStatus LastBNFError
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapBNFStatus, "BNFStatus");
                return this._bnfErrorStatus;
            }
        }

        public string BootPN
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapBootPN, nameof(BootPN));
                byte[] bytes = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)6 },
                    "QryBootPN"
                );
                return bytes.Length == 14 ? new UTF8Encoding().GetString(bytes, 3, 9) : "";
            }
        }

        public bool CashBoxAttached => this._cashBoxAttached;

        public bool CashBoxFull => this._cashBoxFull;

        public int CashBoxTotal
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapCashBoxTotal, nameof(CashBoxTotal));
                byte[] numArray = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)1 },
                    "QryCashBoxTotal"
                );
                return numArray.Length != 11
                    ? 0
                    : (((int)numArray[3] & 15) << 20)
                        + (((int)numArray[4] & 15) << 16)
                        + (((int)numArray[5] & 15) << 12)
                        + (((int)numArray[6] & 15) << 8)
                        + (((int)numArray[7] & 15) << 4)
                        + ((int)numArray[8] & 15);
            }
        }

        public bool Connected => this._connected;

        public Coupon Coupon
        {
            get
            {
                if (this.DocType != DocumentType.Coupon)
                    throw new InvalidOperationException(
                        "The Coupon property is not valid when DocType != DocumentType.Coupon"
                    );
                return this._coupon;
            }
        }

        public bool DebugLog
        {
            get => this._debugLog;
            set
            {
                if (value)
                {
                    if (!this._debugLog && this.Connected)
                        this.openLogFile();
                    this._debugLog = value;
                }
                else
                {
                    if (this._debugLog && this.Connected)
                        this.closeLogFile();
                    this._debugLog = value;
                }
            }
        }

        public string DebugLogPath
        {
            get => this._debugLogPath;
            set
            {
                StringBuilder stringBuilder = new StringBuilder(value);
                if (stringBuilder[stringBuilder.Length - 1] == '\\')
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                this._debugLogPath = stringBuilder.ToString();
            }
        }

        public bool DeviceBusy => this.DeviceState != State.Idling;

        public int DeviceCRC
        {
            get
            {
                this.verifyPropertyIsAllowed(true, nameof(DeviceCRC));
                byte[] numArray = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)0 },
                    "QryDeviceCRC"
                );
                return numArray.Length != 11
                    ? 0
                    : (((int)numArray[3] & 15) << 12)
                        + (((int)numArray[4] & 15) << 8)
                        + (((int)numArray[5] & 15) << 4)
                        + ((int)numArray[6] & 15);
            }
        }

        public bool DeviceFailure => this._deviceState == State.Failed;

        public bool DeviceJammed => this._isDeviceJammed;

        public int DeviceModel => this._deviceModel;

        public bool DevicePaused => this._devicePaused;

        public string DevicePortName => this._port;

        public PowerUp DevicePowerUp => this._powerUp;

        public int DeviceResets
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapDeviceResets, nameof(DeviceResets));
                byte[] numArray = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)2 },
                    "QryDeviceResets"
                );
                return numArray.Length != 11
                    ? 0
                    : (((int)numArray[3] & 15) << 20)
                        + (((int)numArray[4] & 15) << 16)
                        + (((int)numArray[5] & 15) << 12)
                        + (((int)numArray[6] & 15) << 8)
                        + (((int)numArray[7] & 15) << 4)
                        + ((int)numArray[8] & 15);
            }
        }

        public int DeviceRevision => this._deviceRevision;

        public string DeviceSerialNumber
        {
            get
            {
                this.verifyConnected(nameof(DeviceSerialNumber));
                byte[] bytes = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)5 },
                    "QryDeviceSerialNumber"
                );
                if (bytes.Length != 25)
                    return "";
                int index = 3;
                while (
                    index < bytes.Length
                    && bytes[index] > (byte)32
                    && bytes[index] < (byte)127
                    && index <= 22
                )
                    ++index;
                try
                {
                    return new UTF8Encoding().GetString(bytes, 3, index - 3);
                }
                catch
                {
                    return "";
                }
            }
        }

        public bool DeviceStalled => this.DeviceState == State.Stalled;

        public State DeviceState =>
            this.DeviceFailure && this.Connected ? State.Failed : this._deviceState;

        public string DeviceType
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapDeviceType, nameof(DeviceType));
                byte[] bytes = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)4 },
                    "QryDeviceType"
                );
                UTF8Encoding utF8Encoding = new UTF8Encoding();
                int index = 3;
                while (
                    index < bytes.Length
                    && bytes[index] > (byte)32
                    && bytes[index] < (byte)127
                    && index <= 22
                )
                    ++index;
                try
                {
                    return utF8Encoding.GetString(bytes, 3, index - 3);
                }
                catch
                {
                    return "";
                }
            }
        }

        public DocumentType DocType => this._docType;

        [Obsolete("DownloadTimeout has been deprecated.", false)]
        public int DownloadTimeout
        {
            get => this._downloadTimeout;
            set => this._downloadTimeout = value;
        }

        [Obsolete("TransactionTimeout has been deprecated.", false)]
        public int TransactionTimeout
        {
            get => this._transactionTimeout;
            set => this._transactionTimeout = value;
        }

        public int DisconnectTimeout
        {
            get => this._disconnectTimeout;
            set =>
                this._disconnectTimeout =
                    value > 0
                        ? value
                        : throw new InvalidOperationException(
                            "Timeout value must be a positive non-zero integer"
                        );
        }

        public bool EnableAcceptance
        {
            get => this._enableAcceptance;
            set
            {
                this.verifyConnected(nameof(EnableAcceptance));
                this._enableAcceptance = value;
                State deviceState = this.DeviceState;
                if (this._enableAcceptance)
                    return;
                if (deviceState == State.Accepting)
                {
                    this._bDisabledWhileAccpeting = true;
                }
                else
                {
                    if (deviceState != State.Escrow)
                        return;
                    this.EscrowReturn();
                }
            }
        }

        public bool EnableBarCodes
        {
            get => this._enableBarCodes;
            set
            {
                this.verifyPropertyIsAllowed(this._capBarCodes, nameof(EnableBarCodes));
                this._enableBarCodes = value;
            }
        }

        public bool EnableBookmarks
        {
            get => this._enableBookmarks;
            set
            {
                this.verifyPropertyIsAllowed(this._capBookmark, nameof(EnableBookmarks));
                this._enableBookmarks = value;
            }
        }

        public bool EnableCouponExt
        {
            get => this._enableCouponExt;
            set
            {
                this.verifyPropertyIsAllowed(this._capCouponExt, nameof(EnableCouponExt));
                this._enableCouponExt = value;
            }
        }

        public bool EnableNoPush
        {
            get => this._enableNoPush;
            set
            {
                this.verifyPropertyIsAllowed(this._capNoPush, nameof(EnableNoPush));
                this._enableNoPush = value;
            }
        }

        public Orientation EscrowOrientation =>
            this.CapOrientationExt ? this._escrowOrientation : Orientation.Unknown;

        public BanknoteClassification EscrowClassification => this._banknoteClassification;

        public bool HighSecurity
        {
            get => this._highSecurity;
            set => this._highSecurity = value;
        }

        public OrientationControl OrientationCtl
        {
            get => this._orientationCtl;
            set => this._orientationCtl = value;
        }

        public OrientationControl OrientationCtlExt
        {
            get => this._orientationCtlExt;
            set => this._orientationCtlExt = value;
        }

        public string[] VariantNames
        {
            get
            {
                this.verifyPropertyIsAllowed(true, nameof(VariantNames));
                byte[] numArray = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)8 },
                    "QryVariantNames"
                );
                List<string> stringList = new List<string>();
                StringBuilder stringBuilder = new StringBuilder();
                for (int index = 3; index < numArray.Length - 2; ++index)
                {
                    byte num = numArray[index];
                    if (num > (byte)25 && num < (byte)127)
                    {
                        if (num == (byte)95)
                        {
                            if (stringBuilder.Length != 0)
                                stringList.Add(stringBuilder.ToString());
                            stringBuilder = new StringBuilder();
                        }
                        else
                            stringBuilder.Append((char)num);
                    }
                }
                if (stringBuilder.Length != 0)
                    stringList.Add(stringBuilder.ToString());
                return stringList.ToArray();
            }
        }

        public string VariantID
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapVariantID, nameof(VariantID));
                byte[] bytes = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)15 },
                    "QryVariantID"
                );
                return bytes.Length == 14 ? new UTF8Encoding().GetString(bytes, 3, 9) : "";
            }
        }

        public string VariantPN
        {
            get
            {
                this.verifyPropertyIsAllowed(this.CapVariantPN, nameof(VariantPN));
                byte[] bytes = this.sendSynchronousCommand(
                    new byte[4] { (byte)96, (byte)0, (byte)0, (byte)9 },
                    "QryVariantPN"
                );
                return bytes.Length == 14 ? new UTF8Encoding().GetString(bytes, 3, 9) : "";
            }
        }

        public string APIVersion => Acceptor.VERSION;

        public Acceptor()
        {
            this._worker = new Worker(this);
            this._debugLog = false;
            this.init();
            try
            {
                this._debugLogPath = Directory.GetCurrentDirectory();
            }
            catch
            {
                this.DebugLogPath = ".";
            }
        }

        public void Open(string portName) => this.Open(portName, PowerUp.A);

        public void Open(string portName, PowerUp powerUp)
        {
            Close();
            if (this.Connected)
                throw new InvalidOperationException(
                    "Open cannot be called when Connected == true."
                );
            if (this._openThread != null && this._openThread.IsAlive)
                throw new InvalidOperationException(
                    "Open cannot be called second time without previous Close call"
                );
            this.init();
            _transport = new EBDS_SerialPort(this);
            this._powerUp = powerUp;
            this._port = portName;
            if (this._debugLog)
                this.openLogFile();
            _transport.openPort(this._port);
            this._stopOpenThread = false;
            this._openThread = new Thread(new ThreadStart(this.THR_OpenThread));
            this._openThread.Name = "open thread";
            this._openThread.Start();
            this._worker.startThread();
        }

        public void Close()
        {
            if (this._flashDownloadThread != null && this._flashDownloadThread.IsAlive)
            {
                this._stopFlashDownloadThread = true;
                this._flashDownloadThread.Join();
            }
            else if (!this.Connected && this._openThread != null && this._openThread.IsAlive)
            {
                this._raiseDisconnectedEvent = true;
                this._stopOpenThread = true;
            }
            bool enableAcceptance = this._enableAcceptance;
            this._enableAcceptance = false;
            this._worker.stopThread(enableAcceptance);
            while (this._worker.IsRunning)
                Thread.Sleep(500);
            if (_transport != null)
            {
                _transport.stopPortReading();
                _transport.close();
                _transport.Dispose();
                _transport = null;
            }
            this._port = "";
            this.setConnected(false);
            if (this._raiseDisconnectedEvent)
                this.RaiseDisconnectedEvent();
            if (!this._debugLog)
                return;
            this.closeLogFile();
        }

        public bool CancelAdvancedBookmarkMode()
        {
            this.verifyPropertyIsAllowed(this.CapAdvBookmark, "Advanced Bookmark Mode");
            byte[] payload = this.constructOmnibusCommand(6, (byte)112, 2);
            payload[1] = (byte)13;
            payload[5] = (byte)0;
            byte[] reply = this.sendSynchronousCommand(payload, "Advanced Bookmark Mode command");
            this.processReply(reply);
            return reply[10] == (byte)1;
        }

        public void Calibrate() { }

        public bool ClearAudit()
        {
            this.verifyConnected(nameof(ClearAudit));
            if (this.DeviceState != State.Idling && this.DeviceState != State.Failed)
                throw new InvalidOperationException(
                    "Clear Audit is only allowed when DeviceState == (Idling or Failed)."
                );
            byte[] payload = this.constructOmnibusCommand(5, (byte)112, 2);
            payload[1] = (byte)29;
            byte[] reply = this.sendSynchronousCommand(payload, "Clear Audit command");
            this.processReply(reply);
            return reply[10] == (byte)1;
        }

        public bool DisableCashboxCleanlinessReporting()
        {
            this.verifyConnected("Disable Cashbox Cleanliness Reporting");
            byte[] payload = this.constructOmnibusCommand(6, (byte)112, 2);
            payload[1] = (byte)16;
            payload[5] = (byte)0;
            byte[] reply = this.sendSynchronousCommand(payload, "Cashbox Cleanliness Reporting");
            this.processReply(reply);
            return reply[10] == (byte)1;
        }

        public bool EnableCashboxCleanlinessReporting()
        {
            this.verifyConnected("Enable Cashbox Cleanliness Reporting");
            byte[] payload = this.constructOmnibusCommand(6, (byte)112, 2);
            payload[1] = (byte)16;
            payload[5] = (byte)1;
            byte[] reply = this.sendSynchronousCommand(payload, "Cashbox Cleanliness Reporting");
            this.processReply(reply);
            return reply[10] == (byte)1;
        }

        public bool EnterAdvancedBookmarkMode()
        {
            this.verifyPropertyIsAllowed(this.CapAdvBookmark, "Advanced Bookmark Mode");
            byte[] payload = this.constructOmnibusCommand(6, (byte)112, 2);
            payload[1] = (byte)13;
            payload[5] = (byte)1;
            byte[] reply = this.sendSynchronousCommand(payload, "Advanced Bookmark Mode command");
            this.processReply(reply);
            return reply[10] == (byte)1;
        }

        public void EscrowReturn()
        {
            this.verifyConnected(nameof(EscrowReturn));
            byte[] payload = this.constructOmnibusCommand(4, (byte)16, 1);
            payload[2] |= (byte)64;
            this.sendAsynchronousCommand(payload, "EscrowReturn command");
        }

        public void EscrowStack()
        {
            this.verifyConnected(nameof(EscrowStack));
            byte[] payload = this.constructOmnibusCommand(4, (byte)16, 1);
            payload[2] |= (byte)32;
            this.sendAsynchronousCommand(payload, "EscrowStack command");
        }

        public void FlashDownload(string filePath)
        {
            if (
                (!this.Connected || !_transport.IsOpen || this._worker.commLost())
                && this.DeviceState != State.DownloadRestart
            )
                throw new InvalidOperationException(
                    "Calling FlashDownload not allowed when not connected."
                );
            if (this.DeviceState != State.Idling && this.DeviceState != State.DownloadRestart)
                throw new InvalidOperationException(
                    "Flash download allowed only when DeviceState == Idling."
                );
            if (
                (File.GetAttributes(filePath) & FileAttributes.Directory)
                == FileAttributes.Directory
            )
            {
                FileNotFoundException ex = new FileNotFoundException(
                    string.Format("Provided path is a directory: '{0}'", (object)filePath)
                );
                this.log((Exception)ex);
                throw ex;
            }
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                FileNotFoundException ex = new FileNotFoundException(
                    string.Format("File does not exist: '{0}'", (object)fileInfo.FullName)
                );
                this.log((Exception)ex);
                throw ex;
            }
            if (fileInfo.Extension.ToLower() != ".bin" && fileInfo.Extension.ToLower() != ".bds")
            {
                InvalidOperationException ex = new InvalidOperationException(
                    string.Format(
                        "Cannot download file. '{0}' format is not valid, must be a '.bin' or '.bds'.",
                        (object)fileInfo.Extension
                    )
                );
                this.log((Exception)ex);
                throw ex;
            }
            if (fileInfo.Length % 32L != 0L)
            {
                InvalidOperationException ex = new InvalidOperationException(
                    string.Format(
                        "Cannot download file. File length must be divisible by 32.",
                        (object)fileInfo.Extension
                    )
                );
                this.log((Exception)ex);
                throw ex;
            }
            this._flashDownloadThread = new Thread(
                new ParameterizedThreadStart(this.THR_FlashDownloadThread)
            );
            this._flashDownloadThread.IsBackground = true;
            this._flashDownloadThread.Start((object)filePath);
        }

        public AuditContainer GetBanknoteAuditData()
        {
            bool enableAcceptance = this.EnableAcceptance;
            this.EnableAcceptance = false;
            long ticks = DateTime.Now.Ticks;
            while (this.DeviceState != State.Idling && this.DeviceState != State.Failed)
            {
                Thread.Sleep(100);
                if (new TimeSpan(DateTime.Now.Ticks - ticks).TotalMilliseconds < 5000.0)
                    throw new Exception(
                        string.Format(
                            "Device has not reached Idle state in the expected amount of time. Current state == '{0}'",
                            (object)this.DeviceState
                        )
                    );
            }
            byte[] numArray1 = this.sendSynchronousCommand(
                new byte[4] { (byte)96, (byte)0, (byte)0, (byte)22 },
                "Query Audit Total Documents Reporting Structure"
            );
            if (numArray1.Length == 0)
                throw new Exception("Reporting Structure response was null");
            if (numArray1.Length != 11)
                throw new Exception(
                    string.Format(
                        "Reporting Structure response did not contain the expected number of bytes. Expected 0x{0:X2} but detected only 0x{1:X2}",
                        (object)11,
                        (object)numArray1.Length
                    )
                );
            AuditContainer banknoteAuditData =
                ((int)numArray1[2] & 240) == 96
                    ? new AuditContainer((int)numArray1[4], (int)numArray1[5], (int)numArray1[6])
                    : throw new Exception(
                        string.Format(
                            "Reporting Structure response was not of the correct message type. Expected 0x{0:X2} but detected only 0x{1:X2}",
                            (object)(byte)96,
                            (object)((int)numArray1[2] & 240)
                        )
                    );
            for (int field = 23; field < 26; ++field)
            {
                string str;
                switch (field - 23)
                {
                    case 0:
                        str = "Recognized";
                        break;
                    case 1:
                        str = "Validated";
                        break;
                    case 2:
                        str = "Stacked";
                        break;
                    default:
                        throw new Exception("Critical Error: Passed the maximum number of fields");
                }
                for (int orientation = 0; orientation < 4; ++orientation)
                {
                    for (int dataset = 0; dataset < banknoteAuditData.NumberOfDatasets; ++dataset)
                    {
                        byte[] sourceArray = this.sendSynchronousCommand(
                            new byte[4] { (byte)96, (byte)orientation, (byte)dataset, (byte)field },
                            string.Format("Query Audit Total Documents {0}", (object)str)
                        );
                        if (sourceArray.Length == 0)
                            throw new Exception(
                                string.Format("Total Documents {0} response was null", (object)str)
                            );
                        byte[] numArray2 =
                            ((int)sourceArray[2] & 240) == 96
                                ? new byte[sourceArray.Length - 5]
                                : throw new Exception(
                                    string.Format(
                                        "Total Documents {0} response was not of the correct message type. Expected 0x{1:X2} but detected only 0x{2:X2}",
                                        (object)str,
                                        (object)(byte)96,
                                        (object)((int)sourceArray[2] & 240)
                                    )
                                );
                        Array.Copy((Array)sourceArray, 3, (Array)numArray2, 0, numArray2.Length);
                        banknoteAuditData.appendResults(field, orientation, dataset, numArray2);
                    }
                }
            }
            this.EnableAcceptance = enableAcceptance;
            return banknoteAuditData;
        }

        public long GetRFIDSerialNumber()
        {
            this.verifyConnected(nameof(GetRFIDSerialNumber));
            if (!this.CashBoxAttached)
                throw new InvalidOperationException(
                    "GetRFIDSerialNumber is not allowed when cashbox is not installed"
                );
            switch (this.DeviceState)
            {
                case State.Idling:
                case State.Jammed:
                case State.Failed:
                    byte[] payload = this.constructOmnibusCommand(7, (byte)112, 2);
                    payload[1] = (byte)28;
                    payload[5] = (byte)0;
                    payload[6] = (byte)127;
                    byte[] reply1 = this.sendSynchronousCommand(
                        payload,
                        "Get RFID Serial Number command"
                    );
                    this.processReply(reply1);
                    if (reply1.Length != 13)
                        throw new Exception("Device firmware does not support GetRFIDSerialNumber");
                    if (reply1[10] != (byte)1)
                        throw new Exception("Device NAK'd the request for GetRFIDSerialNumber");
                    byte[] reply2 = this.sendSynchronousCommand(
                        this.constructOmnibusCommand(4, (byte)16, 1),
                        "GetRFIDSerialNumber Poll"
                    );
                    this.processReply(reply2);
                    if (reply2.Length != 46)
                        throw new Exception("GetRFIDSerialNumber Failed");
                    long num =
                        (long)(
                            (int)reply2[12] << 28
                            | (int)reply2[13] << 24
                            | (int)reply2[14] << 20
                            | (int)reply2[15] << 16
                            | (int)reply2[16] << 12
                            | (int)reply2[17] << 8
                            | (int)reply2[18] << 4
                            | (int)reply2[19]
                        ) & (long)uint.MaxValue;
                    return num != 0L
                        ? num
                        : throw new Exception("GetRFIDSerialNumber Failed - Is RFID tag present?");
                default:
                    throw new InvalidOperationException(
                        string.Format(
                            "GetRFIDSerialNumber is not allowed when DeviceState == {0}.",
                            (object)this.DeviceState
                        )
                    );
            }
        }

        public void ClearCashBoxTotal()
        {
            this.verifyConnected(nameof(ClearCashBoxTotal));
            this.sendSynchronousCommand(
                new byte[4] { (byte)96, (byte)0, (byte)0, (byte)3 },
                "ClearCashBoxTotal command"
            );
        }

        public byte[] RawTransaction(ref byte[] payload)
        {
            byte[] reply = this.sendSynchronousCommand(payload, "Raw transaction command");
            this.processReply(reply);
            return reply;
        }

        public void SetAssetNumber(string asset) => this.AssetNumber = asset;

        public void SetBezel(Bezel bezel)
        {
            this.verifyConnected(nameof(SetBezel));
            this.sendAsynchronousCommand(
                new byte[4] { (byte)96, (byte)bezel, (byte)0, (byte)17 },
                "SetBezel command"
            );
        }

        public void SoftReset()
        {
            this.verifyConnected(nameof(SoftReset));
            this._docType = DocumentType.NoValue;
            this.sendAsynchronousCommand(
                new byte[4] { (byte)96, (byte)127, (byte)127, (byte)127 },
                "SoftReset command",
                true
            );
            this._inSoftResetWaitForReply = true;
        }

        public void SpecifyEscrowTimeout(int billTimeout, int barcodeTimeout)
        {
            this.verifyConnected(nameof(SpecifyEscrowTimeout));
            byte[] payload = this.constructOmnibusCommand(7, (byte)112, 2);
            payload[1] = (byte)4;
            payload[5] = (byte)billTimeout;
            payload[6] = (byte)barcodeTimeout;
            this.processReply(this.sendSynchronousCommand(payload, "SpecifyEscrowTimeout command"));
        }

        [Obsolete("SpecifyPupExt has been deprecated.", false)]
        public void SpecifyPupExt(
            char pupMode,
            PupExt preEscrow,
            PupExt atEscrow,
            PupExt postEscrow,
            PupExt preStack
        ) { }

        public void StopDownload()
        {
            if (this._flashDownloadThread == null || !this._flashDownloadThread.IsAlive)
                return;
            this._stopFlashDownloadThread = true;
            this._flashDownloadThread.Join();
        }

        public IDocument getDocument() =>
            this._docType != DocumentType.Bill || this._bill == null
                ? (
                    this._docType != DocumentType.Barcode || this._barCode == null
                        ? (
                            this._docType != DocumentType.Coupon || this._coupon == null
                                ? (IDocument)null
                                : (IDocument)this._coupon
                        )
                        : (IDocument)new Barcode(this._barCode)
                )
                : (IDocument)this._bill;

        public void SetCustomerConfigurationOption(ConfigurationIndex index, byte value)
        {
            byte[] numArray = this.sendSynchronousCommand(
                new byte[4] { (byte)96, (byte)index, value, (byte)37 },
                nameof(SetCustomerConfigurationOption)
            );
            if (numArray == null)
                throw new Exception("NULL response to SetCustomerConfigurationOption");
            if (numArray.Length != 11)
                throw new Exception(
                    string.Format(
                        "Unexpected response to SetCustomerConfigurationOption. Received a data length of {0:X2}",
                        (object)numArray.Length
                    )
                );
            if (numArray[3] == (byte)0)
                return;
            if (numArray[3] == (byte)1)
                throw new ArgumentException(
                    "Invalid Configuration Index passed to SetCustomerConfigurationOption"
                );
            if (numArray[3] == (byte)2)
                throw new ArgumentException(
                    "Invalid setting value passed to SetCustomerConfigurationOption"
                );
            if (numArray[3] == (byte)127)
                throw new Exception(
                    string.Format(
                        "'{0}' was negatively acknowledged while in device state '{1}'",
                        (object)nameof(SetCustomerConfigurationOption),
                        (object)this.DeviceState
                    )
                );
            throw new Exception(
                string.Format(
                    "Unexpected response to SetCustomerConfigurationOption.Received a data byte of {0:X2}",
                    (object)numArray[3]
                )
            );
        }

        public byte QueryCustomerConfigurationOption(ConfigurationIndex index)
        {
            byte[] numArray = this.sendSynchronousCommand(
                new byte[4] { (byte)96, (byte)index, (byte)0, (byte)38 },
                nameof(QueryCustomerConfigurationOption)
            );
            if (numArray == null)
                throw new Exception("NULL response to QueryCustomerConfigurationOption");
            if (numArray.Length != 11)
                throw new Exception("Unexpected response");
            if (numArray[3] == (byte)0)
                return numArray[4];
            if (numArray[3] == (byte)1)
                throw new ArgumentException(
                    "Invalid Configuration Index passed to QueryCustomerConfigurationOption"
                );
            throw new Exception("Unexpected response");
        }

        private enum eDownloadBaudRate : byte
        {
            BAUD_INVALID,
            BAUD_9600,
            BAUD_19200,
            BAUD_38400,
            BAUD_115200,
        }
    }
}
