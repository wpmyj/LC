using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LC.DAL.Interfaces;

namespace LC.Service
{
    public class LCServiceException : Exception
    {
        public List<string> ExceptionStrings { get; set; }
        public LCServiceException() : base()
        {
            ExceptionStrings = new List<string>();
        }

        public LCServiceException(string message, RepositoryException exc)
            : base(message, exc)
        {
            ExceptionStrings = new List<string>();
            ExceptionStrings = exc.ExceptionStrings;            
        }

        public LCServiceException(string message, Exception inner)
            : base(message, inner)
        {
            ExceptionStrings = new List<string>();
            
        }

    }
}
