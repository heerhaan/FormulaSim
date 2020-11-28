using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FormuleCirkelEntity.DAL
{
    public class IdentityContext : IdentityDbContext<SimUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
