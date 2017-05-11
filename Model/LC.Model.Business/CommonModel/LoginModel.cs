using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business
{
    public class LoginModel
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户照片
        /// </summary>
        public byte[] UserPicture { get; set; }
        /// <summary>
        /// 用户所在部门编号
        /// </summary>
        public string DepartmentCode { get; set; }
        /// <summary>
        /// 用户所在部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 所属企业编号
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 所属企业名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 首次登录是否需要修改密码
        /// </summary>
        public bool NeedChangePassword { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// mac地址
        /// </summary>
        public string MacAddress { get; set; }
    }
}
