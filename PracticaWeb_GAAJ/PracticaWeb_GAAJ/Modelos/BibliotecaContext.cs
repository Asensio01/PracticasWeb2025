using Microsoft.EntityFrameworkCore;

namespace PracticaWeb_GAAJ.Modelos
{
    public class BibliotecaContext : DbContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options) : base(options) {

        }
        public DbSet<Libros> Libros { get; set; } = null!;

        public DbSet<Autor> Autor { get; set; } 


    }
}
