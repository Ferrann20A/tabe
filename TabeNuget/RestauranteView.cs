﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TabeNuget
{
    [Table("V_RESTAURANTES")]
    public class RestauranteView
    {
        [Key]
        [Column("IDRESTAURANTE")]
        public int IdRestaurante { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("CORREO")]
        public string Correo { get; set; }
        [Column("DIRECCION")]
        public string Direccion { get; set; }
        [Column("TELEFONO")]
        public string Telefono { get; set; }
        [Column("IMAGEN")]
        public string Imagen { get; set; }
        [Column("NOMBRECATEGORIA")]
        public string CategoriaRestaurante { get; set; }
        [Column("VALORACION")]
        public decimal Valoracion { get; set; }

        [NotMapped]
        public DistanceMatrixInfo InfoEntrega { get; set; }
    }
}
