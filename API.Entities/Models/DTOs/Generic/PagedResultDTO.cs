using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Entities.Models.DTOs.Generic
{
    public class PagedResultDTO<T> : ResultDTO<List<T>>
    {
        public int Page { get; set; }
        public int ResultCount { get; set; }
        public int ResultsPerPage { get; set; }
    }
}
