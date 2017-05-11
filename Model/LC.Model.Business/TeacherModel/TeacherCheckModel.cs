using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.TeacherModel
{
    public class TeacherCheckModel
    {
        //返回教师需要结算的对应每月的金额
        //对应月份
        public string month { get; set; }
        //金额
        public decimal money { get; set; }
    }
}
