using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business
{
    [DataContract(IsReference = true)]
    public abstract class ModelBase
    {
        public abstract new string ToString();
    }
}
