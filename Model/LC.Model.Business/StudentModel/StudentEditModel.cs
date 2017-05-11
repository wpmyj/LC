using LC.Model.Business.BaseModel;
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.StudentModel
{
    public class StudentEditModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public System.DateTime Birthdate { get; set; }
        public string Momsname { get; set; }
        public string Dadsname { get; set; }
        public string Momsphone { get; set; }
        public string Dadsphone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ExtraInfo { get; set; }
        public string OriginalClass { get; set; }
        public string Grade { get; set; }
        public string School { get; set; }
        public int RemainingBalance { get; set; }
        public string RelationShip { get; set; }
        public int CenterId { get; set; }
        public string GoogleContactsId { get; set; }
        public string RfidTag { get; set; }
        public string ConsultantName { get; set; }
        public int ConsultantId { get; set; }
        public int StatusId { get; set; }
        public int NeedRecharge { get; set; }
        public Decimal ConsultantRate { get; set; }
        public string StatusText { get; set; }

        public List<int> ClassesId { get; set; }

        public List<string> ClassPath { get; set; }

        public void InitEditModel(student studentEntity)
        {
            this.Address = studentEntity.address;
            this.Birthdate = studentEntity.students_birthdate;
            this.CenterId = studentEntity.center_id.HasValue ? studentEntity.center_id.Value : 0 ;
            this.Dadsname = studentEntity.dads_name;
            this.Dadsphone = studentEntity.dads_phone;
            this.Email = studentEntity.email;
            this.ExtraInfo = studentEntity.extra_info;
            this.GoogleContactsId = studentEntity.google_contacts_id;
            this.Grade = studentEntity.grade;
            this.Id = studentEntity.student_id;
            this.Momsname = studentEntity.moms_name;
            this.Momsphone = studentEntity.moms_phone;
            this.Name = studentEntity.students_name;
            this.Nickname = studentEntity.students_nickname;
            this.OriginalClass = studentEntity.original_class;
            this.RemainingBalance = studentEntity.remaining_balance.HasValue ? studentEntity.remaining_balance.Value : 0;
            this.RfidTag = studentEntity.rfid_tag;
            this.School = studentEntity.school;
            this.ConsultantName = studentEntity.consultants.Count > 0 ? studentEntity.consultants.First().name : "";
            this.ConsultantId = studentEntity.consultants.Count > 0 ? studentEntity.consultants.First().consultant_id : 0;
            this.ConsultantRate = studentEntity.consultant_check_rate.HasValue ? studentEntity.consultant_check_rate.Value : 0;
            this.RelationShip = studentEntity.relationship;
            this.StatusId = studentEntity.status;
            this.StatusText = studentEntity.status1.description;
            if(studentEntity.classess != null)
            {
                ClassesId = new List<int>();
                ClassPath = new List<string>();
                foreach(classes cls in studentEntity.classess)
                {
                    if (cls.is_active)
                    {
                        this.NeedRecharge += cls.last_count * cls.parentClassTypes.unit_price;
                        ClassesId.Add(cls.class_id);
                        ClassPath.Add(string.Format("{0}|{1}", cls.parentClassTypes.name, cls.class_name));
                    }
                }
            }
        }
    }
}
