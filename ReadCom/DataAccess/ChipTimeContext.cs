using Microsoft.EntityFrameworkCore;
using ReadCom.Models;

namespace ReadCom.DataAccess
{
    public class ChipTimeContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(
                @"server=localhost;database=readcom;user=readcom;password=readcom");
        }
        
        public DbSet<ChipTime> ChipTimes { get; set; }
    }
}