﻿using System.ComponentModel.DataAnnotations;

namespace Proyect.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es requerido.")]
        [Display (Name ="Correo Electronico")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "La Contraseña es requerida")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }
    }

}