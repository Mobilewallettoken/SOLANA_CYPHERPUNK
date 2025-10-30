using System.Text;

namespace MobileWallet.Desktop.MPostLib
{
    public class CashAcceptor
    {
        public bool CloseBtn = false;
        public bool CalibrateBtn = false;
        public bool StackBtn = false;
        public bool ReturnBtn = false;
        public string CashInBox = "0";

        public List<string> events = new List<string>();
        public Acceptor BillAcceptor = new Acceptor();
        private PowerUp PupMode = PowerUp.A;
        public State State => BillAcceptor.DeviceState;

        public EventHandler<string> OnEscrow;
        public EventHandler<string> OnStack;

        #region Event Delegate Definitions

        private CalibrateFinishEventHandler CalibrateFinishDelegate;
        private CalibrateProgressEventHandler CalibrateProgressDelegate;
        private CalibrateStartEventHandler CalibrateStartDelegate;
        private CashBoxCleanlinessEventHandler CashBoxCleanlinessDelegate;
        private CashBoxAttachedEventHandler CashBoxAttachedDelegate;
        private CashBoxRemovedEventHandler CashBoxRemovedDelegate;
        private CheatedEventHandler CheatedDelegate;
        private ClearAuditEventHandler ClearAuditDelegate;
        private ConnectedEventHandler ConnectedDelegate;
        private DisconnectedEventHandler DisconnectedDelegate;
        private DownloadFinishEventHandler DownloadFinishDelegate;
        private DownloadProgressEventHandler DownloadProgressDelegate;
        private DownloadRestartEventHandler DownloadRestartDelegate;
        private DownloadStartEventHandler DownloadStartDelegate;
        private ErrorOnSendMessageEventHandler ErrorOnSendMessageDelegate;
        private EscrowEventHandler EscrowedDelegate;
        private FailureClearedEventHandler FailureClearedDelegate;
        private FailureDetectedEventHandler FailureDetectedDelegate;
        private InvalidCommandEventHandler InvalidCommandDelegate;
        private JamClearedEventHandler JamClearedDelegate;
        private JamDetectedEventHandler JamDetectedDelegate;
        private NoteRetrievedEventHandler NoteRetrievedDelegate;
        private PauseClearedEventHandler PauseClearedDelegate;
        private PauseDetectedEventHandler PauseDetectedDelegate;
        private PowerUpCompleteEventHandler PowerUpCompleteDelegate;
        private PowerUpEventHandler PowerUpDelegate;
        private PUPEscrowEventHandler PUPEscrowDelegate;
        private RejectedEventHandler RejectedDelegate;
        private ReturnedEventHandler ReturnedDelegate;
        private StackedWithDocInfoEventHandler StackedDelegate;

        // A new stacked event with document information has been added. Recommanded to be used.
        private StackedWithDocInfoEventHandler StackedWithDocInfoDelegate;
        private StackerFullClearedEventHandler StackerFullClearedDelegate;
        private StackerFullEventHandler StackerFullDelegate;
        private StallClearedEventHandler StallClearedDelegate;
        private StallDetectedEventHandler StallDetectedDelegate;

        #endregion


