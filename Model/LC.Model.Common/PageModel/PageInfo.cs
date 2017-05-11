using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.PageModel
{
    /// <summary>
	/// 分页信息
	/// </summary>
	public class PageInfo 
    {
        /// <summary>
        /// Total number of subsets
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Total number of items in the superset
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// One-based index of this subset
        /// </summary>
        public int PageNumber { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public bool IsFirstPage { get; set; }

        public bool IsLastPage { get; set; }

        public PageInfo(int totalItemCount, int pageSize, int pageNumber)
        {
            PageCount = (int)Math.Ceiling(totalItemCount / (float)pageSize);
            PageNumber = pageNumber;
            TotalItemCount = totalItemCount;
            HasPreviousPage = pageNumber > 1;
            HasNextPage = pageNumber < PageCount;
        }

	}//end PageInfo

}//end namespace CommonModel