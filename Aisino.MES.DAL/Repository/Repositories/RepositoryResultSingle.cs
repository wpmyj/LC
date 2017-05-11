using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LC.DAL.Repository.Repositories
{
    public class RepositoryResultSingle<T> : RepositoryResult
    {
        public T Entity { get; set; }

        public bool HasValue
        {
            get
            {
                if (Entity == null)
                    return false;

                return true;
            }
        }
    }
}
