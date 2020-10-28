using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using System.Collections.Specialized;
using System.Configuration;
using ComPortReader.Properties;

namespace ComPortReader
{


    public partial class RecorderWindow : Form
    {
        private delegate void SafeCallWrite(string text);
        private delegate void SafeCallClear();
        ComPortReader _comPortReader;
        AsyncWriter writer;
        int rate = 115200;
        int dataBit = 8;
        string comName = "";
        string path = Directory.GetCurrentDirectory();
        Parity par = Parity.Even;
        StopBits stopBits = StopBits.Two;

        public RecorderWindow()
        {
          
            InitializeComponent();
            DeviceCheck.RegisterUsbDeviceNotification(this.Handle);
            UpdatePorts();
            InitSettings();

        }
        private bool CheckVars()
        {
            if ((string.IsNullOrEmpty(path))) return false;
            if ((string.IsNullOrEmpty(comName))) return false;
            if (dataBit <= 5 || dataBit > 8) return false;
            return true;
        }
        private void OnStartRecord()
        {
            MarkerKeyLog logger = new MarkerKeyLog();
            _comPortReader?.Dispose();
            writer?.Dispose();
            _comPortReader = new ComPortReader(comName, rate, par, dataBit, stopBits);
            _comPortReader.InitPort();
            writer = new AsyncWriter(path);
            _comPortReader.OnGetCallback(writer.WriteAsync);
            _comPortReader.OnGetCallback(OnShowText);
            logger.SetAction(writer.ChangeMark);
            SaveSettings();




        }

        public void InitSettings()
        {
            comName = Properties.Settings.Default.comName;
            rate = Settings.Default.rate;
            par = (Parity)Settings.Default.par;
            dataBit = Settings.Default.dataBit;
            stopBits = (StopBits) Settings.Default.stopBits;
            path = Settings.Default.path;
            
        }

        public void SaveSettings()
        {
            Settings.Default.comName = comName;
            Settings.Default.rate = rate;
            par = (Parity)Settings.Default.par;
            Settings.Default.dataBit = dataBit;
            stopBits = (StopBits) Settings.Default.stopBits;
            Settings.Default.path = path;
            Properties.Settings.Default.Save();  
            Properties.Settings.Default.Upgrade();  
            Properties.Settings.Default.Reload();  
        }

        public void ClearText()
        {
            if (textBox1.InvokeRequired)
            {
                var d = new SafeCallClear(ClearText);
                textBox1.Invoke(d);
            }
            else
            {
                textBox1.Clear();
            }
        }

        public void OnShowText(string text)
        {
            if (textBox1.InvokeRequired)
            {
                var d = new SafeCallWrite(OnShowText);
                textBox1.Invoke(d, new object[] { text });
            }
            else
            {
                if (textBox1.Lines.Length > 20)
                {
                    textBox1.Clear();
                }
                textBox1.Text += text + Environment.NewLine;
            }
        }

        private void OnSetRate(object sender, EventArgs e)
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

                fToolStripMenuItem.DropDownItems.Add(port, null, new EventHandler(OnSetPort));
            }
        }
        void OnSetPort(object sender, EventArgs e)
        {
            comName = ((ToolStripMenuItem)sender).Text;
        }

        private void OnSetDataBit(object sender, EventArgs e)
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

        private void OnSetParityBit(object sender, EventArgs e)
        {
            ToolStripMenuItem element = (ToolStripMenuItem)sender;
            ToolStrip strip = element.GetCurrentParent();

            for (int i = 0; i < strip.Items.Count; i++)
            {
                if (strip.Items[i] == element)
                {
                    par = (Parity)i;
                    return;
                }
            }
        }

        private void OnSetStopBits(object sender, EventArgs e)
        {
            ToolStripMenuItem element = (ToolStripMenuItem)sender;
            ToolStrip strip = element.GetCurrentParent();
            for (int i = 0; i < strip.Items.Count; i++)
            {
                if (strip.Items[i] == element)
                {


                    stopBits = (StopBits)(i + 1);
                    return;
                }
            }
        }

        private void OnChangePath(object sender, EventArgs e)
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
        private void OnStop(object sender, EventArgs e)
        {
            OnShowText(Environment.NewLine + "Запись остановлена" + Environment.NewLine);
            ClearText();
            _comPortReader?.Dispose();
            writer?.Dispose();
        }

        private async void OnStartRecord(object sender, EventArgs e)
        {

            textBox1.Clear();
            if (!CheckVars())
            {
               
                textBox1.Text += string.Format("Неверные настройки" + Environment.NewLine);
                return;

            }
            if (ComPortReader.IsBusy(comName))
            {
                textBox1.Text += string.Format("Порт " + comName + " занят" + Environment.NewLine);
                return;
            }


            textBox1.Text += string.Format("Настройки: Порт: {0} , Бод: {1} , Бит четности: {2} , Биты данных: {3}, Стоп биты: {4}" + Environment.NewLine, comName, rate, par.ToFriendlyString(), dataBit, stopBits.ToFriendlyString());
                await Task.Run(() => OnStartRecord());
               
            





          
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
