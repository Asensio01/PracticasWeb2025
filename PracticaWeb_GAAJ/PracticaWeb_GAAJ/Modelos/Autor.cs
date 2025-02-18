using System.ComponentModel.DataAnnotations;

namespace PracticaWeb_GAAJ.Modelos
{
    public class Autor
    {
        [Key]
        public int Id { set; get; }

        public string Nombre { set; get; }

        public string Nacionalidad { set; get; }
    }
}
