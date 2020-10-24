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

        public async  void WriteAsync(string text)
        {
            
            sr.Write(text);
            sr.Flush();
        }
    }
}
