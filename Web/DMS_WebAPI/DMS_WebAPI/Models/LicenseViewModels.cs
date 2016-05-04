using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS_WebAPI.Models
{
    public class ActivationProgramViewModel
    {
        [Display(Name = "Ваш код программы")]
        public string ProgramCode { get; set; }

        [Required(ErrorMessage = "Введите код активации!")]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Код активации")]
        public string ActivationCode { get; set; }
    }
}
