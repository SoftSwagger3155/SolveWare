using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Winform_V5
{

    public class Student
    {
        [Category("基本资料")]
        [DisplayName("姓名")]
        [DefaultValue("XXX")]
        public string Name { get; set; } = "XXX";

        [Category("基本资料")]
        [DisplayName("年纪")]
        [DefaultValue(5)]
        public int Age { get; set; } = 5;


        [Category("基本资料")]
        [DisplayName("性别")]
        [TypeConverter(typeof(genderItem))]
        public string Genda { get; set; }

        [Category("其他事项")]
        [DisplayName("TODOs")]
        public string ToDos { get; set; }
    }

    public class genderItem : StringConverter
    {
        //true enable,false disable
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "男", "女" }); //编辑下拉框中的items
        }


        //true: disable text editting.    false: enable text editting;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
    //public class ExtensionMethod
    //{
    //    public static DataTable ListToTable<T>(List<T> list, bool isStoreDB = true)
    //    {
    //        Type tp = typeof(T);
    //        PropertyInfo[] proInfos = tp.GetProperties();
    //        DataTable dt = new DataTable();
    //        foreach (var item in proInfos)
    //        {
    //            dt.Columns.Add(item.Name, item.PropertyType); //添加列明及对应类型
    //        }
    //        foreach (var item in list)
    //        {
    //            DataRow dr = dt.NewRow();
    //            foreach (var proInfo in proInfos)
    //            {
    //                object obj = proInfo.GetValue(item);
    //                if (obj == null)
    //                {
    //                    continue;
    //                }
    //                //if (obj != null)
    //                // {
    //                if (isStoreDB && proInfo.PropertyType == typeof(DateTime) && Convert.ToDateTime(obj) < Convert.ToDateTime("1753-01-01"))
    //                {
    //                    continue;
    //                }
    //                // dr[proInfo.Name] = proInfo.GetValue(item);
    //                dr[proInfo.Name] = obj;
    //                // }
    //            }
    //            dt.Rows.Add(dr);
    //        }
    //        return dt;
    //    }
    //}
}
