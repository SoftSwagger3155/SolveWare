using SolveWare_Service_Core.Base.Abstract;
using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.General;
using SolveWare_Service_Core.Manager.Base.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Manager.Business
{
    public class Resource_Data_Manager<TData> : RESTFulBase, IDataResourceProvider
    {
        public Type ResourceKey { get; private set; }
        public string Name { get; set; }

        public Resource_Data_Manager()
        {
            Name = $"Resource_Data_{typeof(TData).Name}";
            this.FilePath = Path.Combine(SystemPath.GetSystemPath, $"{this.Name}.xml");
        }
        public Resource_Data_Manager(Type resourceKey)
        {
            Name = $"Resource_Data_{typeof(TData).Name}";
            ResourceKey = resourceKey;
            this.FilePath = Path.Combine(SystemPath.GetSystemPath, $"{this.Name}.xml");
        }

        public void AssignResourceKey(Type key)
        {
            this.ResourceKey = key;
        }

        public IList<IElement> Get_All_Items()
        {
            return DataBase;
        }

        public IList<string> Get_All_Item_Name()
        {
            List<string> list = new List<string>();
            DataBase.ToList().ForEach(x => list.Add(x.Name));

            return list;
        }

        public IElement Get_Single_Item(string name)
        {
            return Get_All_Items().FirstOrDefault(x => x.Name == name);
        }

        public bool Initialize()
        {
            bool isOk = false;

            try
            {
                do
                {
                    if (Load() == false) break;

                } while (false);
            }
            catch (Exception ex)
            {
                SolveWare.Core.MMgr.Infohandler.LogMessage($"初始化 {Name} 失败\r\n{ex.Message}", true, true);
            }


            return isOk;
        }

        public bool Load()
        {
            bool isOk = false;
            if (string.IsNullOrEmpty(FilePath)) throw new Exception($"无档案路径");
            this.DataBase.Clear();

            try
            {
                if (SystemPath.IsFileExisted(FilePath))
                {
                    var tempList = XMLHelper.Load<List<TData>>(FilePath);
                    foreach (var item in tempList)
                    {
                        this.DataBase.Add(item as IElement);
                    }
                    isOk = true;
                }
            }
            catch (Exception ex)
            {
                SolveWare.Core.MMgr.Infohandler.LogMessage($"{this.GetType().Name} 资料下载失败\r\n{ex.Message}", true, true);
            }

            return isOk;
        }

        public void Save()
        {
            try
            {
                XMLHelper.Save<List<TData>>((this.DataBase.ToList() as List<TData>), FilePath);
                SolveWare.Core.MMgr.Infohandler.LogMessage($"储存 成功", isWindowShow: true);
            }
            catch (Exception ex)
            {
                SolveWare.Core.MMgr.Infohandler.LogMessage($"储存 失败{Environment.NewLine}{ex.Message}", true);
            }
        }
    }
}
