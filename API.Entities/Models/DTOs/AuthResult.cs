using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Entities.Models.DTOs
{
    public  class AuthResult
    {
        public string? Token { get; set; }
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }
    }
}
