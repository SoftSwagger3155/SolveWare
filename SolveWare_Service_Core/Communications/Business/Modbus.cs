using SolveWare_Service_Core.Communications.Base.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Communications.Business
{
    public enum ModbusStatus
    {
        WriteSuccessful,
        WriteError,
        CRCCheckFailed,
        PortNotOpened,
        Idle,
    }
    public class Modbus
    {
        public const byte MB_READ_COILS = 0x01;             //读线圈寄存器
        public const byte MB_READ_DISCRETE = 0x02;          //读离散输入寄存器
        public const byte MB_READ_HOLD_REG = 0x03;          //读保持寄存器
        public const byte MB_READ_INPUT_REG = 0x04;         //读输入寄存器

        public const byte MB_WRITE_SINGLE_COIL = 0x05;      //写单个线圈
        public const byte MB_WRITE_SINGLE_REG = 0x06;       //写单寄存器
        public const byte MB_WRITE_MULTIPLE_COILS = 0x0f;   //写多线圈
        public const byte MB_WRITE_MULTIPLE_REGS = 0x10;    //写多寄存器

        const int DELAY_MS = 500;

        IInstrumentChassis _chassis;
        ModbusStatus _status = ModbusStatus.Idle;

        public Modbus(IInstrumentChassis communicationChassis)
        {
            this._chassis = communicationChassis;
        }


        public ModbusStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
        public string ChassisResource
        {
            get
            {
                return this._chassis.Resource;
            }
        }
        /// <summary>
        /// Write single coil value
        /// value in dec
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Function_1(int nodeAddress, int coilAddress, int coilCount, ref bool[] vals)
        {
            byte[] header = BuildMessageHeader(nodeAddress, MB_READ_COILS, coilAddress, coilCount);
            byte crcHi, crcLo;
            CRCCalculation.CRC16(header, header.Length, out crcHi, out crcLo);
            byte[] message = new byte[header.Length + 2];
            header.CopyTo(message, 0);
            message[message.Length - 1] = crcHi;
            message[message.Length - 2] = crcLo;
            //byte[] response = new byte[8];
            byte[] response = new byte[5 + (int)Math.Ceiling(coilCount / 8.0)];
            try
            {
                response = _chassis.Query(message, response.Length, DELAY_MS);
            }
            catch
            {
                _status = ModbusStatus.WriteError;
                return false;
            }
            if (CheckResponse(response, nodeAddress, MB_READ_COILS))
            {
                int dataLength = (int)response[2];
                List<byte> data = new List<byte>();
                List<bool> temp = new List<bool>();
                for (int i = 0; i < dataLength; i++)
                {
                    data.Add(response[3 + i]);
                }
                for (int i = 0; i < data.Count; i++)
                {
                    temp.AddRange(BaseDataConverter.ConvertByteToBitBoolean(data[i]));
                }
                Array.Copy(temp.ToArray(), vals, vals.Length);

                _status = ModbusStatus.WriteSuccessful;

                return true;
            }
            else
            {
                _status = ModbusStatus.CRCCheckFailed;
                return false;
            }

        }

        public bool Function_2(int nodeAddress, int registerAddress, int bitCount, ref bool[] values)
        {
            return ReadBits(MB_READ_DISCRETE, nodeAddress, registerAddress, bitCount, ref values);
        }

        public bool Function_3(int nodeAddress, int startRegisterAddress, int registerCount, ref short[] values)
        {
            return ReadRegisters(MB_READ_HOLD_REG, nodeAddress, startRegisterAddress, registerCount, ref values);
        }
        public bool Function_5(int nodeAddress, int registerAddress, int value)
        {
            byte[] header = BuildMessageHeader(nodeAddress, MB_WRITE_SINGLE_COIL, registerAddress, value);

            //byte[] header = BuildMessageHeader(nodeAddress, MB_WRITE_SINGLE_COIL, registerAddress, value);
            byte crcHi, crcLo;
            CRCCalculation.CRC16(header, header.Length, out crcHi, out crcLo);
            byte[] message = new byte[header.Length + 2];
            header.CopyTo(message, 0);
            message[message.Length - 1] = crcHi;
            message[message.Length - 2] = crcLo;
            byte[] response = new byte[8];

            try
            {
                response = _chassis.Query(message, response.Length, DELAY_MS);
            }
            catch (Exception ex)
            {
                _status = ModbusStatus.WriteError;
                return false;
            }
            if (CheckResponse(response, nodeAddress, MB_WRITE_SINGLE_COIL))
            {
                _status = ModbusStatus.WriteSuccessful;
                return true;
            }
            else
            {
                _status = ModbusStatus.CRCCheckFailed;
                return false;
            }
        }

        /// <summary>
        /// Write single register value
        /// value in dec
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Function_6(int nodeAddress, int registerAddress, short data)    //SET TYPE, SET TEMP, SET TORLERANCE
        {
            byte[] response = new byte[8];
            byte[] header = BuildMessageHeader(nodeAddress, MB_WRITE_SINGLE_REG, registerAddress, data);
            byte crcHi, crcLo;
            CRCCalculation.CRC16(header, header.Length, out crcHi, out crcLo);
            byte[] message = new byte[header.Length + 2];
            header.CopyTo(message, 0);
            message[message.Length - 1] = crcHi;
            message[message.Length - 2] = crcLo;

            try
            {

                response = _chassis.Query(message, response.Length, DELAY_MS);

            }
            catch
            {
                _status = ModbusStatus.WriteError;
                return false;
            }
            if (CheckResponse(response, nodeAddress, MB_WRITE_SINGLE_REG))
            {
                _status = ModbusStatus.WriteSuccessful;
                return true;
            }
            else
            {
                _status = ModbusStatus.CRCCheckFailed;
                return false;
            }
        }
        public bool ReadBits(byte functionType, int nodeAddress, int startRegisterAddress, int bitsCount, ref bool[] values)//int _addr, byte _stand, int _length)
        {
            byte[] response = new byte[5 + bitsCount / 8];
            byte[] header = BuildMessageHeader(nodeAddress, functionType, startRegisterAddress, bitsCount);
            byte[] headerWithDataLengthBit = CalcMessageDataLengthBit(functionType, header, ref bitsCount);
            byte[] message = new byte[headerWithDataLengthBit.Length + bitsCount + 2];
            headerWithDataLengthBit.CopyTo(message, 0);
            byte crcHi, crcLo;
            CRCCalculation.CRC16(message, message.Length - 2, out crcHi, out crcLo);
            message[message.Length - 1] = crcHi;
            message[message.Length - 2] = crcLo;

            try
            {

                response = _chassis.Query(message, response.Length, DELAY_MS);

            }
            catch (Exception ex)
            {
                _status = ModbusStatus.WriteError;
                return false;
            }
            //Evaluate message:
            if (CheckResponse(response, nodeAddress, functionType))
            {
                for (int j = 0; j < response[2]; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        values[j * 8 + i] = (response[3 + j] & Convert.ToInt16(Math.Pow(2, i))) == Convert.ToInt16(Math.Pow(2, i));
                    }
                }

                _status = ModbusStatus.WriteSuccessful;
                return true;
            }
            else
            {
                _status = ModbusStatus.CRCCheckFailed;
                return false;
            }
        }

        public bool ReadRegisters(byte functionType, int nodeAddress, int startRegisterAddress, int registerCount, ref short[] values)
        { //Ensure port is open:
            byte[] response = new byte[5 + 2 * registerCount];
            byte[] header = BuildMessageHeader(nodeAddress, functionType, startRegisterAddress, registerCount);

            byte[] headerWithDataLengthBit = CalcMessageDataLengthBit(functionType, header, ref registerCount);

            byte[] message = new byte[headerWithDataLengthBit.Length + registerCount + 2];
            headerWithDataLengthBit.CopyTo(message, 0);
            byte crcHi, crcLo;
            CRCCalculation.CRC16(message, message.Length - 2, out crcHi, out crcLo);
            message[message.Length - 1] = crcHi;
            message[message.Length - 2] = crcLo;

            try
            {
                response = _chassis.Query(message, response.Length, DELAY_MS);
            }
            catch (Exception ex)
            {

                _status = ModbusStatus.WriteError;
                throw ex;
                return false;
            }
            //Evaluate message:
            if (CheckResponse(response, nodeAddress, functionType))
            {
                //Return requested register values:
                for (int i = 0; i < (response.Length - 5) / 2; i++)
                {
                    values[i] = response[2 * i + 3];
                    values[i] <<= 8;
                    values[i] += response[2 * i + 4];
                }
                _status = ModbusStatus.WriteSuccessful;
                return true;
            }
            else
            {
                _status = ModbusStatus.CRCCheckFailed;
                return false;
            }
        }

        /// <summary>
        /// Read multiple hold regsiters values
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <param name="startRegisterAddress"></param>
        /// <param name="registerCount"></param>
        /// <param name="values"></param>
        /// <returns></returns>

        /// <summary>
        /// Read multiple input regsiters values
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <param name="startRegisterAddress"></param>
        /// <param name="registerCount"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool Function_4(int nodeAddress, int startRegisterAddress, int registerCount, ref short[] values)
        {
            return ReadRegisters(MB_READ_INPUT_REG, nodeAddress, startRegisterAddress, registerCount, ref values);
        }
        /// <summary>
        /// Write multiple coils
        /// Values formated in two bytes, example : every byte stand for 8bits - [CD] = 1100 1101
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <param name="startRegisterAddress"></param>
        /// <param name="registerCount"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool Function_15(int nodeAddress, int startRegisterAddress, int registerCount, byte[] values)
        {

            byte[] response = new byte[8];


            byte[] header = BuildMessageHeader(nodeAddress, MB_WRITE_MULTIPLE_COILS, startRegisterAddress, registerCount);
            byte[] headerWithDataLengthBit = CalcMessageDataLengthBit(MB_WRITE_MULTIPLE_COILS, header, ref registerCount);
            //2 CRC
            byte[] message = new byte[headerWithDataLengthBit.Length + registerCount + 2];
            headerWithDataLengthBit.CopyTo(message, 0);
            Array.Copy(values, 0, message, headerWithDataLengthBit.Length, registerCount);

            byte crcHi, crcLo;
            CRCCalculation.CRC16(message, message.Length - 2, out crcHi, out crcLo);
            message[message.Length - 1] = crcHi;
            message[message.Length - 2] = crcLo;

            try
            {
                response = _chassis.Query(message, response.Length, DELAY_MS);
            }
            catch
            {
                _status = ModbusStatus.WriteError;
                return false;
            }
            if (CheckResponse(response, nodeAddress, MB_WRITE_MULTIPLE_COILS))
            {
                _status = ModbusStatus.WriteSuccessful;
                return true;
            }
            else
            {
                _status = ModbusStatus.CRCCheckFailed;
                return false;
            }
        }

        //林斌新增
        public bool Function_16(int nodeAddress, int startRegisterAddress, int registerCount, ushort[] ushortvalues)
        {

            byte[] values = new byte[registerCount * 2];

            for (int i = 0; i < registerCount; i++)
            {
                byte[] sourceArray = BitConverter.GetBytes(ushortvalues[i]);
                values[i * 2] = sourceArray[1];
                values[i * 2 + 1] = sourceArray[0];
                //Array.Copy(sourceArray, 0, values, i * 2, sourceArray.Length);
            }

            bool rtn = Function_16(nodeAddress, startRegisterAddress, registerCount, values);
            return rtn;

        }

        /// <summary>
        /// Write multiple registers
        /// values formated in two bytes, example : [FF][01] | [EC][64] 
        /// </summary>
        /// <param name="nodeAddress"></param>
        /// <param name="startRegisterAddress"></param>
        /// <param name="registerCount"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool Function_16(int nodeAddress, int startRegisterAddress, int registerCount, byte[] values) //SET UNIT TIME, SET CONTROL MODE
        {

            //MB_WRITE_MULTIPLE_REGS
            byte[] response = new byte[8];
            byte[] header = BuildMessageHeader(nodeAddress, MB_WRITE_MULTIPLE_REGS, startRegisterAddress, registerCount);
            byte[] headerWithDataLengthBit = CalcMessageDataLengthBit(MB_WRITE_MULTIPLE_REGS, header, ref registerCount);
            byte[] message = new byte[headerWithDataLengthBit.Length + registerCount + 2];
            headerWithDataLengthBit.CopyTo(message, 0);

            Array.Copy(values, 0, message, headerWithDataLengthBit.Length, registerCount);

            byte crcHi, crcLo;
            CRCCalculation.CRC16(message, message.Length - 2, out crcHi, out crcLo);
            message[message.Length - 1] = crcHi;
            message[message.Length - 2] = crcLo;

            try
            {
                response = _chassis.Query(message, response.Length, DELAY_MS);
            }
            catch (Exception ex)
            {
                _status = ModbusStatus.WriteError;
                return false;
            }
            if (CheckResponse(response, nodeAddress, MB_WRITE_MULTIPLE_REGS))
            {
                _status = ModbusStatus.WriteSuccessful;
                return true;
            }
            else
            {
                _status = ModbusStatus.CRCCheckFailed;
                return false;
            }
        }
        private bool CheckResponse(byte[] response, int nodeAddress, byte functionType)
        {
            try
            {
                if (response[0] != (byte)nodeAddress)
                    return false;
                if (response[1] != functionType)
                    return false;
                //CRC check
                byte crcHi, crcLo;
                CRCCalculation.CRC16(response, response.Length - 2, out crcHi, out crcLo);
                if (crcHi == response[response.Length - 1] && crcLo == response[response.Length - 2])
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Build 6 bytes message header
        /// </summary>
        /// <param name="nodeAddress">Node address(instrument address)</param>
        /// <param name="functionType">Function type</param>
        /// <param name="startAddress">Start register/coil Address</param>
        /// <param name="length">Data length or single data</param>
        private byte[] BuildMessageHeader(int nodeAddress, byte functionType, int startAddress, int length)
        {
            byte[] header = new byte[6];

            header[0] = (byte)nodeAddress;
            header[1] = functionType;
            header[2] = (byte)(startAddress >> 8);
            header[3] = (byte)(startAddress & 0xFF);
            header[4] = (byte)(length >> 8);
            header[5] = (byte)(length & 0xFF);
            return header;
        }


        private byte[] CalcMessageDataLengthBit(byte functionType, byte[] messageHeader, ref int length)
        {
            byte[] newHeader = null;
            switch (functionType)
            {
                case MB_READ_COILS:
                case MB_READ_DISCRETE:
                case MB_READ_HOLD_REG:
                case MB_READ_INPUT_REG:
                case MB_WRITE_SINGLE_COIL:
                case MB_WRITE_SINGLE_REG:
                    newHeader = messageHeader;
                    length = 0;
                    break;
                case MB_WRITE_MULTIPLE_COILS://valuse like 0101001100001111
                    //calculate length
                    length = (length % 8 == 0) ? (length / 8) : (length / 8 + 1);
                    //extand source array as new array by 1 bit
                    newHeader = new byte[messageHeader.Length + 1];
                    //copy source to new one
                    messageHeader.CopyTo(newHeader, 0);
                    //write data length to new array last bit
                    newHeader[messageHeader.Length] = (byte)(length);
                    break;
                case MB_WRITE_MULTIPLE_REGS:
                    //calculate length
                    length *= 2;
                    //extand source array as new array by 1 bit
                    newHeader = new byte[messageHeader.Length + 1];
                    //copy source to new one
                    messageHeader.CopyTo(newHeader, 0);
                    //write data length to last bit
                    newHeader[messageHeader.Length] = (byte)length;
                    break;
            }
            return newHeader;
        }

    }
    public static class CRCCalculation
    {
        /// <summary>
        /// string data in HEX
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CRC16ToHex(string data)
        {
            return CRC16(data).ToString("x");
        }
        public static string CRC16ToHex(string data, string fakeP)
        {
            return CRC16(data).ToString("x");
        }
        /// <summary>
        /// string data in HEX
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int CRC16(string data)
        {
            byte hi, lo;
            byte[] byteArr = BaseDataConverter.ConvertHexStringToByteArray(data, false);
            CRC16(byteArr, byteArr.Length, out hi, out lo);
            return ((hi << 8) | lo);
        }
        public static void CRC16(string data, out byte crcHi, out byte crcLo)
        {
            byte[] byteArr = BaseDataConverter.ConvertHexStringToByteArray(data, false);
            CRC16(byteArr, byteArr.Length, out crcHi, out crcLo);
        }
        public static void CRC16(byte[] data, int length, out byte crcHi, out byte crcLo)
        {
            int i = 0;
            uint uchCRCHi = 0xFF;
            uint uchCRCLo = 0xFF;
            uint uIndex;

            while (length-- > 0)
            {
                uIndex = (uchCRCLo ^ data[i++]);
                uchCRCLo = (byte)(uchCRCHi ^ CRCHi_table[uIndex]);
                uchCRCHi = CRCLo_table[uIndex];
            }
            crcHi = (byte)uchCRCHi;
            crcLo = (byte)uchCRCLo;

        }


        #region crcTable
        static readonly byte[] CRCHi_table ={
        0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,
        0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
        0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,
        0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
        0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x00,0xC1,
        0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,
        0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x00,0xC1,
        0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
        0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,
        0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,
        0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,
        0x81,0x40,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
        0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,
        0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,
        0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,
        0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
        0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,
        0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
        0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,
        0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
        0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x01,0xC0,
        0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,
        0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,
        0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
        0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,
        0x80,0x41,0x00,0xC1,0x81,0x40
        };

        static readonly byte[] CRCLo_table ={
        0x00,0xC0,0xC1,0x01,0xC3,0x03,0x02,0xC2,0xC6,0x06,
        0x07,0xC7,0x05,0xC5,0xC4,0x04,0xCC,0x0C,0x0D,0xCD,
        0x0F,0xCF,0xCE,0x0E,0x0A,0xCA,0xCB,0x0B,0xC9,0x09,
        0x08,0xC8,0xD8,0x18,0x19,0xD9,0x1B,0xDB,0xDA,0x1A,
        0x1E,0xDE,0xDF,0x1F,0xDD,0x1D,0x1C,0xDC,0x14,0xD4,
        0xD5,0x15,0xD7,0x17,0x16,0xD6,0xD2,0x12,0x13,0xD3,
        0x11,0xD1,0xD0,0x10,0xF0,0x30,0x31,0xF1,0x33,0xF3,
        0xF2,0x32,0x36,0xF6,0xF7,0x37,0xF5,0x35,0x34,0xF4,
        0x3C,0xFC,0xFD,0x3D,0xFF,0x3F,0x3E,0xFE,0xFA,0x3A,
        0x3B,0xFB,0x39,0xF9,0xF8,0x38,0x28,0xE8,0xE9,0x29,
        0xEB,0x2B,0x2A,0xEA,0xEE,0x2E,0x2F,0xEF,0x2D,0xED,
        0xEC,0x2C,0xE4,0x24,0x25,0xE5,0x27,0xE7,0xE6,0x26,
        0x22,0xE2,0xE3,0x23,0xE1,0x21,0x20,0xE0,0xA0,0x60,
        0x61,0xA1,0x63,0xA3,0xA2,0x62,0x66,0xA6,0xA7,0x67,
        0xA5,0x65,0x64,0xA4,0x6C,0xAC,0xAD,0x6D,0xAF,0x6F,
        0x6E,0xAE,0xAA,0x6A,0x6B,0xAB,0x69,0xA9,0xA8,0x68,
        0x78,0xB8,0xB9,0x79,0xBB,0x7B,0x7A,0xBA,0xBE,0x7E,
        0x7F,0xBF,0x7D,0xBD,0xBC,0x7C,0xB4,0x74,0x75,0xB5,
        0x77,0xB7,0xB6,0x76,0x72,0xB2,0xB3,0x73,0xB1,0x71,
        0x70,0xB0,0x50,0x90,0x91,0x51,0x93,0x53,0x52,0x92,
        0x96,0x56,0x57,0x97,0x55,0x95,0x94,0x54,0x9C,0x5C,
        0x5D,0x9D,0x5F,0x9F,0x9E,0x5E,0x5A,0x9A,0x9B,0x5B,
        0x99,0x59,0x58,0x98,0x88,0x48,0x49,0x89,0x4B,0x8B,
        0x8A,0x4A,0x4E,0x8E,0x8F,0x4F,0x8D,0x4D,0x4C,0x8C,
        0x44,0x84,0x85,0x45,0x87,0x47,0x46,0x86,0x82,0x42,
        0x43,0x83,0x41,0x81,0x80,0x40
        };
        #endregion
    }
    public static class BaseDataConverter
    {
        public static double ScientificNotationRound(this double value, int digits)
        {
            if (digits < 1)
            {
                throw new ArgumentOutOfRangeException("digits", digits, "digits cannot be smaller than 1.");
            }
            string temp = value.ToString("e" + (digits - 1).ToString());
            return double.Parse(temp);
        }

        public static object ConvertObjectTo(object obj, Type type)
        {
            if (type == null) return obj;
            if (obj == null) return type.IsValueType ? Activator.CreateInstance(type) : null;

            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (type.IsAssignableFrom(obj.GetType())) // 如果待转换对象的类型与目标类型兼容，则无需转换
            {
                return obj;
            }
            else if ((underlyingType ?? type).IsEnum) // 如果待转换的对象的基类型为枚举
            {
                if (underlyingType != null && string.IsNullOrEmpty(obj.ToString())) // 如果目标类型为可空枚举，并且待转换对象为null 则直接返回null值
                {
                    return null;
                }
                else
                {
                    return Enum.Parse(underlyingType ?? type, obj.ToString());
                }
            }
            else if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? type)) // 如果目标类型的基类型实现了IConvertible，则直接转换
            {
                try
                {
                    return Convert.ChangeType(obj, underlyingType ?? type, null);
                }
                catch
                {
                    return underlyingType == null ? Activator.CreateInstance(type) : null;
                }
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                if (converter.CanConvertFrom(obj.GetType()))
                {
                    return converter.ConvertFrom(obj);
                }
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    object o = constructor.Invoke(null);
                    PropertyInfo[] propertys = type.GetProperties();
                    Type oldType = obj.GetType();
                    foreach (PropertyInfo property in propertys)
                    {
                        PropertyInfo p = oldType.GetProperty(property.Name);
                        if (property.CanWrite && p != null && p.CanRead)
                        {
                            property.SetValue(o, ConvertObjectTo(p.GetValue(obj, null), property.PropertyType), null);
                        }
                    }
                    return o;
                }
            }
            return obj;
        }
        public static string ConvertTimespanToString(TimeSpan span)
        {
            double totalHrs = span.Days * 24.0 + span.Hours;
            return string.Format("{0:00}:{1:00}:{2:00}", totalHrs, span.Minutes, span.Seconds);
        }
        public static DateTime ConvertCommentStringToDateTime(string commentString)
        {
            DateTime rDT = DateTime.Now;
            if (DateTime.TryParse(commentString, out rDT))
            {

            }
            return rDT;
        }
        public static string ConvertDateTimeToCommentString(DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }
        public static string ConvertDateTimeTo_FILE_string(DateTime time)
        {
            return time.ToString("yyyyMMdd_HHmmss");
        }
        public static string ConvertDateTimeStringTo_FILE_string(string timestring)
        {
            return timestring.Replace("/", "").Replace(" ", "_").Replace(":", "");
        }
        // CDAB 类型 float -> 2short
        public static ushort[] ConvertFloatToUshortCDAB(float value)
        {
            ushort[] buf = new ushort[2];
            byte[] byteData = BitConverter.GetBytes(value);
            ushort[] registerBuffer = new ushort[byteData.Length / 2];
            registerBuffer[1] = BitConverter.ToUInt16(byteData, 0);
            registerBuffer[0] = BitConverter.ToUInt16(byteData, 2);
            buf[1] = registerBuffer[0];
            buf[0] = registerBuffer[1];
            return buf;
        }
        // 2short -> float CDAB 类型
        //20200602
        public static float ConvertUshortToFloatCDAB(short[] registerBuffer)
        {
            float value = 0;
            byte[] byteData1 = BitConverter.GetBytes(registerBuffer[0]);
            byte[] byteData2 = BitConverter.GetBytes(registerBuffer[1]);
            byte[] byteData = new byte[4];
            byteData[3] = byteData2[1];
            byteData[2] = byteData2[0];
            byteData[1] = byteData1[1];
            byteData[0] = byteData1[0];
            value = BitConverter.ToSingle(byteData, 0);
            return value;
        }

        // 2short -> float ABCD 类型
        //20200602
        public static float ConvertUshortToFloatABCD(short[] registerBuffer)
        {
            float value = 0;
            byte[] byteData1 = BitConverter.GetBytes(registerBuffer[0]);
            byte[] byteData2 = BitConverter.GetBytes(registerBuffer[1]);
            byte[] byteData = new byte[4];
            byteData[3] = byteData1[1];
            byteData[2] = byteData1[0];
            byteData[1] = byteData2[1];
            byteData[0] = byteData2[0];
            value = BitConverter.ToSingle(byteData, 0);
            return value;
        }

        // ABCD 类型 float -> 2short
        public static ushort[] FloatToUshort_ABCD(float value)
        {
            ushort[] buf = new ushort[2];
            byte[] byteData = BitConverter.GetBytes(value);
            ushort[] registerBuffer = new ushort[byteData.Length / 2];
            registerBuffer[1] = BitConverter.ToUInt16(byteData, 0);
            registerBuffer[0] = BitConverter.ToUInt16(byteData, 2);
            buf[1] = registerBuffer[1];
            buf[0] = registerBuffer[0];
            return buf;
        }
        public static double[] ArrayConvert_ABDEShortToDouble(short[] array)
        {
            List<double> ArrayDouble = new List<double>();
            int count = array.Length;
            short[] shortvalue = new short[2];

            for (int i = 0; i < count; i += 2)
            {
                Array.Copy(array, i, shortvalue, 0, 2);

                ArrayDouble.Add(ConvertUshortToFloatABCD(shortvalue));
            }

            return ArrayDouble.ToArray();
        }
        public static ushort[] LongToUshort(int value)
        {
            ushort[] buf = new ushort[2];
            byte[] byteData = BitConverter.GetBytes(value);
            ushort[] registerBuffer = new ushort[byteData.Length / 2];
            registerBuffer[1] = BitConverter.ToUInt16(byteData, 0);
            registerBuffer[0] = BitConverter.ToUInt16(byteData, 2);
            buf[1] = registerBuffer[1];
            buf[0] = registerBuffer[0];
            return buf;
        }

        //2short -> long
        public static int UshortToLong(short[] registerBuffer)
        {
            int value = 0;
            byte[] byteData1 = BitConverter.GetBytes(registerBuffer[0]);
            byte[] byteData2 = BitConverter.GetBytes(registerBuffer[1]);
            byte[] byteData = new byte[4];
            byteData[3] = byteData1[1];
            byteData[2] = byteData1[0];
            byteData[1] = byteData2[1];
            byteData[0] = byteData2[0];
            value = BitConverter.ToInt32(byteData, 0);
            return value;
        }
        public static float[] ArrayConvertStringToSingle(string[] array)
        {
            return Array.ConvertAll(array, new Converter<string, float>(ConvertStringToSingle));
        }
        private static float ConvertStringToSingle(string str)
        {
            return Convert.ToSingle(str);
        }
        public static double[] ArrayConvertStringToDouble(string[] array)
        {
            return Array.ConvertAll(array, new Converter<string, double>(ConvertStringToDouble));
        }
        private static double ConvertStringToDouble(string str)
        {
            return Convert.ToDouble(str);
        }
        public static double[] ArrayConvertByteToDouble(byte[] array)
        {
            return Array.ConvertAll(array, new Converter<byte, double>(ConvertByteToDouble));
        }
        static double ConvertByteToDouble(byte byteVal)
        {
            return Convert.ToDouble(byteVal);
        }
        public static float[] ArrayConvertShortToFloat(short[] array)
        {
            return Array.ConvertAll(array, new Converter<short, float>(ConvertShortToFloat));
        }
        public static float[] ArrayConvertDoubleToFloat(double[] array)
        {
            return Array.ConvertAll(array, new Converter<double, float>(ConvertDoubleToFloat));
        }
        static float ConvertDoubleToFloat(double dblVal)
        {
            var temp = Convert.ToSingle(dblVal);
            return temp;
        }
        public static double[] ArrayConvertShortToFloatToDouble(short[] tempArr)
        {
            double[] tempf = new double[tempArr.Length / 2];
            byte[] lo = new byte[2];
            byte[] hi = new byte[2];
            byte[] temp = new byte[4];
            for (int i = 0; i < tempArr.Length; i += 2)
            {
                lo = BitConverter.GetBytes(tempArr[i + 1]);
                hi = BitConverter.GetBytes(tempArr[i]);

                Array.Copy(lo, 0, temp, 0, 2);
                Array.Copy(hi, 0, temp, 2, 2);
                //Array.Reverse(temp);
                tempf[i / 2] = BitConverter.ToSingle(temp, 0);
            }
            return tempf;
        }
        static float ConvertShortToFloat(short shortVal)
        {
            var temp = Convert.ToSingle(shortVal);
            return temp;
        }
        public static double[] ArrayConvertShortToDouble(short[] array)
        {
            return Array.ConvertAll(array, new Converter<short, double>(ConvertShortToDouble));
        }
        public static double[] ArrayConvertFloatToDouble(float[] array)
        {
            return Array.ConvertAll(array, new Converter<float, double>(ConvertFloatToDouble));
        }
        public static double[] ArrayConvertABCDLongToDouble(short[] array)
        {
            return Array.ConvertAll(array, new Converter<short, double>(ConvertShortToDouble));
        }

        static double ConvertShortToDouble(short shortVal)
        {
            var temp = Convert.ToDouble(shortVal);
            return temp;
        }
        static double ConvertFloatToDouble(float fval)
        {
            var temp = Convert.ToDouble(fval);
            return temp;
        }
        public static int[] ArrayConvertShortToInteger(short[] array)
        {
            return Array.ConvertAll(array, new Converter<short, int>(ConvertShortToInteger));
        }
        static int ConvertShortToInteger(short shortVal)
        {
            return Convert.ToInt16(shortVal);
        }
        public static string ConvertByteArrayToHexString(byte[] Bytes)
        {
            const string HexAlphabet = "0123456789ABCDEF";
            StringBuilder Result = new StringBuilder(Bytes.Length * 2);

            foreach (byte B in Bytes)
            {
                Result.Append(HexAlphabet[(int)(B >> 4)]);
                Result.Append(HexAlphabet[(int)(B & 0xF)]);
            }

            return Result.ToString();
        }

        public static bool[] ConvertByteToBitBoolean(byte byt)
        {
            const int bitsForOneByte = 8;
            bool[] bits = new bool[bitsForOneByte];
            for (int j = 0; j < bitsForOneByte; j++)
            {
                bits[j] = Convert.ToBoolean((byt >> j) & 0x01);
            }
            return bits;
        }
        public static bool[] ConvertShortToBitBoolean(short val)
        {
            var byt = Convert.ToByte(val);
            return ConvertByteToBitBoolean(byt);
        }
        public static int[] ConvertByteToBitInterger(byte byt)
        {
            const int bitsForOneByte = 8;
            int[] bits = new int[bitsForOneByte];
            for (int j = 0; j < bitsForOneByte; j++)
            {
                bits[j] = ((byt >> j) & 0x01);
            }
            return bits;
        }
        public static string ConvertByteToBinString(byte b, bool isReverse)
        {
            string temp = Convert.ToString(b, 2).PadLeft(8, '0');
            if (isReverse)
            {
                List<char> tempArr = temp.ToList<char>();
                tempArr.Reverse();
                return string.Concat<char>(tempArr);
            }
            return temp;
        }
        public static byte[] ConvertHexStringToByteArray(string Hex, bool isReverse)
        {
            byte[] Bytes = new byte[Hex.Length / 2];
            try
            {

                int[] HexValue = new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05,
                                            0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                            0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

                for (int x = 0, i = 0; i < Hex.Length; i += 2, x += 1)
                {
                    Bytes[x] = (byte)(HexValue[Char.ToUpper(Hex[i + 0]) - '0'] << 4 |
                                      HexValue[Char.ToUpper(Hex[i + 1]) - '0']);
                }
                if (isReverse)
                {
                    return Bytes.Reverse().ToArray();
                }

            }
            catch
            {

            }
            return Bytes;
        }
        public static float[] ArrayConvertByteToFloat(short[] floatArray)
        {
            float[] tempf = new float[floatArray.Length / 2];
            byte[] lo = new byte[2];
            byte[] hi = new byte[2];
            byte[] temp = new byte[4];
            for (int i = 0; i < floatArray.Length; i += 2)
            {
                lo = BitConverter.GetBytes(floatArray[i + 1]);
                hi = BitConverter.GetBytes(floatArray[i]);

                Array.Copy(lo, 0, temp, 0, 2);
                Array.Copy(hi, 0, temp, 2, 2);
                //Array.Reverse(temp);
                tempf[i / 2] = BitConverter.ToSingle(temp, 0);
            }
            return tempf;
        }
        public static byte[] ConvertDoubleToBytes(double value, int decimalPlace = 2, bool isReverse = true)
        {
            byte[] bytes = new byte[2];
            string format = ".".PadRight(decimalPlace + 1, '0');
            int temp = Convert.ToInt16(value.ToString(format).Replace(".", ""));
            return isReverse ? BitConverter.GetBytes(temp).Reverse().ToArray() : BitConverter.GetBytes(temp);
        }
        public static byte[] ConvertIntToBytes(int value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(value >> 8);
            bytes[1] = (byte)(value & 0xFF);
            return bytes;
        }
        public static byte[] ConvertLongToBytes(long value)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytes[i] = (byte)(value & 0xFF);
                value = value >> 8;
            }
            return bytes;
        }
        public static long ConvertBytesToInt(byte[] value, int startIndex, int length)
        {
            if (startIndex >= value.Length)
            {
                throw new IndexOutOfRangeException("ConvertBytesToInt(byte[] value, int startIndex, int length) startIndex out of range!");
            }
            byte[] tempBytes = new byte[length];
            Array.Copy(value, startIndex, tempBytes, 0, length);
            return ConvertBytesToInt(tempBytes);
        }
        public static long ConvertBytesToInt(byte[] value)
        {
            int mask = 0xff;
            long temp = 0;
            long result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                result <<= 8;
                temp = value[i] & mask;
                result |= temp;
            }
            return result;
        }
        public static byte[] ConvertShortToByte(short value)
        {
            byte high = (byte)(0x00FF & (value >> 8));
            byte low = (byte)(0x00FF & value);
            byte[] bytes = new byte[2];
            bytes[0] = high;
            bytes[1] = low;
            return bytes;
        }
        public static double ConvertShortArrayToDouble(short[] valArr)
        {
            double tempf;
            byte[] lo = new byte[2];
            byte[] hi = new byte[2];
            byte[] temp = new byte[4];
            lo = BitConverter.GetBytes(valArr[1]);
            hi = BitConverter.GetBytes(valArr[0]);

            Array.Copy(lo, 0, temp, 0, 2);
            Array.Copy(hi, 0, temp, 2, 2);
            //Array.Reverse(temp);
            tempf = BitConverter.ToSingle(temp, 0);
            return tempf;
        }
    }
}
