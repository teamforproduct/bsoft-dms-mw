using BL.Model.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddTag 
    {
        [Required]
        public string Name { get; set; }
        public string Color { get; set; }
        [Required]
        public bool IsActive { get; set; }

    }
}
