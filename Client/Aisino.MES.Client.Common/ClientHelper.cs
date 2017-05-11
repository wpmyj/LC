using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Management;
//using Aisino.MES.Model.DeviceModels;
using System.Xml;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfClientProxyGenerator.Async;
using WcfClientProxyGenerator;
using System.Reflection;
//using Aisino.MES.Led.Model;

namespace Aisino.MES.Client.Common
{
    public class ReportServerConfig
    {
        private string serverURL;

        public string ServerURL
        {
            get { return serverURL; }
            set { serverURL = value; }
        }

        private string reportPath;

        public string ReportPath
        {
            get { return reportPath; }
            set { reportPath = value; }
        }


        private string networkCredentialName;

        public string NetworkCredentialName
        {
            get { return networkCredentialName; }
            set { networkCredentialName = value; }
        }

        private string networkCredentialPwd;

        public string NetworkCredentialPwd
        {
            get { return networkCredentialPwd; }
            set { networkCredentialPwd = value; }
        }

        private string dataSourceName;

        public string DataSourceName
        {
            get { return dataSourceName; }
            set { dataSourceName = value; }
        }

        private string dataSourcePwd;

        public string DataSourcePwd
        {
            get { return dataSourcePwd; }
            set { dataSourcePwd = value; }
        }

        
    }

    public class ClientHelper
    {
        #region property
        public static XmlDocument ServiceConfigDom;
        public static string ServiceHostAddress;

        //private static SysDepartment _orgDept;
        ///// <summary>
        ///// 部门编号
        ///// </summary>
        //public static SysDepartment OrgDept
        //{
        //    get { return _orgDept; }
        //    set { _orgDept = value; }
        //}

        //private static SysDepartment _originalDept;
        ///// <summary>
        ///// 所属组织机构号
        ///// </summary>
        //public static SysDepartment OriginalDept
        //{
        //    get { return ClientHelper._originalDept; }
        //    set { ClientHelper._originalDept = value; }
        //}

        //private static SysUser _loginUser;
        ///// <summary>
        ///// 当前登录用户
        ///// </summary>
        //public static SysUser LoginUser
        //{
        //    get { return _loginUser; }
        //    set { _loginUser = value; }
        //}

        //private static SysAcount _loginAcount;
        ///// <summary>
        ///// 当前登录账号
        ///// </summary>
        //public static SysAcount LoginAcount
        //{
        //    get { return _loginAcount; }
        //    set { _loginAcount = value; }
        //}

        private static ReportServerConfig _reportServerCof;

        public static ReportServerConfig ReportServerCof
        {
            get { return ClientHelper._reportServerCof; }
            set { ClientHelper._reportServerCof = value; }
        }


        #endregion

        #region method
        //public static void GetUnitContainer()
        //{
        //    AisinoMesContainer = new UnityContainer();
        //    UnityConfigurationSection sysManagerSection = (UnityConfigurationSection)GetSysManagerUnityConfig().GetSection("MesManagerUnity");
        //    sysManagerSection.Configure(AisinoMesContainer, "AisinoMesContainer");
        //    //其余配置文件可以陆续添加
        //}

        //public static Configuration GetSysManagerUnityConfig()
        //{
        //    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
        //    fileMap.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + @"\MesManagerUnity.config";
        //    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

        //    return config;
        //}

        //public static T GetServiceInstance<T>(string name)
        //{
        //    T aisinoServiceInstance = AisinoMesContainer.Resolve<T>(name);
        //    return aisinoServiceInstance;
        //}

        //public static SysAcount Login(string name)
        //{
        //    ISysDepartmentUserService myServiceInstance = ClientHelper.GetServiceInstance<ISysDepartmentUserService>("sysDepartmentUserService");
        //    SysAcount loginAcount = myServiceInstance.GetSysAcountByLoginName(name);
        //    if (loginAcount.SysUser != null)
        //    {
        //        OrgDept = loginAcount.SysUser.UserDepartment;
        //        OriginalDept = OrgDept;
        //        //while (OriginalDept.dept_type == Model.DeparrmentType.下属部门)
        //        //{
        //        //    OriginalDept = OriginalDept.ParentDepartment;
        //        //}
        //    }
        //    return loginAcount;
        //}

