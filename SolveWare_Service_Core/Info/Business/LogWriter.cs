using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Info.Business
{
    public class LogWriter
    {
        private Queue<Log> logQueue;

        private string logBasedDir = "";

        private string logFilenName = "";

        private int maxLogAge = 5;

        private int queueSize = 1;

        private DateTime LastFlushed = DateTime.Now;

        private string headerString = "";

        private bool isTimeStampPrefixOnFileName = true;

        private bool isAppendFileCount = true;

        private bool isMonthlyBasedFolder = true;

        private DateTime lastWriteData = DateTime.Now;

        private string oldHeader = "";

        private int noOfLogFileCreatedInDay = 0;

        private bool IsInitSuccess { get; set; }

        public string HeaderString
        {
            get
            {
                return headerString;
            }
            set
            {
                headerString = value;
            }
        }

        public bool IsTimeStampPrefixOnFileName
        {
            get
            {
                return isTimeStampPrefixOnFileName;
            }
            set
            {
                isTimeStampPrefixOnFileName = value;
            }
        }

        public bool IsAppendFileCount
        {
            get
            {
                return isAppendFileCount;
            }
            set
            {
                isAppendFileCount = value;
            }
        }

        public bool IsMonthlyBasedFolder
        {
            get
            {
                return isMonthlyBasedFolder;
            }
            set
            {
                isMonthlyBasedFolder = value;
            }
        }

        public int NoOfLogFileCreatedInDay => noOfLogFileCreatedInDay;

        public void SetLogFileName(string fileName)
        {
            FlushLog();
            lock (logQueue)
            {
                if (fileName.LastIndexOf('.') == -1)
                {
                    fileName = fileName + fileName + ".txt";
                }

                logFilenName = fileName;
            }
        }

        public LogWriter()
        {
            IsInitSuccess = false;
            logQueue = new Queue<Log>();
        }

        public void InitLogFile(string basedDir, string fileName)
        {
            if (Directory.Exists(basedDir))
            {
                logBasedDir = basedDir;
                logFilenName = fileName;
                IsInitSuccess = true;
                return;
            }

            throw new Exception("<" + basedDir + ">Directory is not preset");
        }

        ~LogWriter()
        {
            FlushLog();
        }

        public void WriteToLog(string summary, string message)
        {
            lock (logQueue)
            {
                Log item = new Log(summary, message);
                logQueue.Enqueue(item);
                if (logQueue.Count >= queueSize || DoPeriodicFlush())
                {
                    FlushLog();
                }
            }
        }

        public void WriteToLog(string summary, string message, string logtime)
        {
            lock (logQueue)
            {
                Log item = new Log(summary, message, logtime);
                logQueue.Enqueue(item);
                if (logQueue.Count >= queueSize || DoPeriodicFlush())
                {
                    FlushLog();
                }
            }
        }

        public void WriteToLog(string message)
        {
            lock (logQueue)
            {
                Log item = new Log("", message);
                logQueue.Enqueue(item);
                if (logQueue.Count >= queueSize || DoPeriodicFlush())
                {
                    FlushLog();
                }
            }
        }

        public void WriteToLog(string message, bool onlymsg)
        {
            lock (logQueue)
            {
                Log item = new Log(message, onlymsg);
                logQueue.Enqueue(item);
                if (logQueue.Count >= queueSize || DoPeriodicFlush())
                {
                    FlushLog();
                }
            }
        }

        private bool DoPeriodicFlush()
        {
            if ((DateTime.Now - LastFlushed).TotalSeconds >= (double)maxLogAge)
            {
                LastFlushed = DateTime.Now;
                return true;
            }

            return false;
        }

        private void FlushLog()
        {
            if (logBasedDir == "")
            {
                logQueue.Clear();
                return;
            }

            string text = "";
            while (logQueue.Count > 0)
            {
                text = logBasedDir + "\\";
                if (!Directory.Exists(logBasedDir))
                {
                    Directory.CreateDirectory(logBasedDir);
                }

                if (isMonthlyBasedFolder)
                {
                    string text2 = DateTime.Now.ToString("yyyy-MMM");
                    string text3 = logBasedDir + "\\" + text2 + "\\";
                    if (!Directory.Exists(text3))
                    {
                        Directory.CreateDirectory(text3);
                    }

                    text = text3;
                }

                if ((DateTime.Now - lastWriteData).TotalDays >= 1.0)
                {
                    noOfLogFileCreatedInDay = 0;
                }

                lastWriteData = DateTime.Now;
                if (oldHeader != headerString)
                {
                    noOfLogFileCreatedInDay++;
                }

                if (noOfLogFileCreatedInDay <= 0)
                {
                    noOfLogFileCreatedInDay = 1;
                }

                Log log = logQueue.Dequeue();
                int num = logFilenName.LastIndexOf(".");
                string text4 = ((num == -1) ? ".txt" : logFilenName.Substring(num));
                string text5 = ((num == -1) ? logFilenName : logFilenName.Substring(0, num));
                string text6 = (isTimeStampPrefixOnFileName ? (log.LogDate + "-" + text5) : text5);
                string text7 = (isAppendFileCount ? (text6 + "_" + noOfLogFileCreatedInDay.ToString("D4")) : text6);
                string path = Path.Combine(text, text7 + text4);
                try
                {
                    bool flag = !File.Exists(path);
                    oldHeader = headerString;
                    FileStream stream = File.Open(path, FileMode.Append, FileAccess.Write);
                    StreamWriter streamWriter = new StreamWriter(stream);
                    if (flag && headerString != "")
                    {
                        streamWriter.WriteLine(headerString);
                    }

                    if (log.IsOnlyMessage)
                    {
                        streamWriter.WriteLine(log.Message);
                        continue;
                    }

                    if (log.Message == "")
                    {
                        streamWriter.WriteLine($"{log.LogTime}\t{log.Summary}");
                        continue;
                    }

                    streamWriter.WriteLine($"{log.LogTime}\t{log.Summary}");
                    streamWriter.WriteLine(string.Format("{0}\t{1}", "Detail", log.Message));
                }
                catch (Exception)
                {
                    Thread.Sleep(10);
                }
            }
        }
    }

    public class Log
    {
        public string Summary { get; set; }

        public string Message { get; set; }

        public string LogTime { get; set; }

        public string LogDate { get; set; }

        public bool IsOnlyMessage { get; set; }

        public Log(string summary, string message, string logTime)
        {
            IsOnlyMessage = false;
            Summary = summary;
            Message = message;
            LogDate = DateTime.Now.ToString("yyyy-MM-dd");
            LogTime = logTime;
        }

        public Log(string summary, string message)
        {
            IsOnlyMessage = false;
            Summary = summary;
            Message = message;
            LogDate = DateTime.Now.ToString("yyyy-MM-dd");
            LogTime = DateTime.Now.ToString("HH:mm:ss.fff");
        }

        public Log(string message, bool onlymsg)
        {
            IsOnlyMessage = onlymsg;
            Message = message;
            LogDate = DateTime.Now.ToString("yyyy-MM-dd");
            LogTime = DateTime.Now.ToString("HH-mm-ss-fff");
        }
    }
}
