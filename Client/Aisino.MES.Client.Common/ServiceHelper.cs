using LC.Contracts.BaseManager;
using LC.Contracts.ClassManager;
using LC.Contracts.StudentManager;
using LC.Contracts.TeacherManager;
using LC.Service.Contracts.Common;
using LC.Service.Contracts.SysManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfClientProxyGenerator;
using WcfClientProxyGenerator.Async;

namespace Aisino.MES.Client.Common
{
    public class ServiceCode
    {
        #region Common
        public const string CommonService = "Common/MesCommon.svc";
        #endregion

        #region SysManager
        public const string SysMenuService = "SysManager/SysMenuService.svc";
        public const string SysSubSystemService = "SysManager/SysSubSystem.svc";
        public const string SysRightService = "SysManager/SysRight.svc";
        public const string SysUserService = "SysManager/SysUser.svc";
        public const string SysRoleService = "SysManager/SysRole.svc";
        public const string SysModuleService = "SysManager/SysModule.svc";
        public const string SysFunctionService = "SysManager/SysFunction.svc";
        #endregion

        #region BaseManager
        public const string CenterService = "BaseManager/CenterService.svc";
        public const string ClassroomService = "BaseManager/ClassroomService.svc";
        public const string ConsultantService = "BaseManager/ConsultantService.svc";
        public const string StatusService = "BaseManager/StatusService.svc";
        #endregion

        #region ClassManager
        public const string ClassTypeService = "ClassManager/ClassTypeService.svc";
        public const string ClassService = "ClassManager/ClassService.svc";
        public const string ScheduleService = "ClassManager/ScheduleService.svc";
        #endregion

        #region StudentManager
        public const string StudentService = "StudentManager/StudentService.svc";
        #endregion

        #region TeacherManager
        public const string TeacherService = "TeacherManager/TeacherService.svc";
        #endregion

    }

    public class ServiceHelper
    {
        static BasicHttpBinding CreateBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;

            return binding;
        }

        static NetTcpBinding CreateTcpBinding()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;

            return binding;
        }

        #region Common
        public async static Task<IAsyncProxy<ICommonService>> GetCommonService()
        {
            IAsyncProxy<ICommonService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<ICommonService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.CommonService)))));
            return asyncProxy;
        }

        #endregion

        #region SysManager

        public async static Task<IAsyncProxy<IUserModelService>> GetUserService()
        {
            IAsyncProxy<IUserModelService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IUserModelService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.SysUserService)))));
            return asyncProxy;
        }

        public async static Task<IAsyncProxy<IRightModelService>> GetRightService()
        {
            IAsyncProxy<IRightModelService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IRightModelService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.SysRightService)))));
            return asyncProxy;
        }

        public async static Task<IAsyncProxy<IMenuModelService>> GetMenuService()
        {
            IAsyncProxy<IMenuModelService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IMenuModelService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.SysMenuService)))));
            return asyncProxy;
        }

        public async static Task<IAsyncProxy<ISubSystemModelService>> GetSubSystemService()
        {
            IAsyncProxy<ISubSystemModelService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<ISubSystemModelService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.SysSubSystemService)))));
            return asyncProxy;
        }

        public async static Task<IAsyncProxy<IRoleModelService>> GetRoleService()
        {
            IAsyncProxy<IRoleModelService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IRoleModelService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.SysRoleService)))));
            return asyncProxy;
        }

        public async static Task<IAsyncProxy<IModuleModelService>> GetModuleService()
        {
            IAsyncProxy<IModuleModelService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IModuleModelService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.SysModuleService)))));
            return asyncProxy;
        }

        public async static Task<IAsyncProxy<IFunctionModelService>> GetFunctionService()
        {
            IAsyncProxy<IFunctionModelService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IFunctionModelService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.SysFunctionService)))));
            return asyncProxy;
        }
        #endregion

        #region BaseManager
        public async static Task<IAsyncProxy<ICenterService>> GetCenterService()
        {
            IAsyncProxy<ICenterService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<ICenterService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.CenterService)))));
            return asyncProxy;
        }
        public async static Task<IAsyncProxy<IClassroomService>> GetClassroomService()
        {
            IAsyncProxy<IClassroomService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IClassroomService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.ClassroomService)))));
            return asyncProxy;
        }
        public async static Task<IAsyncProxy<IConsultantService>> GetConsultantService()
        {
            IAsyncProxy<IConsultantService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IConsultantService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.ConsultantService)))));
            return asyncProxy;
        }
        public async static Task<IAsyncProxy<IStatusService>> GetStatusService()
        {
            IAsyncProxy<IStatusService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IStatusService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.StatusService)))));
            return asyncProxy;
        }
        #endregion

        #region ClassManager
        public async static Task<IAsyncProxy<IClassTypeService>> GetClassTypeService()
        {
            IAsyncProxy<IClassTypeService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IClassTypeService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.ClassTypeService)))));
            return asyncProxy;
        }

        public async static Task<IAsyncProxy<IClassesService>> GetClassService()
        {
            IAsyncProxy<IClassesService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IClassesService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.ClassService)))));
            return asyncProxy;
        }

        public async static Task<IAsyncProxy<IScheduleService>> GetScheduleService()
        {
            IAsyncProxy<IScheduleService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IScheduleService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.ScheduleService)))));
            return asyncProxy;
        }
        #endregion

        #region studentManager
        public async static Task<IAsyncProxy<IStudentService>> GetStudentService()
        {
            IAsyncProxy<IStudentService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<IStudentService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.StudentService)))));
            return asyncProxy;
        }
        #endregion

        #region teacherManager
        public async static Task<IAsyncProxy<ITeacherService>> GetTeacherService()
        {
            IAsyncProxy<ITeacherService> asyncProxy = await Task.Run(() => WcfClientProxy.CreateAsyncProxy<ITeacherService>(c => c.SetEndpoint(CreateBinding(), new EndpointAddress(ClientHelper.GetEndpointAddress(ServiceCode.TeacherService)))));
            return asyncProxy;
        }
        #endregion
    }
}
