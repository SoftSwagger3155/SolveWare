using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using Utilities;

namespace SolveWare_Service_Core.General
{
    public class XMLHelper
    {
        public static void Save<T>(T cfg, string path)
        {
            StreamWriter streamWriter = null;
            bool result = false;
            try
            {
                Serializer.LastError = "";
                Serializer.LastErrorDetail = "";
                XmlSerializer xmlSerializer = new XmlSerializer(cfg.GetType());
                streamWriter = new StreamWriter(path, false);
                xmlSerializer.Serialize(streamWriter, cfg);
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
                result = true;
            }
            catch (Exception innerException)
            {
                Trace.WriteLine("Failed to serialize. Reason: " + innerException.Message);
                Serializer.LastError = innerException.Message;
                Serializer.LastErrorDetail = innerException.ToString();
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                    Serializer.LastErrorDetail = Serializer.LastErrorDetail + Environment.NewLine + "[Inner]" + innerException.ToString();
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
            }
        }
        public static T Load<T>(string path)
        {
            return (T)Serializer.LoadXml(typeof(T), path);
        }

        public static void SaveDictionary<TKey, TValue>(string fileName, SerializableDictionary<TKey, TValue> diction)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<TKey, TValue>));
                xmlFormatter.Serialize(fileStream, diction);
            }
        }
        public static SerializableDictionary<TKey, TValue> LoadDictionary<TKey, TValue>(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<TKey, TValue>));
                return (SerializableDictionary<TKey, TValue>)xmlFormatter.Deserialize(fileStream);
            }
        }
    }


    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public SerializableDictionary() { }
        public void WriteXml(XmlWriter write)       // Serializer
        {
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                write.WriteStartElement("SerializableDictionary");
                write.WriteStartElement("key");
                KeySerializer.Serialize(write, kv.Key);
                write.WriteEndElement();
                write.WriteStartElement("value");
                ValueSerializer.Serialize(write, kv.Value);
                write.WriteEndElement();
                write.WriteEndElement();
            }
        }

        public object FindAll(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)       // Deserializer
        {
            reader.Read();
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("SerializableDictionary");
                reader.ReadStartElement("key");
                TKey tk = (TKey)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue vl = (TValue)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadEndElement();
                this.Add(tk, vl);
                reader.MoveToContent();
            }
            reader.ReadEndElement();

        }
        public XmlSchema GetSchema()
        {
            return null;
        }
    }
}
