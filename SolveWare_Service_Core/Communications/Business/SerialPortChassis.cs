using SolveWare_Service_Core.Communications.Base.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Communications.Business
{
    public class SerialPortChassis : InstrumentChassisBase, IDisposable
    {
        private int _dataBits = 1;
        private int _baudRate = 9600;
        private int _timeout_ms = 60000;
        private string _portName;
        private SerialPort _port;
        private Parity _parity = Parity.None;
        private StopBits _stopBits = StopBits.One;

        public bool RtsEnable
        {
            get
            {
                if (this.IsOnline)
                {
                    return this._port.RtsEnable;
                }
                return false;
            }
            set
            {
                if (this.IsOnline)
                {
                    this._port.RtsEnable = value;
                }
            }
        }

        #region ctor
        public SerialPortChassis(string name, string resource, bool isOnline) : base(name, resource, isOnline)
        {
            this.Name = name;
            this.IsOnline = isOnline;
            this.Resource = resource;

            string[] props = resource.Split(',');
            this._portName = props[0].ToUpper(); ;
            this._baudRate = Convert.ToInt32(props[1]);
            this._dataBits = Convert.ToInt16(props[2]);
            this._parity = (Parity)Enum.Parse(typeof(Parity), props[3]);
            this._stopBits = (StopBits)Enum.Parse(typeof(StopBits), props[4]);

            this._port = new SerialPort();
            this._port.PortName = this._portName;
            this._port.BaudRate = this._baudRate;
            this._port.StopBits = this._stopBits;
            this._port.Parity = this._parity;
            this._port.DataBits = this._dataBits;
            this._port.ReadTimeout = this.Timeout_ms;
            this._port.WriteTimeout = this.Timeout_ms;
        }
        #endregion

        public override void Initialize()
        {
            if (this.IsOnline)
            {
                this.OpenPort();
            }
        }
        public byte[] ReadBytes()
        {
            this.OpenPort();
            int bufLen = this._port.BytesToRead;
            byte[] buffer = new byte[bufLen];
            this._port.Read(buffer, 0, bufLen);

            return buffer;
        }
        public byte[] ReadBytes(int bufferLength)
        {
            this.OpenPort();
            int bufLen = this._port.BytesToRead;
            byte[] buffer = new byte[bufLen];
            this._port.Read(buffer, 0, bufLen);

            byte[] result;
            if (bufLen <= bufferLength)
            {
                result = buffer;
            }
            else
            {
                byte[] desBytes = new byte[bufferLength];
                Array.Copy(buffer, desBytes, bufferLength);
                result = desBytes;
            }

            return result;
        }
        public override string ReadString()
        {
            lock (this)
            {
                this.OpenPort();
                string result = this._port.ReadExisting();

                if (this._port.BytesToRead > 0)
                {
                    byte[] tempBuff = new byte[this._port.BytesToRead];
                    this._port.Read(tempBuff, 0, tempBuff.Length);
                }

                return result;
            }
        }
        public void Dispose()
        {
            this.OnDispose();
        }
        public override void Initialize(int timeout)
        {
            this.Timeout_ms = timeout;
            this.OpenPort();
        }
        public override void Turn_On_Line(bool isOnline)
        {
            this.IsOnline = isOnline;
        }
        public override void Turn_On_Simulation()
        {
            this.IsOnline = false;
        }
        public override void TryWrite(string cmd)
        {
            lock (this)
            {
                this.OpenPort();
                this.ClearBuffer();
                string[] tempArr = cmd.Split(';');
                StringBuilder strb = new StringBuilder();
                for (int i = 0; i < tempArr.Length; i++)
                {
                    string temp = tempArr[i].EndsWith("\n") ? tempArr[i] : tempArr[i] + "\n";
                    temp += ";";
                    strb.Append(temp);
                }
                cmd = strb.ToString().TrimEnd(';');
                Console.WriteLine(cmd);
                this._port.Write(cmd);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (this._port.BytesToWrite > 0 && sw.Elapsed.TotalMilliseconds < this._timeout_ms)
                {
                    Thread.Sleep(5);
                }
                sw.Stop();

                if (this._port.BytesToWrite > 0)
                {
                    string errMsg = string.Format("Write command: [{0}] time out via resource [{1}]", cmd, this.Resource);
                    throw new Exception(errMsg);
                }
            }
        }
        public override void TryWrite(byte[] cmd)
        {
            this.OpenPort();
            this._port.Write(cmd, 0, cmd.Length);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (this._port.BytesToWrite > 0 && sw.Elapsed.TotalMilliseconds < this._timeout_ms)
            {
                Thread.Sleep(5);
            }
            if (this._port.BytesToWrite > 0)
            {
                string errMsg = string.Format("Write command: [{0}] time out via resource [{2}]", cmd.ToString(), this.Resource);
                throw new Exception(errMsg);
            }
        }
        public override byte[] Query(byte[] cmd, int bytesToRead, int delay_ms)
        {
            this.OpenPort();
            this._port.ReadExisting();
            this.TryWrite(cmd);
            bool isDataReceived = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (!isDataReceived)
            {
                if (this._port.BytesToRead > 0)
                {
                    isDataReceived = true;
                }
                Thread.Sleep(50);
                if (sw.Elapsed.TotalMilliseconds > this.Timeout_ms)
                {
                    sw.Stop();
                    string errMsg = string.Format("Query command: [{0}] time out via resource [{1}]", cmd, this.Resource);
                    throw new Exception(errMsg);
                }
            }
            return this.ReadBytes(bytesToRead);
        }
        public override string Query(string cmd, int delay_ms)
        {
            lock (this)
            {
                this.OpenPort();
                //this.ClearBuffer();
                this._port.ReadExisting(); //清空接收区
                this.TryWrite(cmd);
                bool isDataReceived = false;

                Thread.Sleep(50);  //加速接收
                Stopwatch sw = new Stopwatch();

                int count1 = 0;
                int count2 = 0;

                sw.Start();
                while (!isDataReceived)
                {
                    //Debug.Print("count1=" + count1.ToString() + ";count2=" + count2.ToString());

                    count1 = this._port.BytesToRead;
                    if (count1 > 0)
                    {
                        if (count1 == count2)
                        {
                            isDataReceived = true;
                            break;
                        }
                        else
                        {
                            count2 = count1;
                        }

                    }
                    Thread.Sleep(50);
                    if (sw.Elapsed.TotalMilliseconds > this.Timeout_ms)
                    {
                        sw.Stop();
                        string errMsg = string.Format("Query command: [{0}] time out via resource [{1}]", cmd, this.Resource);
                        Console.WriteLine(errMsg);
                        break;
                        //调试总抛异常,烦死了,注释掉
                        //throw new Exception(errMsg);
                    }
                }

            }
            return this.ReadString();
        }
        public override byte[] Query(byte[] cmd, int delay_ms)
        {
            this.OpenPort();
            this.TryWrite(cmd);
            bool isDataReceived = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!isDataReceived)
            {
                if (this._port.BytesToRead > 0)
                {
                    isDataReceived = true;
                }
                Thread.Sleep(1);
                if (sw.Elapsed.TotalMilliseconds > this.Timeout_ms)
                {
                    sw.Stop();
                    string errMsg = string.Format("Query command: [{0}] time out via resource [{1}]", cmd.ToString(), this.Resource);
                    throw new Exception(errMsg);
                }
            }
            return this.ReadBytes();
        }

        private void OpenPort()
        {
            if (!this._port.IsOpen)
            {
                this._port.Open();
            }
        }
        private void OnDispose()
        {
            if (this._port != null && this._port.IsOpen)
            {
                this._port.Close();
            }
        }
        private void ClearBuffer()
        {
            if (this._port.IsOpen)
            {
                this._port.DiscardInBuffer();
                this._port.DiscardOutBuffer();
            }
        }
    }
}
