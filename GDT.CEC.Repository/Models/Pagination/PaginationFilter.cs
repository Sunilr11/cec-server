using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository.Models.Pagination
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public PaginationFilter(int pageNumber, int pageSize, int totalRecords)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 0 ? pageSize : totalRecords;
            var totalNoPages = ((double)totalRecords / (double)this.PageSize);
            this.TotalPages = totalNoPages > 0 ? Convert.ToInt32(Math.Ceiling(totalNoPages)) : 0;
        }
    }
}
