﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TabeNuget
{
    [Table("USUARIOS")]
    public class Usuario
    {
        [Key]
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("CORREO")]
        public string Correo { get; set; }
        [Column("CONTRASENYA")]
        public byte[] Contrasenya { get; set; }
        [Column("SALT")]
        public string Salt { get; set; }
        [Column("TELEFONO")]
        public string Telefono { get; set; }
        [Column("DIRECCION")]
        public string Direccion { get; set; }
        [Column("TIPOUSUARIO")]
        public int TipoUsuario { get; set; }
    }
}
