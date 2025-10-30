using System.IO.Ports;
using MobileWallet.Desktop.API;
using Python.Runtime;

namespace MobileWallet.Desktop.Atm
{
    public class PythonDispenser
    {

        private String _latestResponse = "";

        // Initialize the hardware
        public bool Initialize()
        {
            try
            {
                string cassetteStatus = ReadCassette();
                App.AppLogger.Debug("Cassette returned status: {}",cassetteStatus);
                return true;
            }
            catch (Exception e)
            {
                App.AppLogger.Error(e, e.Message);
                Console.Write(e);
            }
            return false;
        }

        public string RunCommand(string portName, string message)
        {
            using (Py.GIL())
            {
                // Import the Python module
                dynamic pyModule = Py.Import("app_serial");
                // Define the port and command bytes
                // Call the main function from Python script
                _ = App.LogError("Request: " + message, LogType.Withdraw);
                App.AppLogger.Debug("started python serial communication ");
                string response = pyModule.main(portName, message + '\r');
                App.AppLogger.Debug("ended serial port communication");
                _ = App.LogError("Response: " + response, LogType.Withdraw);
                if (!string.IsNullOrWhiteSpace(response))
                {
                    _latestResponse = response;
                    return response;
                }
            }
            return "";
        }
        private static void CheckPortsOnSystem(string port)
        {
            //testing to see if the port used is found on the system
            try
            {
                string[] ports_on_system = SerialPort.GetPortNames();
                if (!ports_on_system.Contains(port))
                {
                    throw new ArgumentException($"Port {port} not available on this system");
                }
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
            }
        }
        private void SerialPortOnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine("Received Error: " + e.EventType.ToString());
            App.AppLogger.Error("Error at the level of serial port");
        }

        public SerialPort? Initialize(
            string portName,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.Even
        )
        {
            try
            {
                var serialPort = new SerialPort(portName, baudRate, parity);
                serialPort.Handshake = Handshake.None;
                serialPort.DataBits = dataBits;
                serialPort.StopBits = StopBits.One;
                serialPort.Open();
                return serialPort;
            }
            catch (Exception e)
            {
                App.AppLogger.Error(e, e.Message);
                Console.Write(e);
            }
            return null;
        }

        static void SendCommand(SerialPort serialPort, string hexCommand)
        {
            // Convert hex string to byte array
            byte[] commandBytes = HexStringToByteArray(hexCommand);
            // Send command to the control board
            serialPort.Write(commandBytes, 0, commandBytes.Length);
            Console.WriteLine("Sent: " + hexCommand);
        }

        static string ReadResponse(SerialPort serialPort)
        {
            // Wait for response data
            Thread.Sleep(100); // Adjust this delay if necessary

            int bytesToRead = serialPort.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            serialPort.Read(buffer, 0, bytesToRead);

            // Convert response bytes to hex string
            return BitConverter.ToString(buffer).Replace("-", " ");
        }

