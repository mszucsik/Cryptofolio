using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Cryptofolio.Models;

namespace Cryptofolio.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Cryptofolio.Models.Portfolio> Portfolio { get; set; }
        public DbSet<Cryptofolio.Models.Holding> Holding { get; set; }
        public DbSet<Cryptofolio.Models.Asset> Asset { get; set; }
        public DbSet<Cryptofolio.Models.MarketPrice> MarketPrice { get; set; }
        public DbSet<Cryptofolio.Models.Comment> Comment { get; set; }
    }
}
