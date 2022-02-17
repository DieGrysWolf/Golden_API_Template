using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Entities.Models.DTOs.Errors
{
    public class ErrorDTO
    {
        public int Code { get; set; }
        public string? Type { get; set; }
        public string? Message { get; set; }
    }
}
