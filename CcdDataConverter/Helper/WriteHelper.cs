using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CcdDataConverter.Model;

namespace CcdDataConverter.Helper
{
    class WriteHelper
    {
        const string _logPath = @"log\";
        static object lockMe = new object();

        public static void Log(LogFormat log, string fileName)
        {
            string filePath = string.Format("{0}{1}.csv", _logPath, fileName);
            bool fileExists = false;
            lock (lockMe)
            {
                if (File.Exists(filePath))
                {
                    fileExists = true;
                }
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    if (!fileExists)
                    {
                        sw.WriteLine("IN DATE TIME,FlawID,FlawName,Flaw Y value(M),Flaw X value(M),JobID,OUT DATA,TYPE,Flaw Y value(mm),Flaw X value(mm),OUT DATA TIME,STATUS");
                    }
                    string output = string.Format("{0},{1},,{2},{3},{4}",
                        log.InputDateTime,
                        log.InputData,
                        log.OutputData,
                        log.OutputDateTime,
                        log.Status);
                    sw.WriteLine(output);
                    sw.Close();
                }
            }
        }
    }
}
