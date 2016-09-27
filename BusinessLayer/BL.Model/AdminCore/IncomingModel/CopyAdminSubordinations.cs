using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using BL.Model.Enums;
//using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// Дублирование настройки рассылки для должностей
    /// </summary>
    public class CopyAdminSubordinations
    {

        /// <summary>
        /// Должность, с которой скопировать настройку 
        /// </summary>
        //[Required]
        public int SourcePositionId { get; set; }

        /// <summary>
        /// Должность, которой применить настройку 
        /// </summary>
        //[Required]
        public int TargetPositionId { get; set; }

        /// <summary>
        /// Режим копирования
        /// </summary>
        //[Required]
        public EnumCopyMode CopyMode { get; set; }

    }
}