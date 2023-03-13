using SolveWare_Service_Core.Base.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Motor.Data
{
    [Serializable]
    public class MtrMisc : ModelBase
    {
        private MotorRelativeButtonImage rightRelativeButtonImage = MotorRelativeButtonImage.右箭头;
        private MotorRelativeButtonImage leftRelativeButtonImage = MotorRelativeButtonImage.左箭头;
        private MotorJogButtonImage rightJogButtonImage = MotorJogButtonImage.右箭头;
        private MotorJogButtonImage leftJogButtonImage = MotorJogButtonImage.左箭头;
        private MotorUnit motorUnit = MotorUnit.mm;


        [DisplayName("1 右 相对位置键")]
        public MotorRelativeButtonImage RightRelativeButtonImage
        {
            get => rightRelativeButtonImage;
            set => UpdateProper(ref rightRelativeButtonImage, value);
        }
        [DisplayName("2 左 相对位置键")]
        public MotorRelativeButtonImage LeftRelativeButtonImage
        {
            get => leftRelativeButtonImage;
            set => UpdateProper(ref leftRelativeButtonImage, value);
        }
        [DisplayName("3 右 Jog位置键")]
        public MotorJogButtonImage RightJogButtonImage
        {
            get => rightJogButtonImage;
            set => UpdateProper(ref rightJogButtonImage, value);
        }
        [DisplayName("4 左 Jog位置键")]
        public MotorJogButtonImage LeftJogButtonImage
        {
            get => leftJogButtonImage;
            set => UpdateProper(ref leftJogButtonImage, value);
        }
        [DisplayName("5 单位显示")]
        public MotorUnit MotorUnit
        {
            get => motorUnit;
            set => UpdateProper(ref motorUnit, value);
        }

    }

    public enum MotorRelativeButtonImage
    {
        左璇,
        右璇,
        左箭头,
        右箭头,
        上箭头,
        下箭头
    }
    public enum MotorJogButtonImage
    {
        左璇,
        右璇,
        左箭头,
        右箭头,
        上箭头,
        下箭头
    }
    public enum MotorUnit
    {
        Deg,
        mm
    }
}
