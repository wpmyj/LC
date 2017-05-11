using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LC.DAL.Repository.Repositories
{
    public class RepositoryResult
    {
        /// <summary>
        /// True if no handled or unhandled exceptions occurred.
        /// </summary>
        public bool NoErrors { get; set; }

        public string Message { get; set; }
    }
}
