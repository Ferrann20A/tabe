﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoASPNET;
using ProyectoASPNET.Models;
using TabeAPI.Models;

namespace TabeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantesController : ControllerBase
    {
        private RepositoryRestaurantes repo;

        public RestaurantesController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        // GET: api/Restaurantes
        /// <summary>
        /// Devuelve todos los restaurantes
        /// </summary>
        /// <remarks>
        /// Permite obtener todos los restaurantes de la BBDD
        /// </remarks>
        /// <response code="200">Devuelve el conjunto de restaurantes</response>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Restaurante>>> GetRestaurantes()
        {
            return await this.repo.GetRestaurantesAsync();
        }

        // GET: api/Restaurantes/{id}
        /// <summary>
        /// Busca un restaurante
        /// </summary>
        /// <remarks>
        /// Permite obtener un restaurante según su ID
        /// </remarks>
        /// <param name="id">ID del restaurante</param>
        /// <response code="200">Devuelve el restaurante</response>
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<ActionResult<Restaurante>> FindRestaurante(int id)
        {
            return await this.repo.FindRestauranteAsync(id);
        }

        // GET: api/Restaurantes/GetRestauranteFromLoggedUser
        /// <summary>
        /// Busca el restaurante del usuario logeado (debe ser de tipo Restaurante)
        /// </summary>
        /// <response code="200">Devuelve el restaurante</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Restaurante</response>
        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult<Restaurante>> GetRestauranteFromLoggedUser()
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario == 3) return await this.repo.GetRestauranteFromLoggedUserAsync(usuario.IdUsuario);
            return Unauthorized();
        }

        // POST: api/Restaurantes
        /// <summary>
        /// Crea un nuevo restaurante
        /// </summary>
        /// <param name="model">Datos del nuevo restaurante + nueva contraseña</param>
        /// <response code="200">Devuelve el nuevo restaurante</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Admin</response>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Restaurante>> CreateRestaurante(RestauranteAPIModel model)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario == 2) return await this.repo.CreateRestauranteAsync(model.Restaurante, model.Password);
            return Unauthorized();
        }

        // PUT: api/Restaurantes
        /// <summary>
        /// Modifica un restaurante
        /// </summary>
        /// <param name="restaurante">Datos nuevos del restaurante</param>
        /// <response code="200">Devuelve el restaurante modificado</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Admin</response>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<Restaurante>> EditRestaurante(Restaurante restaurante)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario != 1) return await this.repo.EditRestauranteAsync(restaurante);
            return Unauthorized();
        }

        // DELETE: api/Restaurantes/{id}
        /// <summary>
        /// Elimina un restaurante
        /// </summary>
        /// <param name="id">ID del restaurante</param>
        /// <response code="200">Restaurante eliminado con éxito</response>
        /// <response code="401">No autorizado. El usuario no es de tipo Admin</response>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteRestaurante(int id)
        {
            string jsonUsuario = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            if (usuario.TipoUsuario == 2)
            {
                if (await this.repo.FindRestauranteAsync(id) == null) return NotFound();
                else
                {
                    await this.repo.DeleteRestauranteAsync(id);
                    return Ok();
                }
            }
            return Unauthorized();
        }
    }
}
