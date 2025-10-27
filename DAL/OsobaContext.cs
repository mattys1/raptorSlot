using raptorSlot.Models;
using Microsoft.EntityFrameworkCore;

namespace raptorSlot.DAL
{
    public class OsobaContext(DbContextOptions<OsobaContext> options) : DbContext(options)
    {
        public DbSet<Osoba> Osoby {  get; set; }
    }
}
