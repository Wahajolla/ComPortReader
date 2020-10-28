using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ComPortReader
{
    public class MarkerKeyLog
    {
        public Action OnSetMark;  
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(
            System.Windows.Forms.Keys vKey); 

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(
            System.Int32 vKey);
        
        private System.String keyBuffer;
        private System.Timers.Timer timerKeyMine;
        private System.Timers.Timer timerBufferFlush;
        
        private void timerKeyMine_Elapsed(object sender, 
            System.Timers.ElapsedEventArgs e)
        {

                if(GetAsyncKeyState(32) == -32767)
                {
                    OnSetMark?.Invoke();
                }
            
        }

        public void SetAction(Action OnSetMark)
        {
            this.OnSetMark = OnSetMark;
        }
        
        
        public MarkerKeyLog()
        {
            keyBuffer = "";

          
            this.timerKeyMine = new System.Timers.Timer();
            this.timerKeyMine.Enabled = true;
            this.timerKeyMine.Elapsed += new System.Timers.ElapsedEventHandler(this.timerKeyMine_Elapsed);
            this.timerKeyMine.Interval = 10;
			
      
        }

        
        #region Properties
        public System.Boolean Enabled
        {
            get
            {
                return timerKeyMine.Enabled && timerBufferFlush.Enabled;
            }
            set
            {
                timerKeyMine.Enabled = timerBufferFlush.Enabled = value;
            }
        }

        public System.Double FlushInterval
        {
            get
            {
                return timerBufferFlush.Interval;
            }
            set
            {
                timerBufferFlush.Interval = value;
            }
        }

        public System.Double MineInterval
        {
            get
            {
                return timerKeyMine.Interval;
            }
            set
            {
                timerKeyMine.Interval = value;
            }
        }
        #endregion
    }
}