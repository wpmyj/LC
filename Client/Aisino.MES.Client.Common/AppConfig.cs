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
        /// 构造函数，设置对应的app.config文件
        /// </summary>
        public AppConfigs()
        {
            strFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        }

        #region 设置appSettings节点
        /// <summary>
        /// 返回在appsettings节点下面所有具有相同key的值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>所有相同key的值</returns>
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
        /// 在appsettings下面新增一个值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">对应的值</param>
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
        /// 由于现在里面存放了由相同key的值，所以需要根据value判断是否存在相同的，主要用于登录名
        /// </summary>
        /// <param name="value">需要检查的是否存在的值</param>
        /// <returns>是否存在</returns>
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
        /// 查找key是否已经存在，对于登录名不使用该方法
        /// </summary>
        /// <param name="key">需要判断的key</param>
        /// <returns>是否存在</returns>
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
        /// 删除appsettings中的某个值
        /// </summary>
        /// <param name="key">需要删除的key</param>
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
        /// 更新appsettings下面某个值
        /// </summary>
        /// <param name="key">需要更新值的key</param>
        /// <param name="value">需要更新的value</param>
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

        #region 设置connectionStrings节点
        /// <summary>
        /// 返回在connectionStrings节点下面所有具有相同name的值
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>所有相同name的值</returns>
        public List<string> getConnectionStringsByName(string name)
        {
            List<string> reConstr = new List<string>();
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNodeList c_list = dc.SelectSingleNode("//connectionStrings").ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                //判断是否为注释行
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
        /// 在connectionStrings下面新增一个值
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="connectionString">对应的值</param>
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
        /// 由于现在里面存放了由相同name的值，所以需要根据connectionString判断是否存在相同的，主要用于登录名
        /// </summary>
        /// <param name="connectionString">需要检查的是否存在的值</param>
        /// <returns>是否存在</returns>
        public bool valueExistInConnectionStrings(string connectionString)
        {
            bool res = false;
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNodeList c_list = dc.SelectSingleNode("//connectionStrings").ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                //判断是否为注释行
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
        /// 查找name是否已经存在，对于登录名不使用该方法
        /// </summary>
        /// <param name="name">需要判断的name</param>
        /// <returns>是否存在</returns>
        public bool nameExistInConnectionStrings(string name)
        {
            bool res = false;
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNodeList c_list = dc.SelectSingleNode("//connectionStrings").ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                //判断是否为注释行
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
        /// 删除appsettings中的某个值
        /// </summary>
        /// <param name="name">需要删除的name</param>
        public void delConnectionStrings(string name)
        {
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNode node = dc.SelectSingleNode("//connectionStrings");

            XmlNodeList c_list = node.ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                //判断是否为注释行
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
        /// 更新ConnectionStrings下面某个值
        /// </summary>
        /// <param name="name">需要更新值的name</param>
        /// <param name="connectionString">需要更新的connectionString</param>
        public void updateConnectionStrings(string name, string connectionString)
        {
            XmlDocument dc = new XmlDocument();
            dc.Load(strFile);

            XmlNode node = dc.SelectSingleNode("//connectionStrings");

            XmlNodeList c_list = node.ChildNodes;
            foreach (XmlNode xn in c_list)
            {
                //判断是否为注释行
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
