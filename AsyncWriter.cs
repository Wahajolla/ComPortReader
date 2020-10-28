using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComPortReader
{
    class AsyncWriter : IDisposable
    {
        StreamWriter sr;
        string path = Directory.GetCurrentDirectory();
        string totalpath;
        private bool isMarked = true;
        public void Dispose()
        {
            sr.Close();
            sr.Dispose();
        }

        public AsyncWriter(string path)
        {
            totalpath = path + @"\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
            sr = new StreamWriter(totalpath, false, Encoding.UTF8);
        }

        public AsyncWriter()
        {
            totalpath = path + @"\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
            sr = new StreamWriter(totalpath, false, Encoding.UTF8);
        }
           
        public void ChangeMark()
        {
            isMarked = true;
        }


        public async  void WriteAsync(string text) 
        {
            string infoMark = "";
            text = text.Replace("\r\n", @"\r\n");
            text =  text.Replace("\n", @"\n");
            text =  text.Replace("\r", @"\r");
            infoMark = ";" + DateTime.Now.ToString("yyyy:MM:dd:hh:mm:ss.fff") + ";";
            if (isMarked)
            {
                infoMark += "*****";
                isMarked = !isMarked;
            }
           
           await sr.WriteLineAsync(text + infoMark);
           await sr.FlushAsync();
        }
    }
}
