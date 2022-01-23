using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Entities.Models.DTOs.Requests
{
    public class UserLoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