        public CashAcceptor()
        {
            CalibrateFinishDelegate = new CalibrateFinishEventHandler(HandleCalibrateFinishEvent);
            CalibrateProgressDelegate = new CalibrateProgressEventHandler(
                HandleCalibrateProgressEvent
            );
            CalibrateStartDelegate = new CalibrateStartEventHandler(HandleCalibrateStartEvent);
            CashBoxCleanlinessDelegate = new CashBoxCleanlinessEventHandler(
                HandleCashBoxCleanlinessEvent
            );
            CashBoxAttachedDelegate = new CashBoxAttachedEventHandler(HandleCashBoxAttachedEvent);
            CashBoxRemovedDelegate = new CashBoxRemovedEventHandler(HandleCashBoxRemovedEvent);
            CheatedDelegate = new CheatedEventHandler(HandleCheatedEvent);
            ClearAuditDelegate = new ClearAuditEventHandler(HandleClearAuditEvent);
            ConnectedDelegate = new ConnectedEventHandler(HandleConnectedEvent);
            DisconnectedDelegate = new DisconnectedEventHandler(HandleDisconnectedEvent);
            DownloadFinishDelegate = new DownloadFinishEventHandler(HandleDownloadFinishEvent);
            DownloadProgressDelegate = new DownloadProgressEventHandler(
                HandleDownloadProgressEvent
            );
            DownloadRestartDelegate = new DownloadRestartEventHandler(HandleDownloadRestartEvent);
            DownloadStartDelegate = new DownloadStartEventHandler(HandleDownloadStartEvent);
            ErrorOnSendMessageDelegate = new ErrorOnSendMessageEventHandler(
                HandleSendMessageErrorEvent
            );
            EscrowedDelegate = new EscrowEventHandler(HandleEscrowedEvent);
            FailureClearedDelegate = new FailureClearedEventHandler(HandleFailureClearedEvent);
            FailureDetectedDelegate = new FailureDetectedEventHandler(HandleFailureDetectedEvent);
            InvalidCommandDelegate = new InvalidCommandEventHandler(HandleInvalidCommandEvent);
            JamClearedDelegate = new JamClearedEventHandler(HandleJamClearedEvent);
            JamDetectedDelegate = new JamDetectedEventHandler(HandleJamDetectedEvent);
            NoteRetrievedDelegate = new NoteRetrievedEventHandler(HandleNoteRetrievedEvent);
            PauseClearedDelegate = new PauseClearedEventHandler(HandlePauseClearedEvent);
            PauseDetectedDelegate = new PauseDetectedEventHandler(HandlePauseDetectedEvent);
            PowerUpCompleteDelegate = new PowerUpCompleteEventHandler(HandlePowerUpCompleteEvent);
            PowerUpDelegate = new PowerUpEventHandler(HandlePowerUpEvent);
            PUPEscrowDelegate = new PUPEscrowEventHandler(HandlePUPEscrowEvent);
            RejectedDelegate = new RejectedEventHandler(HandleRejectedEvent);
            ReturnedDelegate = new ReturnedEventHandler(HandleReturnedEvent);
            StackedDelegate = new StackedWithDocInfoEventHandler(HandleStackedEvent);
            // A new stacked event with document information has been added. Recommanded to be used.
            StackedWithDocInfoDelegate = new StackedWithDocInfoEventHandler(
                HandleStackedWithDocInfoEvent
            );
            StackerFullClearedDelegate = new StackerFullClearedEventHandler(
                HandleStackerFullClearedEvent
            );
            StackerFullDelegate = new StackerFullEventHandler(HandleStackerFullEvent);
            StallClearedDelegate = new StallClearedEventHandler(HandleStallClearedEvent);
            StallDetectedDelegate = new StallDetectedEventHandler(HandleStallDetectedEvent);

            // Connect to the events.
            BillAcceptor.OnCalibrateFinish += CalibrateFinishDelegate;
            BillAcceptor.OnCalibrateProgress += CalibrateProgressDelegate;
            BillAcceptor.OnCalibrateStart += CalibrateStartDelegate;
            BillAcceptor.OnCashBoxCleanlinessDetected += CashBoxCleanlinessDelegate;
            BillAcceptor.OnCashBoxAttached += CashBoxAttachedDelegate;
            BillAcceptor.OnCashBoxRemoved += CashBoxRemovedDelegate;
            BillAcceptor.OnCheated += CheatedDelegate;
            BillAcceptor.OnClearAuditComplete += ClearAuditDelegate;
            BillAcceptor.OnConnected += ConnectedDelegate;
            BillAcceptor.OnDisconnected += DisconnectedDelegate;
            BillAcceptor.OnDownloadFinish += DownloadFinishDelegate;
            BillAcceptor.OnDownloadProgress += DownloadProgressDelegate;
            BillAcceptor.OnDownloadRestart += DownloadRestartDelegate;
            BillAcceptor.OnDownloadStart += DownloadStartDelegate;
            BillAcceptor.OnSendMessageFailure += ErrorOnSendMessageDelegate;
            BillAcceptor.OnEscrow += EscrowedDelegate;
            BillAcceptor.OnFailureCleared += FailureClearedDelegate;
            BillAcceptor.OnFailureDetected += FailureDetectedDelegate;
            BillAcceptor.OnInvalidCommand += InvalidCommandDelegate;
            BillAcceptor.OnJamCleared += JamClearedDelegate;
            BillAcceptor.OnJamDetected += JamDetectedDelegate;
            BillAcceptor.OnNoteRetrieved += NoteRetrievedDelegate;
            BillAcceptor.OnPauseCleared += PauseClearedDelegate;
            BillAcceptor.OnPauseDetected += PauseDetectedDelegate;
            BillAcceptor.OnPowerUpComplete += PowerUpCompleteDelegate;
            BillAcceptor.OnPowerUp += PowerUpDelegate;
            BillAcceptor.OnPUPEscrow += PUPEscrowDelegate;
            BillAcceptor.OnRejected += RejectedDelegate;
            BillAcceptor.OnReturned += ReturnedDelegate;
            //BillAcceptor.OnStacked += StackedDelegate;
            //A new STACKED event with document information has been added. Recommended to be used.
            BillAcceptor.OnStackedWithDocInfo += StackedWithDocInfoDelegate;
            BillAcceptor.OnStackerFullCleared += StackerFullClearedDelegate;
            BillAcceptor.OnStackerFull += StackerFullDelegate;
            BillAcceptor.OnStallCleared += StallClearedDelegate;
            BillAcceptor.OnStallDetected += StallDetectedDelegate;
        }

