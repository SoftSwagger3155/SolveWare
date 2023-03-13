using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Communications.Base.Interface
{
    public interface IInstrumentChassis
    {
        void Initialize();
        void Initialize(int timeout);
        string Resource { get; }
        string Name { get; }
        bool IsOnline { get; }
        void Turn_On_Line(bool isOnline);
        void Turn_On_Simulation();
        void TryWrite(string cmd);
        void TryWrite(byte[] msg);
        byte[] Query(byte[] cmd, int bytesToRead, int delay_ms);
        byte[] Query(byte[] cmd, int delay_ms);
        string Query(string cmd, int delay_ms);
        string ReadString();
        int Timeout_ms { get; set; }
    }
}
