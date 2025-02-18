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
        [HttpGet("Autor-con-más-Libros/{Autor}")]
        public IActionResult MasLibrosPublicados()
        {
            var libros = (from e in _BibliotecaContexto.Libros
                          join t in _BibliotecaContexto.Autor
                          on e.AutorId equals t.Id
                          group t by t.Nombre into grupo
                          orderby grupo.Count() descending
                          select new
                          {
                              Nombre = grupo.Key,
                              CantidadLibrosEscritos = grupo.Count()
                          });

            if (libros is null)
            {
                return NotFound(new { mensaje = "No se encontraron libros" });
            }

            return Ok(libros);
        }

        [HttpGet("Libros-más-recientes")]
        public IActionResult LibrosMasRecientes()
        {
            var libros = _BibliotecaContexto.Libros
                         .OrderByDescending(e => e.AnioPublicacion)
                         .Select(e => new
                         {
                             e.Titulo,
                             e.AnioPublicacion
                         })
                         .ToList();
            if (libros == null || !libros.Any())
            {
                return NotFound(new { mensaje = "No se encontraron libros" });
            }

            return Ok(libros);
        }


        [HttpGet("Libros-x-año")]
        public IActionResult LibrosTotalesPorAnio()
        {
            var libros = _BibliotecaContexto.Libros
                         .GroupBy(e => e.AnioPublicacion)
                         .OrderByDescending(g => g.Key)
                         .Select(g => new
                         {
                             AnioPublicacion = g.Key,
                             CantidadLibros = g.Count()
                         })
                         .ToList();

            if (libros == null || !libros.Any())
            {
                return NotFound(new { mensaje = "No se encontraron libros" });
            }

            return Ok(libros);
        }
        [HttpGet("VerificarAutor/{id}")]
        public IActionResult VerificarAutor(int id)
        {

            var autorConLibros = _BibliotecaContexto.Libros
                                   .Any(l => l.AutorId == id);

            if (!autorConLibros)
            {

                return NotFound(new { mensaje = "Este autor no tiene libros publicados." });
            }


            return Ok(new { mensaje = "El autor tiene libros publicados." });
        }



        [HttpGet("PrimerLibroPublicado")]
        public IActionResult FirtsBooktoAutor(int id)
        {
            var primerLibro = (from l in _BibliotecaContexto.Libros
                               join a in _BibliotecaContexto.Autor
                               on l.AutorId equals a.Id
                               where l.AutorId == id
                               && l.AnioPublicacion == (
                                   from l2 in _BibliotecaContexto.Libros
                                   where l2.AutorId == l.AutorId
                                   select l2.AnioPublicacion
                               ).Min()
                               select new
                               {
                                   a.Nombre,
                                   l.Titulo,
                                   l.AnioPublicacion
                               }).FirstOrDefault();

            if (primerLibro == null)
            {
                return NotFound(new { mensaje = "No se encontró el primer libro del autor o no tiene libros." });
            }

            return Ok(primerLibro);
        }

    }
}
