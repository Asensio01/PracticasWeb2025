using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticaWeb_GAAJ.Modelos;

namespace PracticaWeb_GAAJ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BibliotecaController : ControllerBase
    {
        private readonly BibliotecaContext _BibliotecaContexto;

        public BibliotecaController(BibliotecaContext bibliotecaContext)
        {
            _BibliotecaContexto = bibliotecaContext;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Libros> listadoLibros = (from e in _BibliotecaContexto.Libros
                                          select e).ToList();
            if (listadoLibros.Count() == 0)
            {
                return NotFound();
            }

            return Ok(listadoLibros);
        }



        [HttpGet("{id}")]
        public IActionResult GetLibro(int id)
        {
            var libro = (from e in _BibliotecaContexto.Libros
                         join t in _BibliotecaContexto.Autor
                         on e.AutorId equals t.Id
                         where e.Id == id
                         select new
                         {
                             e.Id,
                             e.Titulo,
                             e.AnioPublicacion,
                             AutorNombre = t.Nombre,
                             e.CategoriaId,
                             e.Resumen
                         }).FirstOrDefault();
            if (libro == null)
            {
                return NotFound();

            }
            return Ok(libro);

        }

        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarLibro([FromBody] Libros libro )
        {
            try
            {
                _BibliotecaContexto.Libros.Add(libro);
                _BibliotecaContexto.SaveChanges();
                return Ok(libro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarEquipo(int id, [FromBody] Libros librosUpdate)
        {
            Libros? libroActual = (from e in _BibliotecaContexto.Libros
                                     where e.Id == id
                                     select e).FirstOrDefault();



            if (libroActual == null)
            {
                return NotFound();
            }

           libroActual.Titulo = librosUpdate.Titulo;
            libroActual.AnioPublicacion = librosUpdate.AnioPublicacion;
            libroActual.AutorId = librosUpdate.AutorId;
            libroActual.CategoriaId = librosUpdate.CategoriaId;
            libroActual.Resumen = librosUpdate.Resumen;

            _BibliotecaContexto.Entry(libroActual).State = EntityState.Modified;
            _BibliotecaContexto.SaveChanges();


            return Ok(librosUpdate);
        }


        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarEquipos(int id)
        {
            Libros? libros = (from e in _BibliotecaContexto.Libros
                               where e.Id == id
                               select e).FirstOrDefault();



            if (libros == null)
            {
                return NotFound();
            }



            _BibliotecaContexto.Libros.Attach(libros);
            _BibliotecaContexto.Libros.Remove(libros);
            _BibliotecaContexto.SaveChanges();



            return Ok(libros);
        }

        [HttpPost]
        public IActionResult GetLibrosbyYear (int anio) {
        var libros = _BibliotecaContexto.Libros

                .Where(e => e.AnioPublicacion > anio)
                .Select(e => new
                {
                    e.Id,
                    e.Titulo,
                    e.AnioPublicacion
                })
                .ToList();
            if (!libros.Any())
            {
                return NotFound("No hay libros publicados después de ese año.");
            }

            return Ok(libros);

        }

        [HttpGet("libros-por-autor/{nombreAutor}")]
        public IActionResult GetLibrosPorAutor(string nombreAutor)
        {
            var librosEscritos = (from l in _BibliotecaContexto.Libros
                                  join a in _BibliotecaContexto.Autor
                                  on l.AutorId equals a.Id
                                  where a.Nombre == nombreAutor
                                  group l by a.Nombre into grupo
                                  select new
                                  {
                                      Nombre = grupo.Key, 
                                      LibrosEscritos = grupo.Count() 
                                  }).FirstOrDefault();  

            if (librosEscritos is null)
            {
                return NotFound(new { mensaje = $"No se encontraron libros para el autor {nombreAutor}" });
            }

            return Ok(librosEscritos);
        }



        [HttpGet("buscar-titulo/{titulo}")]
        public IActionResult GetLibroPorTitulo(string titulo)
        {
            var libro =  (from e in _BibliotecaContexto.Libros
                                     join t in _BibliotecaContexto.Autor
                                     on e.AutorId equals t.Id
                                     where e.Titulo == titulo
                                     select new
                                     {
                                         e.Id,
                                         e.Titulo,
                                         e.AnioPublicacion,
                                         AutorNombre = t.Nombre,
                                         e.CategoriaId,
                                         e.Resumen
                                     }).FirstOrDefault();

            if (libro == null)
            {
                return NotFound($"No se encontró el libro con título: {titulo}");
            }

            return Ok(libro);
        }


    }
}
