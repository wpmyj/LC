using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.BaseModel
{
    public class ConsultantCheckModel
    {
        //返回会籍顾问需要结算的对应每月的金额
        //对应月份
        public string month { get; set; }
        //金额
        public decimal money { get; set; }
        //会籍顾问id
        public int id { get; set; }
        //会籍顾问名称
        public string name { get; set; }
        //合计抽成学员数量
        public int studentNum { get; set; }
    }
}
