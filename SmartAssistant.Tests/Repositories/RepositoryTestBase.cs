using Microsoft.EntityFrameworkCore;
using SmartAssistant.WebApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Repositories
{
    public class RepositoryTestBase
    {
        protected DbContextOptions<ApplicationDbContext> DbContextOptions { get; }

        public RepositoryTestBase()
        {
            DbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Use a unique DB for each test
                .Options;
        }

        protected ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(DbContextOptions);
        }
    }
}
