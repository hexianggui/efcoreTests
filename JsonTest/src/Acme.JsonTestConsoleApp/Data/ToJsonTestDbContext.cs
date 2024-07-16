using Acme.JsonTestConsoleApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Acme.JsonTestConsoleApp.Data
{
    public class ToJsonTestDbContext : AbpDbContext<ToJsonTestDbContext>
    {
        public ToJsonTestDbContext(DbContextOptions<ToJsonTestDbContext> options)
            : base(options)
        {
        }
        public DbSet<ToJsonTestEntity> ToJsonTestEntitys { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ToJsonTestEntity>(b =>
            {
                b.ToTable("ToJsonTestEntity");

                b.OwnsMany(x => x.JsonItems, o =>
                {
                    o.ToJson();
                });
                b.ConfigureByConvention();
            });
             
            
           

        }
    }
}
