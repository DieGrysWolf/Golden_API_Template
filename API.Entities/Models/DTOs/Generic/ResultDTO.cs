using API.Entities.Models.DTOs.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Entities.Models.DTOs.Generic
{
    public class ResultDTO<T> // Single Item Return
    {
        public T? Content { get; set; }
        public ErrorDTO? Error { get; set; }
        public bool Success => Error == null;
        public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
    }
}
