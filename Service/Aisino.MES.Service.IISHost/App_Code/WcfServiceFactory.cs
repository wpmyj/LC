using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Unity.Wcf;
using System.Configuration;
using System;

namespace LC.Service.IISHost
{
	public class WcfServiceFactory : UnityServiceHostFactory
    {
        protected override void ConfigureContainer(IUnityContainer container)
        {
            UnityConfigurationSection sysManagerSection = (UnityConfigurationSection)GetMesManagerUnityConfig().GetSection("LCManagerUnity");
            sysManagerSection.Configure(container, "LCContainer");
        }

        private Configuration GetMesManagerUnityConfig()
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + @"\LCManagerUnity.config";
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            return config;
        }
    }    
}