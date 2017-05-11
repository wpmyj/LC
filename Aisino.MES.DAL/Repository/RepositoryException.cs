using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace LC.DAL.Interfaces
{
    public class RepositoryException : Exception
    {
        public List<string> ExceptionStrings { get; set; }
        public RepositoryException() : base()
        {
            ExceptionStrings = new List<string>();
        }

        public RepositoryException(string message, DbEntityValidationException exc) : base(message, exc)
        {
            ExceptionStrings = new List<string>();
            foreach (var item in exc.EntityValidationErrors)
            {
                if (item.ValidationErrors.Count > 0)
                {
                    foreach (var ve in item.ValidationErrors)
                    {
                        ExceptionStrings.Add(ve.PropertyName + ":" + ve.ErrorMessage);
                    }
                }
            }
            
        }

        public RepositoryException(string message, DbUpdateException ex) : base(message, ex)
        {
            ExceptionStrings = new List<string>();
            foreach (var item in ex.Entries)
            {
                var entryResult = item.GetValidationResult();
                foreach (var ve in entryResult.ValidationErrors)
                {
                    ExceptionStrings.Add(ve.PropertyName + ":" + ve.ErrorMessage);
                }
            }
        }

        public RepositoryException(string message, Exception inner)
            : base(message, inner)
        {
            ExceptionStrings = new List<string>();
            
        }

    }
}
