using CRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD.DAL
{
    public class OsobaContext(DbContextOptions<OsobaContext> options) : DbContext(options)
    {
        public DbSet<Osoba> Osoby {  get; set; }
    }
}
