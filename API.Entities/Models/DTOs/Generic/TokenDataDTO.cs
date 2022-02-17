using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Entities.Models.DTOs.Generic
{
    public class TokenDataDTO
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
