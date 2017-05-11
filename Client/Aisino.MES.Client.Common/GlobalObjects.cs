
using LC.Model.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aisino.MES.Client.Common
{
    public static class GlobalObjects
    {
        //服务连接地址
        public static string ServiceHostAddress { get; set; }

        public static string ServiceTcpAddress { get; set; }

        //服务器是否已连接
        public static bool IsConnected { get; set; }

        //当前登录信息
        public static LoginModel currentLoginUser { get; set; }


        //是否开启取消确认
        public static bool blOpenCancelWarning = true;

        //分页信息
        //public static PageInfo pageInfo  = new PageInfo(10,1);

        /// <summary>
        /// 自动关闭提示框等待时间
        /// </summary>
        public static int AutoClose = 2000;

        public static string ReportDataSourceConnectString = @"Data Source=192.168.10.101;Initial Catalog=aisino_mes;Persist Security Info=True;User ID=grainer;Password=password;MultipleActiveResultSets=True";

        public static string CurrentLanguage = "zh-CN";

        #region status类型
        public static string StudentStatus = "student_status";
        public static string TeacherStatus = "teacher_status";
        public static string ScheduleStatus = "schedule_status";
        #endregion

        #region right
        public static string TeacherRight = "teacher";
        #endregion
    }
}
