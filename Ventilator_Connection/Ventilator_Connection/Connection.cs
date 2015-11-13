using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Ventilator_Connection
{
    abstract public class Connection
    {
        public delegate void DataReceivedHandler(string msg);

        protected string connectionState;

        public string ConnectionState
        {
            get { return connectionState; }
        }

        public abstract void Connect();
        public abstract bool Send(string msg);
        public abstract void Close();
    }

    public class BluetoothConnection : Connection
    {
        private SerialPort port;

        public string Portname
        {
            get
            {
                return port.PortName;
            }
            set
            {
                int portnum;

                if (value.StartsWith("COM") && int.TryParse(value.Substring(3), out portnum))
                    port.PortName = value;
                else
                    throw new ArgumentException();
            }
        }

        public BluetoothConnection(string portname)
        {
            this.port = new SerialPort();
            this.Portname = portname;
        }

        ///<summary>
        /// Tries to connect to the device, and sets the ConnectionState property:
        /// <para>Connection OK: "ConnectionUp"</para>
        /// <para>Connection failed: the message of the caught exception</para>
        ///</summary>
        public override void Connect()
        {
            port.BaudRate = 115200;
            port.DataBits = Convert.ToInt16(8);
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.Parity = Parity.None;
            try
            {
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                port.Open();
                if (port.IsOpen)
                {
                    connectionState = "ConnectionUp";
                }
            }
            catch (Exception ex)
            {
                connectionState = ex.Message;
            }
        }

        public override void Close()
        {
            if (port.IsOpen)
            {
                port.Close();
                connectionState = "ConnectionClosed";
            }
        }

        public event DataReceivedHandler DataReceived;

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string incomingdata = port.ReadExisting();

            //Event set
            DataReceived(incomingdata);
        }

        public override bool Send(string message)
        {
            if (port.IsOpen)
            {
                port.Write(message);
                return true;
            }
            else
                return false;
        }
    }

    public class WifiConnection : Connection
    {
        public override void Connect()
        {
            throw new NotImplementedException();
        }

        public override bool Send(string msg)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }
    }
}
