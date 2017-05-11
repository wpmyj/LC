using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace Aisino.MES.Client.Common
{
    public class AppConfigs
    {
        private string strFile;
        /// <summary>
        /// ���캯�������ö�Ӧ��app.config�ļ�
        /// </summary>
        public AppConfigs()
        {
            strFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        }

        #region ����appSettings�ڵ�
        /// <summary>
        /// ������appsettings�ڵ��������о�����ͬkey��ֵ
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>������ͬkey��ֵ</returns>
        public List<string> getAppSettingsByKey(string key)
        {
            List<string> revalue = new List<string>();
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNodeList c_list = dc.SelectSingleNode("//appSettings").ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                XmlElement xe = (XmlElement)xn;
                if(xe.Attributes["key"].InnerText == key)
                    revalue.Add(xe.Attributes["value"].InnerText) ;
            }
            return revalue;
        }

        /// <summary>
        /// ��appsettings��������һ��ֵ
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">��Ӧ��ֵ</param>
        public void addAppSettings(string key, string value)
        {
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNode node = dc.SelectSingleNode("//appSettings");
            try
            {
                XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", key));
                elem = dc.CreateElement("add");
                elem.SetAttribute("key", key);
                elem.SetAttribute("value", value);
                node.AppendChild(elem);
                dc.Save(strFile);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// ��������������������ͬkey��ֵ��������Ҫ����value�ж��Ƿ������ͬ�ģ���Ҫ���ڵ�¼��
        /// </summary>
        /// <param name="value">��Ҫ�����Ƿ���ڵ�ֵ</param>
        /// <returns>�Ƿ����</returns>
        public bool valueExistInAppSettings(string value)
        {
            bool res = false;
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNodeList c_list = dc.SelectSingleNode("//appSettings").ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.Attributes["value"].InnerText == value)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        /// <summary>
        /// ����key�Ƿ��Ѿ����ڣ����ڵ�¼����ʹ�ø÷���
        /// </summary>
        /// <param name="key">��Ҫ�жϵ�key</param>
        /// <returns>�Ƿ����</returns>
        public bool keyExistInAppSettings(string key)
        {
            bool res = false;
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNodeList c_list = dc.SelectSingleNode("//appSettings").ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.Attributes["key"].InnerText == key)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        /// <summary>
        /// ɾ��appsettings�е�ĳ��ֵ
        /// </summary>
        /// <param name="key">��Ҫɾ����key</param>
        public void delAppSettings(string key)
        {
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNode node = dc.SelectSingleNode("//appSettings");

            XmlNodeList c_list = node.ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.Attributes["key"].InnerText == key)
                {
                    node.RemoveChild(xn);
                    break;
                }
            }
            dc.Save(strFile);
        }

        /// <summary>
        /// ����appsettings����ĳ��ֵ
        /// </summary>
        /// <param name="key">��Ҫ����ֵ��key</param>
        /// <param name="value">��Ҫ���µ�value</param>
        public void updateAppSettings(string key, string value)
        {
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNode node = dc.SelectSingleNode("//appSettings");

            XmlNodeList c_list = node.ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.Attributes["key"].InnerText == key)
                {
                    xe.Attributes["value"].InnerText = value;
                    break;
                }
            }
            dc.Save(strFile);
        }
        #endregion

        #region ����connectionStrings�ڵ�
        /// <summary>
        /// ������connectionStrings�ڵ��������о�����ͬname��ֵ
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>������ͬname��ֵ</returns>
        public List<string> getConnectionStringsByName(string name)
        {
            List<string> reConstr = new List<string>();
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNodeList c_list = dc.SelectSingleNode("//connectionStrings").ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                //�ж��Ƿ�Ϊע����
                if (xn.Name == "#comment")
                {
                    continue;
                }

                XmlElement xe = (XmlElement)xn;
                if (xe.Attributes["name"].InnerText == name)
                    reConstr.Add(xe.Attributes["connectionString"].InnerText);
            }
            return reConstr;
        }

        /// <summary>
        /// ��connectionStrings��������һ��ֵ
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="connectionString">��Ӧ��ֵ</param>
        public void addConnectionStrings(string name, string connectionString)
        {
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNode node = dc.SelectSingleNode("//connectionStrings");
            try
            {
                XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@name='{0}']", name));
                elem = dc.CreateElement("add");
                elem.SetAttribute("name", name);
                elem.SetAttribute("connectionString", connectionString);
                node.AppendChild(elem);
                dc.Save(strFile);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// ��������������������ͬname��ֵ��������Ҫ����connectionString�ж��Ƿ������ͬ�ģ���Ҫ���ڵ�¼��
        /// </summary>
        /// <param name="connectionString">��Ҫ�����Ƿ���ڵ�ֵ</param>
        /// <returns>�Ƿ����</returns>
        public bool valueExistInConnectionStrings(string connectionString)
        {
            bool res = false;
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNodeList c_list = dc.SelectSingleNode("//connectionStrings").ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                //�ж��Ƿ�Ϊע����
                if (xn.Name == "#comment")
                {
                    continue;
                }
                XmlElement xe = (XmlElement)xn;
                if (xe.Attributes["connectionString"].InnerText == connectionString)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        /// <summary>
        /// ����name�Ƿ��Ѿ����ڣ����ڵ�¼����ʹ�ø÷���
        /// </summary>
        /// <param name="name">��Ҫ�жϵ�name</param>
        /// <returns>�Ƿ����</returns>
        public bool nameExistInConnectionStrings(string name)
        {
            bool res = false;
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNodeList c_list = dc.SelectSingleNode("//connectionStrings").ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                //�ж��Ƿ�Ϊע����
                if (xn.Name == "#comment")
                {
                    continue;
                }

                XmlElement xe = (XmlElement)xn;
                if (xe.Attributes["name"].InnerText == name)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        /// <summary>
        /// ɾ��appsettings�е�ĳ��ֵ
        /// </summary>
        /// <param name="name">��Ҫɾ����name</param>
        public void delConnectionStrings(string name)
        {
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNode node = dc.SelectSingleNode("//connectionStrings");

            XmlNodeList c_list = node.ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                //�ж��Ƿ�Ϊע����
                if (xn.Name == "#comment")
                {
                    continue;
                }

                XmlElement xe = (XmlElement)xn;
                if (xe.Attributes["name"].InnerText == name)
                {
                    node.RemoveChild(xn);
                    break;
                }
            }
            dc.Save(strFile);
        }

        /// <summary>
        /// ����ConnectionStrings����ĳ��ֵ
        /// </summary>
        /// <param name="name">��Ҫ����ֵ��name</param>
        /// <param name="connectionString">��Ҫ���µ�connectionString</param>
        public void updateConnectionStrings(string name, string connectionString)
        {
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNode node = dc.SelectSingleNode("//connectionStrings");

            XmlNodeList c_list = node.ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                //�ж��Ƿ�Ϊע����
                if (xn.Name == "#comment")
                {
                    continue;
                }

                XmlElement xe = (XmlElement)xn;
                if (xe.Attributes["name"].InnerText == name)
                {
                    xe.Attributes["connectionString"].InnerText = connectionString;
                    break;
                }
            }
            dc.Save(strFile);
        }
        #endregion
    }

}
