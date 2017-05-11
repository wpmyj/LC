using LC.Model.PageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LC.DAL.Repository.Helpers
{
    /// <summary>
    /// Describes how you want to page results.  It is assumed that all values will be populated.
    /// </summary>
    public class PagingCriteria
    {
        /// <summary>
        /// Number of records per page
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// The page you want to retrieve
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The entity property you want to sort
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// "asc" or "desc"
        /// </summary>
        public string SortDirection { get; set; }

        public static implicit operator PagingCriteria(AisinoPagingCriteria apc)
        {
            PagingCriteria paging = new PagingCriteria();
            paging.PageSize = apc.PageSize;
            paging.PageNumber = apc.PageNumber;
            paging.SortBy = apc.SortBy;
            paging.SortDirection = apc.SortDirection;  
            return paging;
        }
    }
}

