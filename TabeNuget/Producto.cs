﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TabeNuget
{
    [Table("PRODUCTOS")]
    public class Producto
    {
        [Key]
        [Column("IDPRODUCTO")]
        public int IdProducto { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }
        [Column("PRECIO")]
        public decimal Precio { get; set; }
        [Column("IMAGEN")]
        public string Imagen { get; set; }
        [Column("IDRESTAURANTE")]
        public int IdRestaurante { get; set; }
    }
}
