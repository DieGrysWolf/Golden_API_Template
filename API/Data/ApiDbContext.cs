global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Data
{
    public class ApiDbContext : IdentityDbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {

        }
    }
}
