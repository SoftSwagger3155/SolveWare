using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SolveWare_Service_Core.Info.Business
{
    public class ErrorEventManager : DispatcherObject
    {
        private LogWriter errorLogWritter;

        private LogWriter eventLogWritter;

        private LogWriter dataLogWritter;

        private Dictionary<uint, string> ErrorList = new Dictionary<uint, string>();

        private Dictionary<uint, string> EventList = new Dictionary<uint, string>();

        private string DefinationExtention = ".dat";

        private string ErrorDefinationFileName = "";

        private string EventDefinationFileName = "";

        private string errorFileName = "";

        private string writeEventFileName = "";

        private string dataLogFileName = "";

        private string LogFileExtension = ".txt";

        private int NoOfItemDisplayOnList = 100;

        private string ErrorEventDefinationFolderName = "Definitions";

        public ObservableCollection<ErrorEventItem> LstMsgEvent = new ObservableCollection<ErrorEventItem>();

        private object addEventSyncObj = new object();

        private bool IsToAddtoList = false;

        private bool hasToWriteEventLogFile = false;

        private bool hasToWriteErrorLogFile = false;

        private string logRoot = "";

        private object lck = new object();

        public bool HasToWriteEventLogFile
        {
            get
            {
                return hasToWriteEventLogFile;
            }
            set
            {
                hasToWriteEventLogFile = value;
            }
        }

        public bool HasToWriteErrorLogFile
        {
            get
            {
                return hasToWriteErrorLogFile;
            }
            set
            {
                hasToWriteErrorLogFile = value;
            }
        }

        public bool IsInitializationDone { get; set; }

        public string LogRoot => logRoot;

        public ErrorEventManager(string errorFileName = "error", string eventFileName = "event", bool addToList = true, bool writetoFile = true)
        {
            writeEventFileName = "event";
            dataLogFileName = "data";
            IsToAddtoList = addToList;
            hasToWriteEventLogFile = writetoFile;
            hasToWriteErrorLogFile = writetoFile;
            ErrorDefinationFileName = errorFileName;
            EventDefinationFileName = eventFileName;
            errorLogWritter = new LogWriter();
            eventLogWritter = new LogWriter();
            dataLogWritter = new LogWriter();
        }

        public string Init(string definationFolder, string logRoot)
        {
            IsInitializationDone = false;
            try
            {
                this.logRoot = logRoot;
                InitEventErrorList(definationFolder);
                if (logRoot != "")
                {
                    string text = Path.Combine(logRoot, "ErrorLog");
                    string text2 = Path.Combine(logRoot, "EventLog");
                    string text3 = Path.Combine(logRoot, "DataLog");
                    if (!Directory.Exists(logRoot))
                    {
                        Directory.CreateDirectory(logRoot);
                    }

                    if (!Directory.Exists(text))
                    {
                        Directory.CreateDirectory(text);
                    }

                    if (!Directory.Exists(text3))
                    {
                        Directory.CreateDirectory(text3);
                    }

                    if (!Directory.Exists(text2))
                    {
                        Directory.CreateDirectory(text2);
                    }

                    errorLogWritter.InitLogFile(text, errorFileName + LogFileExtension);
                    eventLogWritter.InitLogFile(text2, writeEventFileName + LogFileExtension);
                    dataLogWritter.InitLogFile(text3, dataLogFileName + LogFileExtension);
                    IsInitializationDone = true;
                }
                else
                {
                    hasToWriteEventLogFile = false;
                    hasToWriteErrorLogFile = false;
                }
            }
            catch (Exception ex)
            {
                hasToWriteEventLogFile = false;
                hasToWriteErrorLogFile = false;
                return ex.ToString();
            }

            return "";
        }

        private void InitEventErrorList(string rootFolderLoc)
        {
            if (rootFolderLoc == "")
            {
                return;
            }

            string text = rootFolderLoc + ErrorEventDefinationFolderName + "\\" + ErrorDefinationFileName + DefinationExtention;
            string text2 = "";
            if (File.Exists(text))
            {
                text2 = ReadErrorEventDefinationFromFile(text, ErrorList);
                if (text2 != "")
                {
                    throw new Exception(text2);
                }
            }

            string text3 = rootFolderLoc + ErrorEventDefinationFolderName + "\\" + EventDefinationFileName + DefinationExtention;
            if (File.Exists(text3))
            {
                text2 = ReadErrorEventDefinationFromFile(text3, EventList);
                if (text2 != "")
                {
                    throw new Exception(text2);
                }
            }
        }

        public string ReadErrorEventDefinationFromFile(string fileLoc, Dictionary<uint, string> storeList)
        {
            string result = "";
            FileStream fileStream = null;
            StreamReader streamReader = null;
            try
            {
                string text = "";
                fileStream = new FileStream(fileLoc, FileMode.Open);
                streamReader = new StreamReader(fileStream);
                text = streamReader.ReadToEnd();
                string[] array = text.Split("\n".ToCharArray());
                storeList.Clear();
                for (int i = 0; i < array.Length; i++)
                {
                    if (!(array[i] != "") || array[i].StartsWith("//"))
                    {
                        continue;
                    }

                    string[] array2 = array[i].Split("-".ToCharArray());
                    if (array2.Length == 2)
                    {
                        uint result2 = 0u;
                        string text2 = array2[1].Replace("\n", "");
                        text2 = array2[1].Replace("\r", "");
                        if (uint.TryParse(array2[0], out result2))
                        {
                            storeList.Add(result2, text2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            finally
            {
                streamReader?.Close();
                fileStream?.Close();
            }

            return result;
        }

        public string GetEventDescription(uint key, ref string eventDescription)
        {
            string result = "";
            try
            {
                if (!IsInitializationDone)
                {
                    throw new Exception("Must be Initialize Event List first");
                }

                if (!EventList.TryGetValue(key, out eventDescription))
                {
                    throw new Exception("Key not found. No Event Description available");
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public string GetErrorDescription(uint key, ref string errorDescription)
        {
            string result = "";
            try
            {
                if (!IsInitializationDone)
                {
                    throw new Exception("Must be Initialize Error List first");
                }

                if (!ErrorList.TryGetValue(key, out errorDescription))
                {
                    throw new Exception("Key not found. No error description available");
                }
            }
            catch (Exception ex)
            {
                result = (errorDescription = ex.ToString());
            }

            return result;
        }

        public void ProcessError(uint ID)
        {
            ProcessError(ID, "");
        }

        public void ProcessError(uint id, string extraInfo)
        {
            if (IsInitializationDone && IsToAddtoList)
            {
                string errorDescription = "";
                string errorDescription2 = GetErrorDescription(id, ref errorDescription);
                if (errorDescription2 != "")
                {
                    errorDescription = "No Error Description!";
                }

                ProcessError(errorDescription, extraInfo, id);
            }
        }

        public void ProcessError(string errorDescription, string extraInfo = "", uint id = 0u)
        {
            if (IsInitializationDone && IsToAddtoList)
            {
                string text = DateTime.Now.ToString("HH:mm:ss.ff");
                string text2 = ((id == 0) ? "" : id.ToString());
                string text3 = "Err:" + text2 + " " + errorDescription;
                if (hasToWriteErrorLogFile)
                {
                    errorLogWritter.WriteToLog(text3, extraInfo, text);
                }

                AddToDisplayList(text3, text);
            }
        }

        public void ProcessEvent(uint id)
        {
            ProcessEvent(id, "");
        }

        public void ProcessEvent(uint id, string extra)
        {
            if (IsInitializationDone && IsToAddtoList)
            {
                string eventDescription = "";
                string eventDescription2 = GetEventDescription(id, ref eventDescription);
                if (eventDescription2 != "")
                {
                    eventDescription = "No Event Description for ID <" + id + ">";
                }

                ProcessEvent(eventDescription, extra, id);
            }
        }

        public void ProcessEvent(string description)
        {
            ProcessEvent(description, "");
        }

        public void ProcessEvent(string description, string extraInfo, uint id = 0u)
        {
            if (IsInitializationDone && (hasToWriteEventLogFile || IsToAddtoList))
            {
                string text = DateTime.Now.ToString("HH:mm:ss.ff");
                string text2 = ((id == 0) ? "" : id.ToString());
                string text3 = "Evt:" + text2 + " " + description;
                if (hasToWriteEventLogFile)
                {
                    eventLogWritter.WriteToLog(text3, extraInfo, text);
                }

                AddToDisplayList(text3, text);
            }
        }

        public void ProcessData(string sData)
        {
            if (IsInitializationDone)
            {
                DateTime now = DateTime.Now;
                string text = $"{now:dd_M_yyyy}";
                dataLogWritter.WriteToLog(sData);
            }
        }

        public void ClearMessage()
        {
            lock (lck)
            {
                LstMsgEvent.Clear();
            }
        }

        private void AddToDisplayList(string sEvtMsg, string time)
        {
            if (!IsToAddtoList)
            {
                return;
            }

            string msg = time + " : " + sEvtMsg;
            Action action = delegate
            {
                lock (lck)
                {
                    LstMsgEvent.Insert(0, new ErrorEventItem(msg));
                    if (LstMsgEvent.Count > NoOfItemDisplayOnList)
                    {
                        while (LstMsgEvent.Count > NoOfItemDisplayOnList / 2)
                        {
                            LstMsgEvent.RemoveAt(LstMsgEvent.Count - 1);
                        }
                    }
                }
            };
            if (base.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                base.Dispatcher.BeginInvoke(action, DispatcherPriority.Normal, null);
            }
        }
    }

    public class ErrorEventItem : INotifyPropertyChanged
    {
        private string val = "";

        public string Value
        {
            get
            {
                return val;
            }
            set
            {
                val = value;
                OnPropertyChanged("Value");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ErrorEventItem(string sval)
        {
            Value = sval;
        }

        private void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
