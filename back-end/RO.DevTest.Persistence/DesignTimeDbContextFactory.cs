using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RO.DevTest.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DefaultContext>
    {
        public DefaultContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=rodevtest;User Id=postgres;Password=root;");

            return new DefaultContext(optionsBuilder.Options);
        }
    }
}
