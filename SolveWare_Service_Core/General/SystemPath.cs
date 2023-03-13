using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.General
{
    public class SystemPath
    {
        static string ExeRoot = Directory.GetCurrentDirectory();
        public static string RootDirectory
        {
            get
            {
                if (Debugger.IsAttached) return RootInfoDirection;
                return ExeRoot;
            }

        }

        public static string RootInfoDirection;// = $"D:\\{Assembly.GetExecutingAssembly().GetName().Name}";
        public static string RootLogDirectory; //= $"D:\\{Assembly.GetExecutingAssembly().GetName().Name} Logs";
        public static string RootDataDirectory; //= $"D:\\{Assembly.GetExecutingAssembly().GetName().Name} Data";

        public static string RootOperatorIdFile = "Operators\\OperatorID.txt";
        public static string RootTestPurposeFile = "TestPurposes\\TestPurpose.txt";
        public static string RootSumConfigDirection = "\\SmuConfigs\\";
        public static string RootTestDataDirection = "\\TestDatas\\";
        public static string RootTestJobDirection = "\\TestJobs\\";


        public const string RecipeDirectory = "Recipe";
        public const string LastCntDirectory = "LastCnt";
        public const string LogDirectory = "Log";
        public const string LotDirectory = "Lot";
        public const string SystemDirectory = "System";
        public const string MVVMDirectory = "MVVM";
        public const string WorkOrderDirectory = "WorkOrder";
        public const string VisionDirectory = "Vision";
        public const string VisionSupportDirectory = "Vision Support";
        public const string ControlDirectory = "Control";
        public const string TimingLogDirectory = "TimingLog";
        public const string LotExt = ".csv";
        public const string VisExt = "mvp";
        public const string ControlExt = "mcp";
        public const string BatExt = ".txt";
        public const string LogExt = ".log";

        public const string ConfigFileName = "Config.xml";

        public const string AxesConfigFileName = "AxesConfig.xml";
        public const string AxesSpeedFileName = "AxesSpeed.xml";
        public const string AxesTableFileName = "AxesTable.xml";
        public const string AxesMiscFileName = "AxesMisc.xml";

        public const string TECParameterFileName = "TECParameters.xml";

        public const string TowerLightFileName = "TowerLightConfig.xml";
        public const string OtherDataFileName = "OtherData.xml";

        public const string IoCardFileName = "IoCard.xml";
        public const string InputFileName = "Input.xml";
        public const string OutputFileName = "Output.xml";

        public const string FSMController_Home_Directory = "FSM_Home_Controller";
        public const string FSMController_Auto_Directory = "FSM_Auto_Controller";
        public const string FSMController_Reset_Directory = "FSM_Reset_Controller";

        static public string GetTimingPath
        {
            get { return RootLogDirectory + "\\" + TimingLogDirectory; }
        }

        static public string GetRecipePath
        {
            get { return RootInfoDirection + "\\" + RecipeDirectory; }
        }

        static public string GetControlRecipePath
        {
            get { return RootInfoDirection + "\\" + RecipeDirectory + "\\" + ControlDirectory; }
        }

        static public string GetVisionRecipePath
        {
            get { return RootInfoDirection + "\\" + RecipeDirectory + "\\" + VisionDirectory; }
        }

        static public string GetVisionSupportRecipePath
        {
            get { return RootInfoDirection + "\\" + RecipeDirectory + "\\" + VisionSupportDirectory; }
        }

        static public string GetSystemPath
        {
            get { return RootDirectory + "\\" + SystemDirectory; }
        }
        static public string GetMVVMPath
        {
            get { return RootDirectory + "\\" + MVVMDirectory; }
        }

        static public string GetLastCntPath
        {
            get { return RootInfoDirection + "\\" + LastCntDirectory; }
        }

        static public string GetWorkOrderPath
        {
            get { return RootInfoDirection + "\\" + WorkOrderDirectory; }
        }

        static public string GetLogPath
        {
            get { return RootLogDirectory + "\\" + LogDirectory; }
        }

        static public string GetLotPath
        {
            get { return RootInfoDirection + "\\" + LotDirectory; }
        }

        static public string LoadedSystemPath
        {
            get { return GetSystemPath; }
        }
        static public string GetFSMHomeControllerDirectory
        {
            get { return RootInfoDirection + "\\" + FSMController_Home_Directory; }
        }
        static public string GetFSMAutoControllerDirectory
        {
            get { return RootInfoDirection + "\\" + FSMController_Auto_Directory; }
        }
        static public string GetFSMResetControllerDirectory
        {
            get { return RootInfoDirection + "\\" + FSMController_Reset_Directory; }
        }

        public static void CreateDefaultDirectory(bool excludelogs = false)
        {
            CreateDirectoryIfDontHave(RootInfoDirection);
            CreateDirectoryIfDontHave(RootLogDirectory);
            //CreateDirectoryIfDontHave(GetRecipePath);
            CreateDirectoryIfDontHave(GetSystemPath);
            //CreateDirectoryIfDontHave(GetLastCntPath);
            //CreateDirectoryIfDontHave(GetWorkOrderPath);
            //CreateDirectoryIfDontHave(GetVisionSupportRecipePath);

            //CreateDirectoryIfDontHave(GetLotPath);
            //CreateDirectoryIfDontHave(GetControlRecipePath);
            //CreateDirectoryIfDontHave(GetVisionRecipePath);
            CreateDirectoryIfDontHave(GetMVVMPath);


            CreateDirectoryIfDontHave(GetFSMHomeControllerDirectory);
            CreateDirectoryIfDontHave(GetFSMAutoControllerDirectory);
            CreateDirectoryIfDontHave(GetFSMResetControllerDirectory);

            if (!excludelogs)
                CreateDirectoryIfDontHave(GetTimingPath);
            if (!excludelogs)
                CreateDirectoryIfDontHave(GetLogPath);
        }
        public static void CreateDirectoryIfDontHave(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        public static bool IsFileExisted(string path)
        {
            //@"c:\tempuploads\newFile.txt"
            return File.Exists($@"{path}");
        }
    }
}
