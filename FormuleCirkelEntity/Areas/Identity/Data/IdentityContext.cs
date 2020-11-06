using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FormuleCirkelEntity.Data
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
            builder.Entity<SimUser>()
                .Property(s => s.Drivers)
                .HasConversion(
                    list => JsonConvert.SerializeObject(list, Formatting.None),
                    json => JsonConvert.DeserializeObject<List<int>>(json) ?? new List<int>());
            builder.Entity<SimUser>()
                .Property(s => s.Teams)
                .HasConversion(
                    list => JsonConvert.SerializeObject(list, Formatting.None),
                    json => JsonConvert.DeserializeObject<List<int>>(json) ?? new List<int>());
        }
    }
}
