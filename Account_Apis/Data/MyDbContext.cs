using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Account_Apis.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Account_Apis.Data
{
    public class MyDbContext : IdentityDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }
        // Add DbSet for User
        public DbSet<User> Users { get; set; }
    }
}