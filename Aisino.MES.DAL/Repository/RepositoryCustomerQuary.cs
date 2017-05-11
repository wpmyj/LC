using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace LC.DAL.Repository
{
    public class RepositoryCustomerQuary
    {
        public DbContext ctx { get; set; }

        public RepositoryCustomerQuary(DbContext context)
        {
            ctx = context;
        }

        public IEnumerable<T> QueryCustomerObjectByESql<T>(string esql)
        {
            return ctx.Database.SqlQuery<T>(esql);
        }
    }
}
