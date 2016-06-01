﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.SystemCore
{
    public class ModifyLicenceInfo
    {
        [Required]
        public int ClientId { get; set; }
        public string Name { get; set; }
        public DateTime FirstStart { get; set; }
        /// <summary>
        /// Только для информации
        /// </summary>
        public bool IsTrial { get; set; }
        public int? NamedNumberOfConnections { get; set; }
        public int? ConcurenteNumberOfConnections { get; set; }
        public int? DurationDay { get; set; }
        public string Functionals { get; set; }
        public string LicenceKey { get; set; }
    }
}