        static byte[] HexStringToByteArray(string hex)
        {
            string[] hexValues = hex.Split(' ');
            byte[] bytes = new byte[hexValues.Length];
            for (int i = 0; i < hexValues.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexValues[i], 16);
            }
            return bytes;
        }

        public bool StartShutter()
        {
            SerialPort? serialPort = null;
            try
            {
                serialPort = Initialize(Global.SHUTTER_COMMUNICATION_PORT, 19200, 8, Parity.None);
                if (serialPort == null)
                {
                    return false;
                }

                string command = "10 02 03 00 43 37 01 10 03 76";
                SendCommand(serialPort, command);
                var response = ReadResponse(serialPort);
                if (response == "10 06")
                {
                    command = "10 05";
                    Thread.Sleep(2000);
                    SendCommand(serialPort, command);
                    response = ReadResponse(serialPort);
                    if (response.Contains("10 02 50 02 00 37 01 10 03 34"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                App.AppLogger.Error(e, e.Message);
                Console.WriteLine(e);
            }
            finally { }

            return false;
        }

        public bool StopShutter()
        {
            SerialPort? serialPort = null;
            try
            {
                serialPort = Initialize(Global.SHUTTER_COMMUNICATION_PORT, 19200, 8, Parity.None);
                if (serialPort == null)
                {
                    return false;
                }
                string command = "10 02 03 00 43 38 01 10 03 79";
                SendCommand(serialPort, command);
                var response = ReadResponse(serialPort);
                if (response == "10 06")
                {
                    command = "10 05";
                    SendCommand(serialPort, command);
                    response = ReadResponse(serialPort);
                    if (response == "10 02 50 02 00 38 01 10 03 3B")
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
                Console.WriteLine(ex);
            }
            finally { }

            return false;
        }

        /// <summary>
        /// Open Cassette Command If Null means Success else Error
        /// </summary>
        /// <returns></returns>
        public string? OpenCassette()
        {
            try
            {
                string command = LrcCalculate("8");
                WriteToSerialPort(command);
                string r = DisplayResult(_latestResponse);
                return r;
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
                return ex.Message;
            }
        }

        /// <summary>
        /// Close Cassette Command If Null means Success else Error
        /// </summary>
        /// <returns></returns>
        public string? CloseCassette()
        {
            try
            {
                string command = LrcCalculate("7");
                WriteToSerialPort(command + Environment.NewLine);
                return DisplayResult(_latestResponse);
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
                return ex.Message;
            }
        }

        /// <summary>
        /// Reset Command If Null means Success else Error
        /// </summary>
        /// <returns></returns>
        public string? Reset()
        {
            try
            {
                string command = LrcCalculate("0");
                WriteToSerialPort(command);
                return DisplayResult(_latestResponse);
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
                return ex.Message;
            }
        }

        // Take cash from cassettes

        public string? move_forward(
            string cassette1,
            string cassette2,
            string cassette3,
            string cassette4
        )
        {
            try
            {
                string commandString = "20";
                if (cassette1 != "0")
                {
                    commandString = commandString + "1" + cassette1.PadLeft(3, '0');
                }
                if (cassette2 != "0")
                {
                    commandString = commandString + "2" + cassette2.PadLeft(3, '0');
                }
                if (cassette3 != "0")
                {
                    commandString = commandString + "3" + cassette3.PadLeft(3, '0');
                }
                if (cassette4 != "0")
                {
                    commandString = commandString + "4" + cassette4.PadLeft(3, '0');
                }
                string command = LrcCalculate(commandString);
                WriteToSerialPort(command);
                return DisplayResult(_latestResponse);
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
                return ex.Message;
            }
        }

        // Deliver Notes
        public string? deliver_notes()
        {
            try
            {
                string command = LrcCalculate("3");
                WriteToSerialPort(command + Environment.NewLine);
                return DisplayResult(_latestResponse);
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
                return ex.Message;
            }
        }

        private static string LrcCalculate(string message)
        {
            Console.WriteLine("Command String: " + message);
            // Step 1: Calculate the "exclusive or" of all characters in the string
            byte xorResult = 0;
            foreach (char character in message)
            {
                xorResult ^= Convert.ToByte(character);
            }

            // Step 2: Divide the hexadecimal value by 0x10 and truncate the result
            byte y = (byte)(xorResult / 0x10);

            // Step 3: Calculate the "logical and" between the result and 0x0F
            byte z = (byte)(xorResult & 0x0F);

            // Step 4: Add 0x30 to the last two values to get L1 and L2
            byte l1 = (byte)(y | 0x30);
            byte l2 = (byte)(z | 0x30);

            // Concatenate the original message with L1 and L2
            string finalOutput = message + Convert.ToChar(l1) + Convert.ToChar(l2);

            return finalOutput;
        }
 public string? DisplayResult(string response) 
        {
            if (string.IsNullOrWhiteSpace(response)) return "Invalide response (empty or null)";
            // _latestResponse = string.Empty;
            return (char)response[0] switch
            {

                '0' => null,
                '1' => null, //"Low level",
                '2' => "Empty Cassette",
                '3' => "Machine not opened",
                '4' => "Rejected Notes",
                '5' => "Diverter Failure",
                '6' => "Failure to feed",
                '7' => "Transmission error",
                '8' => "Illegal Communication or Communication sequence",
                '9' => "Jam in note qualifier",
                ':' => "NC not present or properly inserted",
                '<' => "No notes retracted",
                '?' => "RV not present or Properly inserted",
                '@' => "Delivery failure",
                'A' => "Reject Failure",
                'B' => "Too many notes req",
                'C' => "Jam in note feeder transport",
                'D' => "Reject vault almost full",
                'E' => "Cassette internal failure",
                'F' => "Main Motor Failure",
                'G' => "Rejected Cheque",
                'I' => "Note Qualifier Faulty",
                'J' => "NF exit sensor failure",
                'K' => "Shutter failure",
                'M' => "Notes in bundle output unit",
                'N' => "Communication timeout",
                'P' => "Shutter failure",
                'Q' => "Cassette not Identified",
                'W' => "Error in throat",
                '[' => "Sensor error",
                '\'' => "NMD Internal Failure",
                'a' => "Cassette Lock Faulty",
                'b' => "Error in notes stacking area",
                'c' => "Module need service",
                'e' => "No Message to resend",
                'p' => "Cassette out error",
                _ => "Unhandled status error"

            };

        }

       
        private bool WriteToSerialPort(string command)
        {
            try
            {
                App.AppLogger.Debug($"Started running serial port command on port : {Global.CASH_DISPENSER_COMMUNICATION_PORT}" );
                RunCommand(Global.CASH_DISPENSER_COMMUNICATION_PORT, command);
                App.AppLogger.Debug("Finished running serial port command");
                return true;
                // if (_serialPort != null)
                // {
                //     _serialPort.DiscardInBuffer();
                //     _serialPort.DiscardOutBuffer();
                //     _serialPort.WriteLine(command);
                //     return true;
                // }
            }
            catch (Exception e)
            {
                App.AppLogger.Error(e, e.Message);
                Console.WriteLine(e);
            }
            return false;
        }

        public string? ReadCassette()
        {
            App.AppLogger.Debug("Started cassette read function (Dispenser.cs:PythonDispenser.ReadCassette())");
            try
            {
                string command = LrcCalculate("5");
                WriteToSerialPort(command);
                return DisplayResult(_latestResponse);
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex, ex.Message);
                return ex.Message;
            }
        }

        public List<CassetteDTO> ReadCassetteStatus()
        {
            App.AppLogger.Debug("Started cassette read function (Dispenser.cs:PythonDispenser.ReadCassette())");
            try
            {
                if (!Global.UseHardware)
                {
                    return new List<CassetteDTO>()
                    {
                        new CassetteDTO()
                        {
                            Id = "1",
                            No = 1,
                            Status = "No Issues",
                        },
                        new CassetteDTO()
                        {
                            Id = "1",
                            No = 2,
                            Status = "No Issues",
                        },
                        new CassetteDTO()
                        {
                            Id = "1",
                            No = 3,
                            Status = "No Issues",
                        },
                    };
                }
                string command = LrcCalculate("5");
                App.AppLogger.Debug("Started writing to serial port");
                WriteToSerialPort(command);
                App.AppLogger.Debug("Finished writing to serial port");
                //1001234511146872100100310100A4<
                App.AppLogger.Debug("Started Decoding cassette status");
                return CassetteStatusDecoder.DecodeCassetteStatus(_latestResponse);
            }
            catch (Exception ex)
            {
                _ = App.LogError(ex.Message, error: ex.StackTrace);
            }
            return new List<CassetteDTO>();
        }
    }


}
