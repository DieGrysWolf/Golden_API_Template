global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Entities.Models.DTOs;
using API.DataServer.Data;

namespace API.DataServer
{ 
    public class ApiDbContext : IdentityDbContext
    {
        public virtual DbSet<RefreshToken>? RefreshTokens { get; set; }
        public virtual DbSet<UserModel>? UsersInfo { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {

        }
    }
}