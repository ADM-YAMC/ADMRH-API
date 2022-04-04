using System;
using System.Collections.Generic;

#nullable disable

namespace ADMRH_API.Models
{
    public partial class Archivo
    {
        public int IdArchivos { get; set; }
        public string FotoFrente { get; set; }
        public string FotoPerfil { get; set; }
        public string Cv { get; set; }
    }
}
