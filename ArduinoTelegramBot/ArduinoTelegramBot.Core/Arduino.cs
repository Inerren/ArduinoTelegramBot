using RJCP.IO.Ports;
using System;

namespace ArduinoTelegramBot.Core
{
    public class Arduino: IDisposable
    {
        private SerialPortStream _serialPort;

        public event Action<string> DataReceived;

        /// <summary>
        /// Class to work with Arduino
        /// </summary>
        /// <param name="comPort">COM-port number</param>
        /// <param name="baudRate">COM-port baud rate</param>
        public Arduino(string comPort, int baudRate = 9600)
        {
            if (String.IsNullOrWhiteSpace(comPort))
            {
                throw new ArgumentNullException("comPort", "Need to COM-port number to initialize.");
            }

            _serialPort = new SerialPortStream(comPort, baudRate);
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort.DataReceived += SerialPort_DataReceived;
        }

        /// <summary>
        /// Initializes work of Arduino
        /// </summary>
        public void Initialize()
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
        }

        /// <summary>
        /// Method for sending commands to Arduino
        /// </summary>
        /// <param name="command">Command to send</param>
        public void SendCommand(string command)
        {
            if (String.IsNullOrWhiteSpace(command)) return;

            if (!command.EndsWith("\n"))
            {
                command += "\n";
            }

            if (_serialPort.IsOpen)
            {
                _serialPort.Write(command);
            }
        }

        /// <summary>
        /// Data Received from Arduino event
        /// </summary>
        /// <param name="sender">Serial port - sender</param>
        /// <param name="e">Event arguments</param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _serialPort = (SerialPortStream)sender;
            string message = _serialPort.ReadExisting();

            if (!String.IsNullOrEmpty(message) && DataReceived != null)
            {
                DataReceived(message);
            }
        }

        public void Dispose()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            _serialPort.Dispose();
        }
    }
}
