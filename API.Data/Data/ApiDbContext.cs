global using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Data.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {

        }
    }
}
