using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolveWare_Service_Core.Base.Abstract
{
    public abstract class RESTFulBase : IRESTFul
    {
        public string FilePath
        {
            get;
            set;
        }
        public IList<IElement> DataBase
        {
            get;
            set;
        }

        public bool AddSingleData(IElement data)
        {
            if (this.DataBase.ToList().Exists(x => (x as IElement).Id == (data as IElement).Id))
            {
                return false;
            }

            this.DataBase.Add(data);
            return true;
        }
        public bool DeleteSingleData(IElement data)
        {
            int index = this.DataBase.ToList().FindIndex(x => (x as IElement).Id == (data as IElement).Id);
            if (index < 0)
            {
                return false;
            }

            List<IElement> tempDatas = this.DataBase.ToList();
            tempDatas.RemoveAt(index);

            this.DataBase.Clear();

            foreach (var item in tempDatas)
            {
                this.DataBase.Add(item);
            }

            return true;
        }
        public IElement GetSingleData(string name)
        {
            IElement data = default(IElement);
            if (this.DataBase.Count == 0) return data;

            data = this.DataBase.ToList().Find(x => (x as IElement).Name == name);
            return data;
        }
        public IElement GetSingleData(IElement IElementBase)
        {
            IElement data = default(IElement);
            if (this.DataBase.Count == 0) return data;

            data = this.DataBase.ToList().Find(x => (x as IElement).Name == IElementBase.Name && (x as IElement).Id == IElementBase.Id);
            return data;
        }
        public bool SaveSingleData(IElement data)
        {
            bool isSaveOK = false;
            var result = MessageBox.Show("确认 储存?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return isSaveOK;


            //Check Name
            var checkObj = this.DataBase.ToList().FindAll(x => (x as IElement).Name == (data as IElement).Name);
            if (checkObj.Count > 1)
            {
                MessageBox.Show("已有相同名字的物件");
                return isSaveOK;
            }

            //无相同的物件 直接存
            if (checkObj.Count == 0)
            {
                this.DataBase.Add(data);
            }
            else
            {
                //有相同的物件 抓出来
                int index = this.DataBase.ToList().FindIndex(x => (x as IElement).Id == (data as IElement).Id);
                if (index < 0)
                {
                    MessageBox.Show("已有相同名字的物件 | 储存 失败");
                    return isSaveOK;
                }

                this.DataBase[index] = data;
            }

            XMLHelper.Save<List<IElement>>(DataBase.ToList(), FilePath);
            MessageBox.Show("储存 成功");
            isSaveOK = true;
            return isSaveOK;
        }
    }
}
