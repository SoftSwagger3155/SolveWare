using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.General
{
    public class ErrorCodes
    {
        public const int NoError = 0;
        public const int ActionNotTaken = -1;
        public const int UserReqestStop = -2;
        public const int Unexecuted = -3;
        public const int NoStateActionAssign = -4;
        public const int OutOfBoundary = -5;
        public const int RunOutOfUnit = -6;
        public const int NoRelevantData = -7;
        public const int NoRelevantObject = -8;
        public const int MotorHomingError = -9;
        public const int MotorMoveError = -10;
        public const int ActionFailed = -11;
        public const int VisionFailed = -12;
        public const int NoMotorObject = -13;
        public const int MoveSlowDownError = -14;
        public const int NoIOObject = -15;
        public const int IOFunctionError = -16;
        public const int MotionFunctionError = -17;


        static Dictionary<int, string> ErrorMessageMap = new Dictionary<int, string>();

        public static void InitErrorMap()
        {
            Dictionary<int, string> m = ErrorMessageMap;
            m.Add(ActionNotTaken, "Action Not Taken | 行为任务尚未被执行");
            m.Add(UserReqestStop, "User Reqest Stop | 使用者停止机器");
            m.Add(NoStateActionAssign, "No State Action Assigned | 无行为任务指派");
            m.Add(Unexecuted, "Unexecuted | 未执行");
            m.Add(MotorHomingError, "Homing Error | 复位失败");
            m.Add(MotorMoveError, "Motor Move Error | 马达位移失败");
            m.Add(OutOfBoundary, "Out Of Boundary | 超出范围");
            m.Add(RunOutOfUnit, "Run Out Of Unit | 无产品");
            m.Add(VisionFailed, "Vision Failed | 视觉失败");
            m.Add(NoMotorObject, "No Motor Object | 无马达物件");
            m.Add(MoveSlowDownError, "Move Slow Down Error | 缓速失败");
            m.Add(NoIOObject, "NO IO Object | 无IO物件");
            m.Add(IOFunctionError, "IO Function Error | IO 执行失败");
            m.Add(MotionFunctionError, "Motion Function Error | Motion 执行失败");
        }

        public static string GetErrorDescription(int errorCode, string extraInfo = "")
        {
            string errorMsg = "Undefined";
            string fmsError = "";// FSMInnerErrorCode.GetErrorMessage(errorCode);
            if (fmsError != "")
                errorMsg = fmsError + " Code=" + errorCode.ToString();
            else if (ErrorMessageMap.ContainsKey(errorCode))
                errorMsg = ErrorMessageMap[errorCode];
            else
                errorMsg = errorMsg + " Code=" + errorCode.ToString();

            if (!string.IsNullOrEmpty(extraInfo))
                errorMsg += "<" + extraInfo + ">";

            return errorMsg;

        }
    }
}