        public String DocInfoToString(DocumentType docType, IDocument doc)
        {
            if (docType == DocumentType.None)
                return "Doc Type: None";
            else if (docType == DocumentType.NoValue)
                return "Doc Type: No Value";
            else if (docType == DocumentType.Bill)
            {
                //return "Doc Type Bill = null";
                if (doc == null)
                    return "Doc Type Bill = null";
                else if (!BillAcceptor.CapOrientationExt)
                    return "Doc Type Bill = "
                        + doc.ToString()
                        + " (Classification: "
                        + BillAcceptor.EscrowClassification.ToString()
                        + ")";
                else
                    return "Doc Type Bill = "
                        + doc.ToString()
                        + " ("
                        + BillAcceptor.EscrowOrientation.ToString()
                        + ", Classification: "
                        + BillAcceptor.EscrowClassification.ToString()
                        + ")";
            }
            else if (docType == DocumentType.Barcode)
            {
                if (doc == null)
                    return "Doc Type Bar Code = null";
                else
                    return "Doc Type Bar Code = " + doc.ToString();
            }
            else if (docType == DocumentType.Coupon)
            {
                if (doc == null)
                    return "Doc Type Coupon = null";
                else
                    return "Doc Type Coupon = " + doc;
            }
            else
                return "Unknown Doc Type Error";
        }

        #region Event Handlers

        private void HandleCalibrateFinishEvent(object sender, EventArgs e)
        {
            events.Add("Event: Calibrate Finish.");
            Console.WriteLine(events.Last());
        }

        private void HandleCalibrateProgressEvent(object sender, EventArgs e)
        {
            events.Add("Event: Calibrate Progress.");
            Console.WriteLine(events.Last());
        }

        private void HandleCalibrateStartEvent(object sender, EventArgs e)
        {
            events.Add("Event: Calibrate Start.");
            Console.WriteLine(events.Last());
        }

