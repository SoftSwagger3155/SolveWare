using SolveWare_Service_Core.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Manager.Base.Interface
{
    public interface IMachineStatus
    {
        /// <summary>
        /// 机器状态
        /// </summary>
        Machine_Status Status { get; }

        /// <summary>
        /// 设置机器状态
        /// </summary>
        /// <param name="status"></param>
        void SetStatus(Machine_Status status);


        bool IsStop { get; set; }
        ///// <summary>
        ///// 确认是否有错误或是停止
        ///// </summary>
        ///// <param name="errorCode"></param>
        ///// <returns></returns>
        //bool HasErrorStop(int errorCode);
    }
}
