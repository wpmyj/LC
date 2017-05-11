using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.StudentModel
{
    public class StudentDisplayModel
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

        public string Status { get; set; }
    }

    public class EntityComparer : IEqualityComparer<StudentDisplayModel>
    {
        public bool Equals(StudentDisplayModel a, StudentDisplayModel b)
        {
            if (Object.ReferenceEquals(a, b)) return true;
            if (Object.ReferenceEquals(a, null) || Object.ReferenceEquals(b, null))
                return false;

            return a.Id == b.Id;
        }

        public int GetHashCode(StudentDisplayModel a)
        {
            if (Object.ReferenceEquals(a, null)) return 0;
            int hashName = a.Name == null ? 0 : a.Name.GetHashCode();
            int hashCode = a.Grade.GetHashCode();

            return hashName ^ hashCode;
        }
    }  
}
