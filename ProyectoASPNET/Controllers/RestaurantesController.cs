﻿using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Filters;
using ProyectoASPNET.Helpers;
using TabeNuget;
using ProyectoASPNET.Services;
using System.Security.Claims;

namespace ProyectoASPNET.Controllers
{
    public class RestaurantesController : Controller
    {
        private IServiceRestaurantes service;
        private ServiceStorageBlobs serviceBlobs;
        private ServiceCacheRedis serviceRedis;
        private HelperGoogleApiDirections helperDistanceMatrix;
        private string UrlBlobProductos;
        private string UrlBlobRestaurantes;
        private string UrlBlobCategorias;

        public RestaurantesController
            (IServiceRestaurantes service,
            ServiceStorageBlobs serviceBlobs,
            ServiceCacheRedis serviceRedis,
            HelperGoogleApiDirections helperDistanceMatrix,
            IConfiguration configuration)
        {
            this.service = service;
            this.serviceBlobs = serviceBlobs;
            this.serviceRedis = serviceRedis;
            this.helperDistanceMatrix = helperDistanceMatrix;
            UrlBlobProductos = configuration.GetValue<string>("BlobUrls:UrlProductos");
            UrlBlobRestaurantes = configuration.GetValue<string>("BlobUrls:UrlRestaurantes");
            UrlBlobCategorias = configuration.GetValue<string>("BlobUrls:UrlCategorias");
        }

        [AuthorizeUser]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            List<CategoriaRestaurante> categorias = await this.service.GetCategoriasRestaurantesAsync();
            categorias.ForEach(c => c.IconoCategoria = UrlBlobCategorias + c.IconoCategoria);
            ViewData["CATEGORIAS"] = categorias;
            return View();
        }

        [AuthorizeUser]
        public async Task<IActionResult> _ListRestaurantes
            (string? categoria, string searchquery = "", string orden = "valoracion", int posicion = 1)
        {
            List<RestauranteView> restaurantes = new List<RestauranteView>();
            if (categoria != null)
                restaurantes = await this.service.FilterRestaurantesViewAsync(categoria, searchquery);
            else
                restaurantes = await this.service.GetRestaurantesViewAsync(searchquery);
            ViewData["POSICION"] = posicion;
            ViewData["NUMREGISTROS"] = restaurantes.Count();
            int idusuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Usuario usu = await this.service.FindUsuarioAsync(idusuario);
            string direccionUsu = usu.Direccion;
            var tasks = restaurantes
                .Select(async r => r.InfoEntrega = await this.helperDistanceMatrix
                    .GetDistanceMatrixInfoAsync(r.Direccion, direccionUsu));
            await Task.WhenAll(tasks);
            if (orden != "valoracion")
                restaurantes = restaurantes.OrderBy(r => r.InfoEntrega.TiempoEstimado).ToList();
            restaurantes = restaurantes.Skip(8 * (posicion - 1)).Take(8).ToList();
            restaurantes.ForEach(r => r.Imagen = UrlBlobRestaurantes + r.Imagen);
            return PartialView("_ListRestaurantes", restaurantes);
        }

        [AuthorizeUser]
        public async Task<IActionResult> Productos(int idrestaurante)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            ProductosActionModel model = new ProductosActionModel
            {
                Restaurante = await this.service.FindRestauranteViewAsync(idrestaurante),
                CategoriasProductos = await this.service.GetCategoriasProductosAsync(idrestaurante)
            };
            model.Restaurante.Imagen = UrlBlobRestaurantes + model.Restaurante.Imagen;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public async Task<IActionResult> Productos
            (string form, int idrestaurante, int cantidad, int idproducto)
        {
            if (form == "cesta")
            {
                int restauranteSession = HttpContext.Session.GetObject<int>("RESTAURANTE");
                // Si id (idrestaurante) no es igual al idrestuarante en el Session, nos salta error
                // Solo podemos tener productos en nuestra cesta de un solo restaurante
                if (restauranteSession == 0
                    || (restauranteSession != 0 && restauranteSession == idrestaurante))
                {
                    await serviceRedis.UpdateCesta(new ProductoCesta
                    {
                        IdProducto = idproducto,
                        Cantidad = cantidad
                    });
                }
                else ViewData["ERROR"] = true;
            }
            ProductosActionModel model = new ProductosActionModel
            {
                Restaurante = await this.service.FindRestauranteViewAsync(idrestaurante),
                CategoriasProductos = await this.service.GetCategoriasProductosAsync(idrestaurante)
            };
            model.Restaurante.Imagen = UrlBlobRestaurantes + model.Restaurante.Imagen;
            return View(model);
        }

        [AuthorizeUser]
        public async Task<IActionResult> _ListProductos(int idrestaurante, int categoria)
        {
            List<Producto> productos;
            if (categoria == 0)
                productos = await this.service.GetProductosRestauranteAsync(idrestaurante);
            else
                productos = await this.service.GetProductosByCategoriaAsync(idrestaurante, categoria);
            productos.ForEach(p => p.Imagen = UrlBlobProductos + p.Imagen);
            return PartialView("_ListProductos", productos);
        }

        [AuthorizeUser]
        public async Task<IActionResult> UpdateValoracionRestaurante
            (int idrestaurante, int idusuario, int valoracion)
        {
            if (HttpContext.User.FindFirst(ClaimTypes.Role).Value != "1")
            {
                return RedirectToAction("CheckRoutes", "Auth");
            }
            await this.service.UpdateValoracionRestauranteAsync(
                new ValoracionRestaurante
                {
                    IdRestaurante = idrestaurante,
                    IdUsuario = idusuario,
                    Valoracion = valoracion
                });
            return RedirectToAction("Productos", new { idrestaurante = idrestaurante });
        }
    }
}
