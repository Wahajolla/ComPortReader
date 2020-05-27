using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.IO;

namespace ComPortReader
{
    public partial class Form1 : Form
    {
        int rate= 9600;
        int dataBit=8;
        string comName = "";
        string path;
        Parity par = Parity.Even;
        StopBits stopBits = StopBits.One;
        SerialPort port = new SerialPort();
        StreamWriter sr;
        string s;
        public Form1()
        {
            InitializeComponent();
            DeviceCheck.RegisterUsbDeviceNotification(this.Handle);
            UpdatePorts(); 
        }
        private void SetUpPort(string path,string name, int  rate=9600, Parity par=Parity.None, int dataBit=8, StopBits stopBits=StopBits.One)
        {  
            try
            {
                port = new SerialPort(name, rate, par, dataBit, stopBits);
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                sr = File.CreateText(path + @"\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt");
                port.DtrEnable = true;
                port.Open();
                textBox1.Clear();
                textBox1.Text += string.Format("Настройки: Порт {0} , Бод {1} , Бит четности {2} , Биты данных {3}, Стоп биты {4}" + Environment.NewLine, comName, rate, par, dataBit, stopBits);
            }
            catch
            {
                textBox1.Clear();
                textBox1.Text += "Ошибка в выборе параметров" + Environment.NewLine;

            } 
        }
        private void port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if(port.IsOpen)
            {
                s = port.ReadExisting();
                sr.WriteLine(s);
            }
        }
        private void SetRate(object sender, EventArgs e)
        {
            rate = int.Parse(((ToolStripMenuItem)sender).Text); 
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == DeviceCheck.WmDevicechange)
            {
                UpdatePorts();
            }
        }
        private void UpdatePorts()
        {
            fToolStripMenuItem.DropDownItems.Clear();
            string[] ports = SerialPort.GetPortNames();
           
            foreach (string port in ports)
            {
              
                fToolStripMenuItem.DropDownItems.Add(port, null, new EventHandler(SetPort));
            }
        }
        void SetPort (object sender, EventArgs e)
        {
            comName = ((ToolStripMenuItem)sender).Text;
        }

        private void SetDataBit(object sender, EventArgs e)
        {
            ToolStripTextBox textBox = (ToolStripTextBox)sender;
            if (int.TryParse(textBox.Text, out dataBit))
            {
                dataBit = int.Parse(((ToolStripTextBox)sender).Text);
            }
            else
            {
                textBox.Text = new string(textBox.Text.Where(t => char.IsDigit(t)).ToArray());
            }
        }

        private void SetPar(object sender, EventArgs e)
        {
            ToolStripMenuItem element = (ToolStripMenuItem)sender;
            ToolStrip strip = element.GetCurrentParent();
      
            for (int i = 0; i < strip.Items.Count; i++)
            {
               if(strip.Items[i]== element)
                {
                    Debug.WriteLine(i);
                    par = (Parity)i;
                    return;
                }
            }
        }

        private void SetStop(object sender, EventArgs e)
        {
            ToolStripMenuItem element = (ToolStripMenuItem)sender;
            ToolStrip strip = element.GetCurrentParent();
            for (int i = 0; i < strip.Items.Count; i++)
            {
                if (strip.Items[i] == element)
                {


                    stopBits = (StopBits)(i+1);
                    return;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog filePath = new FolderBrowserDialog();
            if (filePath.ShowDialog() == DialogResult.OK)
            {
                path = filePath.SelectedPath;
                textBox2.Text = path;
            }
            else
            {
                textBox1.Text = "Путь не был выбран";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetUpPort(path, comName, rate, par, dataBit, stopBits);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            port.Close();
            sr.Flush();
            sr.Dispose();
            textBox1.Clear();
        }
    }
}
