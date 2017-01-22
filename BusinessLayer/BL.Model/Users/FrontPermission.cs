using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Users
{
    public class FrontPermission
    {
        /// <summary>
        /// Модуль
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Фича
        /// </summary>
        public string Feature { get; set; }

        /// <summary>
        /// Доступ по CRUD
        /// </summary>
        public string AccessType { get; set; }

    }
}
