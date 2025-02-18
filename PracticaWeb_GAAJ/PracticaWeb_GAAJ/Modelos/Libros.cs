using System.ComponentModel.DataAnnotations;

namespace PracticaWeb_GAAJ.Modelos
{
    public class Libros
    {
        [Key]
        public int?  Id { set; get; }

        public string? Titulo { set; get; }

        public int? AnioPublicacion { set; get; }

        public int? AutorId { set; get;}
        public int? CategoriaId { set; get;}

        public string? Resumen { set; get;}



    }
}
