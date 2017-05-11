using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LC.DAL.Repository.Helpers;

namespace LC.DAL.Repository.Repositories
{
    public class RepositoryResultList<T> : RepositoryResult
    {
        public PagedMetadata PagedMetadata { get; set; }

        public IEnumerable<T> Entities { get; set; }
    }
}
