using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticaWeb_GAAJ.Modelos;

namespace PracticaWeb_GAAJ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {

        private readonly BibliotecaContext _BibliotecaContexto;

        public AutoresController(BibliotecaContext bibliotecaContext)
        {
            _BibliotecaContexto = bibliotecaContext;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Autor> listadoAutores = (from e in _BibliotecaContexto.Autor
                                          select e).ToList();
            if (listadoAutores.Count() == 0)
            {
                return NotFound();
            }

            return Ok(listadoAutores
                );
        }


        [HttpGet("{id}")]
        public IActionResult GetAutor(int id)
        {
            var autor = (from e in _BibliotecaContexto.Autor
                         join t in _BibliotecaContexto.Libros
                         on e.Id equals t.AutorId
                         where e.Id == id
                         select new
                         {
                             e.Id,
                             e.Nombre,
                             e.Nacionalidad,
                             TituloLibro = t.Titulo,
                         }).FirstOrDefault();
            if (autor == null)
            {
                return NotFound();

            }
            return Ok(autor);

        }


        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarAutor([FromBody] Autor autor)
        {
            try
            {
                _BibliotecaContexto.Autor.Add(autor);
                _BibliotecaContexto.SaveChanges();
                return Ok(autor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarAutor(int id, [FromBody] Autor AutorUpdate)
        {
            Autor? AutorActual = (from e in _BibliotecaContexto.Autor
                                   where e.Id == id
                                   select e).FirstOrDefault();



            if (AutorActual == null)
            {
                return NotFound();
            }

            AutorActual.Nombre = AutorUpdate.Nombre;
            AutorActual.Nacionalidad = AutorUpdate.Nacionalidad;

            _BibliotecaContexto.Entry( AutorActual).State = EntityState.Modified;
            _BibliotecaContexto.SaveChanges();


            return Ok(AutorUpdate);
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarAutor(int id)
        {
            Autor? autor = (from e in _BibliotecaContexto.Autor
                              where e.Id == id
                              select e).FirstOrDefault();



            if ( autor == null)
            {
                return NotFound();
            }



            _BibliotecaContexto.Autor.Attach(autor);
            _BibliotecaContexto.Autor.Remove(autor);
            _BibliotecaContexto.SaveChanges();



            return Ok(autor);
        }

   

    }
}
