using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model
{
    [DataContractAttribute]
    public class LCFault
    {
        private string report;

        [DataMemberAttribute]
        public string Message
        {
            get { return this.report; }
            set { this.report = value; }
        }
        public LCFault(string message)
        {
            this.report = message;
        }
    }
}
