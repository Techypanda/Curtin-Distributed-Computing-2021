using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Business_Webservice.Models
{
    public class LogFile
    {
        private static string path = $"{HttpRuntime.AppDomainAppPath}";
        public static void Log(string content)
        {
            bool waitingOnFIle = false;
            StreamWriter sw = null;
            if (File.Exists($"{path}/{DateTime.Today.ToString("dd-MM-yyyy")}-LogFile.txt"))
            {
                while (!waitingOnFIle)
                {
                    try
                    {
                        sw = new StreamWriter($"{path}/{DateTime.Today.ToString("dd-MM-yyyy")}-LogFile.txt", true);
                        waitingOnFIle = true;
                    }
                    catch (IOException)
                    { }
                }
            } else
            {
                while (!waitingOnFIle)
                {
                    try
                    {
                        sw = new StreamWriter($"{path}/{DateTime.Today.ToString("dd-MM-yyyy")}-LogFile.txt");
                        waitingOnFIle = true;
                    }
                    catch (IOException) { }
                }
            }
            sw.WriteLine($"{DateTime.Now} - {content}");
            sw.Close();
        }
    }
}