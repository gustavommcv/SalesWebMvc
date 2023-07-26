using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesWebMcv.Models;

namespace SalesWebMcv.Data
{
    public class SalesWebMcvContext : DbContext
    {
        public SalesWebMcvContext (DbContextOptions<SalesWebMcvContext> options)
            : base(options)
        {
        }

        public DbSet<SalesWebMcv.Models.Department> Department { get; set; } = default!;
    }
}
