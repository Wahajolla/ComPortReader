using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComPortReader
{
    class ComPort : IDisposable
    {
        public CancellationTokenSource source = new CancellationTokenSource();
        public CancellationToken token;
        public Action<string> OnGetRecord; 
        SerialPort port = new SerialPort();
        public ComPort(string name, int rate = 115200, Parity par = Parity.Even, int dataBit = 8, StopBits stopBits = StopBits.Two)
        {
            port = new SerialPort(name, rate, par, dataBit, stopBits);
           
        }

        public void InitPort()
        {
            token = source.Token;
            port.DtrEnable = true;
            port.Encoding = Encoding.UTF8;
            port.DataReceived += OnRecordRecived;
            port.ReadBufferSize = 12000;
            port.Open();
        }
         
        public static bool IsBusy(string name)
        {
            bool isBusy = true;
            SerialPort sp;
            try
            {
                sp = new SerialPort(name);
                sp.Open();
                sp.Dispose();
                isBusy = false;
            }
            catch
            {
                
            }
            return isBusy;
        }

        private void OnRecordRecived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            
                if (port.IsOpen)
                {
                try
                {
                    string s = port.ReadLine();
                    OnGetRecord.Invoke(s);
                }
                catch
                {

                }
                }
            

        }

        public void OnGetCallback(Action<string> method)
        {
            OnGetRecord += method; 
        }

        public void Dispose()
        {
            OnGetRecord = delegate { };
            source.Cancel();
            Thread.Sleep(100);
            port.Close();
            port.Dispose();
           
        }
    }
}
