﻿using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class AddTemplateTask
    {
        [Required]
        public int DocumentId { get; set; }
        [Required]
        public string Task { get; set; }
        public string Description { get; set; }
    }
}