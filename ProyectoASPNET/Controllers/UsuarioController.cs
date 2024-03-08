﻿using Microsoft.AspNetCore.Mvc;
using ProyectoASPNET.Extensions;
using ProyectoASPNET.Models;

namespace ProyectoASPNET.Controllers
{
    public class UsuarioController : Controller
    {
        private RepositoryRestaurantes repo;

        public UsuarioController(RepositoryRestaurantes repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Perfil()
        {
            if (HttpContext.Session.GetString("USER") != null)
            {
                int idusuario = HttpContext.Session.GetObject<int>("USER");
                Usuario usu = await this.repo.FindUsuarioAsync(idusuario);
                return View(usu);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(Usuario usuario)
        {
            await this.repo.EditUsuarioAsync(usuario);
            return RedirectToAction("Perfil");
        }

        public async Task<IActionResult> Pedidos()
        {
            if (HttpContext.Session.GetString("USER") != null)
            {
                int idusuario = HttpContext.Session.GetObject<int>("USER");
                List<Pedido> pedidos = await this.repo.GetPedidosUsuarioAsync(idusuario);
                List<int> idPedidos = pedidos.Select(p => p.IdPedido).ToList();
                ViewData["PRODUCTOS"] = await this.repo.GetProductosPedidoViewAsync(idPedidos);
                return View(pedidos);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }
    }
}