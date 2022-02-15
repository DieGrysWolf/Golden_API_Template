using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataServer.Data
{
    public class UserModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid IdentityId { get; set; }
        public bool Alive { get; set; } = true; // this is to shadow delete
        public DateTime DateJoined { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