        /// <summary>
        /// 将byte[]转换为Image
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>Image</returns>
        public static Image ReadImage(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(ms);
            ms.Close();
            return (Image)obj;
        }
        /// <summary>
        /// 将Image转换为byte[]
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>byte[]</returns>
        public static byte[] ConvertImage(Image image)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, (object)image);
            ms.Close();
            return ms.ToArray();
        }

        ///// <summary>
        ///// 根据传入的编号增加组织机构编码
        ///// </summary>
        ///// <param name="code">传入编号</param>
        ///// <returns></returns>
        //public static string AddOrgCode(string code)
        //{
        //    return OriginalDept.DepartmentCode + code;
        //}

        //public static string GetBaseCode(string code)
        //{
        //    return code.Remove(0, OriginalDept.DepartmentCode.Length);
        //}

        public static string DoVoiceCast(string input)
        {
            if (String.IsNullOrEmpty(input)) return ""; // 如果输入为空则返回空
            string src = input; // 原始字符串的副本
            string ret = ""; // 准备返回的结果字符串
            for (int i = 0; i < src.Length; i++)
            {
                switch (src[i])
                {
                    case '0':
                        ret += "零";
                        break;
                    case '1':
                        ret += "一";
                        break;
                    case '2':
                        ret += "二";
                        break;
                    case '3':
                        ret += "三";
                        break;
                    case '4':
                        ret += "四";
                        break;
                    case '5':
                        ret += "五";
                        break;
                    case '6':
                        ret += "六";
                        break;
                    case '7':
                        ret += "七";
                        break;
                    case '8':
                        ret += "八";
                        break;
                    case '9':
                        ret += "九";
                        break;
                    default:
                        ret += src[i] + " "; // 其他文字逐个复制过来
                        break;
                }
            }
            return ret;
        }

        public static string FormatXml(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            StringBuilder sb = new StringBuilder();
            System.IO.TextWriter tr = new System.IO.StringWriter(sb);
            XmlTextWriter wr = new XmlTextWriter(tr);
            wr.Formatting = Formatting.Indented;
            doc.Save(wr);
            wr.Close();
            return sb.ToString();
        }
        #endregion

        public static string GetMacAddress()
        {
            try
            {
                List<string> macList = new List<string>();
                ManagementClass mc;
                mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if (mo["IPEnabled"].ToString() == "True")
                        return mo["MacAddress"].ToString();
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据用户获得菜单权限
        /// </summary>
        /// <param name="userLoginInfo"></param>
        public static string GetMenuInfoByUser()
        {
            string strSysMenu = string.Empty;
            //foreach (SysRoleUser sysRoleUser in _loginAcount.ro)
            //{
            //    if (sysRoleUser.SysRole == null)
            //    {
            //        return string.Empty;
            //    }
            //    SysRole sr = sysRoleUser.SysRole;

            //    foreach (SysRoleRight sysRoleRight in sr.SysRoleRights)
            //    {
            //        SysRight srr = sysRoleRight.SysRight;
            //        foreach (SysRightMenu sysRightMenu in srr.SysRightMenus)
            //        {
            //            if (strSysMenu.Trim().Length == 0)
            //            {
            //                strSysMenu = sysRightMenu.SysMenu.menu_code;
            //            }
            //            else
            //            {
            //                strSysMenu = strSysMenu + "," + sysRightMenu.SysMenu.menu_code;
            //            }
            //        }
            //    }
            //}
            return strSysMenu.Trim();
        }

        /// <summary>
        /// 获取服务器连接地址
        /// </summary>
        public static void GetServiceHostInfo()
        {
            AppConfigs ac = new AppConfigs();
            GlobalObjects.ServiceHostAddress = ac.getAppSettingsByKey("ServiceHostAddress").First();
        }

        public static void GetClientLan()
        {
            AppConfigs ac = new AppConfigs();
            GlobalObjects.CurrentLanguage = ac.getAppSettingsByKey("Lang").First();
        }

        public static void GetReportServerConfig()
        {
            ReportServerCof = new ReportServerConfig();
            AppConfigs ac = new AppConfigs();
            ReportServerCof.ReportPath = ac.getAppSettingsByKey("ReportPath").First();
        }

        public static void SetReportServerConfig()
        {
            AppConfigs ac = new AppConfigs();
            ac.updateAppSettings("ReportPath", ReportServerCof.ReportPath);
        }

        #region 服务通道接入

        public static Uri GetEndpointAddress(string serviceName)
        {
            return new System.Uri(GlobalObjects.ServiceHostAddress + serviceName);
        }

        public static Uri GetEndpointTcpAddress(string serviceName)
        {
            return new System.Uri(GlobalObjects.ServiceTcpAddress + serviceName);
        }
            
        public static T GetFactoryChannel<T>(string serviceName)
        {
            ChannelFactory<T> factory = new ChannelFactory<T>();
            factory.Endpoint.Address = new EndpointAddress(new System.Uri(ServiceHostAddress + serviceName));
            factory.Endpoint.Binding = new BasicHttpBinding();

            T channel = factory.CreateChannel();

            ((IClientChannel)channel).Open();

            return channel;
}

        public static void Close<T>(T channel)
        {
            ((IClientChannel)channel).Close();
        }

        public static void Abort<T>(T channel)
        {
            ((IClientChannel)channel).Abort();
        }
        #endregion

    }

}