        private void HandleCashBoxCleanlinessEvent(object sender, CashBoxCleanlinessEventArgs e)
        {
            events.Add("Event: Cashbox Cleanliness - {0}" + e.Value.ToString());
            Console.WriteLine(events.Last());
        }

        private void HandleCashBoxAttachedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Cassette Installed.");
            Console.WriteLine(events.Last());
        }

        private void HandleCashBoxRemovedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Cassette Removed.");
            Console.WriteLine(events.Last());
        }

        private void HandleCheatedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Cheat Detected.");
            Console.WriteLine(events.Last());
        }

        private void HandleClearAuditEvent(object sender, ClearAuditEventArgs e)
        {
            if (e.Success)
            {
                events.Add("Event: Clear Audit Complete: Success");
            }
            else
            {
                events.Add("Event: Clear Audit Complete: FAILED");
            }
            Console.WriteLine(events.Last());
        }

        private void HandleConnectedEvent(object sender, EventArgs e)
        {
            CloseBtn = true;
            CalibrateBtn = false;
            events.Add("Event: Connected.");
            Console.WriteLine(events.Last());
            //PopulateCapabilities();
            //PopulateBillSet();
            //PopulateBillValue();
            //PopulateProperties();
            //PopulateInfo();
            BillAcceptor.EnableAcceptance = true;
            BillAcceptor.AutoStack = false;
        }

        private void HandleDisconnectedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Disconnected.");
            Console.WriteLine(events.Last());
        }

        private void HandleDownloadFinishEvent(object sender, AcceptorDownloadFinishEventArgs e)
        {
            if (e.Success)
            {
                events.Add("Event: Download Finished: OK");
            }
            else
            {
                events.Add("Event: Download Finished: FAILED");
            }
            Console.WriteLine(events.Last());
        }

        private void HandleDownloadProgressEvent(object sender, AcceptorDownloadEventArgs e)
        {
            if (e.SectorCount % 100 == 0)
            {
                events.Add("Event: Download Progress:" + e.SectorCount.ToString());
                Console.WriteLine(events.Last());
            }
        }

        private void HandleDownloadRestartEvent(object sender, EventArgs e)
        {
            events.Add("Event: Download Restart.");
            Console.WriteLine(events.Last());
        }

        private void HandleDownloadStartEvent(object sender, AcceptorDownloadEventArgs e)
        {
            events.Add("Event: Download Start. Total Sectors: " + e.SectorCount.ToString());
            Console.WriteLine(events.Last());
        }

        private void HandleSendMessageErrorEvent(object sender, AcceptorMessageEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Event: Error in send message. ");
            sb.Append(e.Msg.Description);
            sb.Append("  ");

            foreach (byte b in e.Msg.Payload)
            {
                sb.Append(b.ToString("X2") + " ");
            }

            events.Add(sb.ToString());
            Console.WriteLine(events.Last());

            if (BillAcceptor.DeviceState == State.Escrow)
            {
                StackBtn = true;
                ReturnBtn = true;
            }
        }

        private void HandleEscrowedEvent(object sender, EventArgs e)
        {
            var data = DocInfoToString(BillAcceptor.DocType, BillAcceptor.getDocument());
            events.Add("Event: Escrowed: " + data);
            Console.WriteLine(events.Last());
            StackBtn = true;
            ReturnBtn = true;
            this.OnEscrow?.Invoke(this, data);
        }

        private void HandleFailureClearedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Device Failure Cleared. ");
            Console.WriteLine(events.Last());
        }

        private void HandleFailureDetectedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Device Failure Detected. ");
            Console.WriteLine(events.Last());
        }

        private void HandleInvalidCommandEvent(object sender, EventArgs e)
        {
            events.Add("Event: Invalid Command.");
            Console.WriteLine(events.Last());
        }

        private void HandleJamClearedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Jam Cleared.");
            Console.WriteLine(events.Last());
        }

        private void HandleJamDetectedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Jam Detected.");
            Console.WriteLine(events.Last());
        }

        private void HandleNoteRetrievedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Note Retrieved.");
            Console.WriteLine(events.Last());
        }

        private void HandlePauseClearedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Pause Cleared.");
            Console.WriteLine(events.Last());
        }

        private void HandlePauseDetectedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Pause Detected.");
            Console.WriteLine(events.Last());
        }

        private void HandlePowerUpCompleteEvent(object sender, EventArgs e)
        {
            events.Add("Event: Power Up Complete.");
            Console.WriteLine(events.Last());
        }

        private void HandlePowerUpEvent(object sender, EventArgs e)
        {
            events.Add("Event: Power Up.");
            Console.WriteLine(events.Last());
        }

        private void HandlePUPEscrowEvent(object sender, EventArgs e)
        {
            events.Add(
                "Event: Power Up with Escrow: "
                    + DocInfoToString(BillAcceptor.DocType, BillAcceptor.getDocument())
            );
            Console.WriteLine(events.Last());
            StackBtn = true;
            ReturnBtn = true;
        }

        private void HandleRejectedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Rejected.");
            Console.WriteLine(events.Last());
        }

        private void HandleReturnedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Returned.");
            Console.WriteLine(events.Last());
            StackBtn = false;
            ReturnBtn = false;
        }

        private void HandleStackedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Stacked");
            Console.WriteLine(events.Last());
            StackBtn = false;
            ReturnBtn = false;

            if (BillAcceptor.CapCashBoxTotal)
            {
                CashInBox = BillAcceptor.CashBoxTotal.ToString();
            }
        }

        private void HandleStackedWithDocInfoEvent(object sender, StackedEventArgs e)
        {
            var data = DocInfoToString(e.DocType, e.Document);
            events.Add("Event: StackedWithDocInfo: " + data);
            Console.WriteLine(events.Last());
            StackBtn = false;
            ReturnBtn = false;

            if (BillAcceptor.CapCashBoxTotal)
            {
                CashInBox = BillAcceptor.CashBoxTotal.ToString();
            }
            OnStack?.Invoke(this, data);
        }

        private void HandleStackerFullClearedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Cassette Full Cleared.");
            Console.WriteLine(events.Last());
        }

        private void HandleStackerFullEvent(object sender, EventArgs e)
        {
            events.Add("Event: Cassette Full.");
            Console.WriteLine(events.Last());
        }

        private void HandleStallClearedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Stall Cleared.");
            Console.WriteLine(events.Last());
        }

        private void HandleStallDetectedEvent(object sender, EventArgs e)
        {
            events.Add("Event: Stall Detected.");
            Console.WriteLine(events.Last());
        }

        #endregion


        // Open Bill Acceptor

        public string open_acceptor(string ComPortName = Global.DepositPort)
        {
            try
            {
                BillAcceptor.Open(ComPortName, PowerUp.A);
                return "Success";
            }
            catch (Exception err)
            {
                return "Unable to open the bill acceptor on com port <"
                    + ComPortName
                    + "> : "
                    + err.Message;
            }
        }

        // Close Bill Acceptor

        public string close_acceptor()
        {
            try
            {
                if (BillAcceptor.Connected)
                {
                    BillAcceptor.Close();
                    events.Clear();
                    Thread.Sleep(2000);
                }
                return "Success";
            }
            catch (Exception err)
            {
                return "Unable to close the bill acceptor on com port";
            }
        }

        // Escrow Stack

        public string ecrow_stack()
        {
            try
            {
                StackBtn = false;
                ReturnBtn = false;
                BillAcceptor.EscrowStack();
                return "Success";
            }
            catch (Exception err)
            {
                return "Unable to stack: " + err.Message;
            }
        }

        // Return Escrow Stack

        public string return_escrow_stack()
        {
            try
            {
                StackBtn = false;
                ReturnBtn = false;
                BillAcceptor.EscrowReturn();
                return "Success";
            }
            catch (Exception err)
            {
                return "Unable to return : " + err.Message;
            }
        }
    }
}
