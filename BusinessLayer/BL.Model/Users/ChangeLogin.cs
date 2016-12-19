using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Users
{
    public class ChangeLogin
    {
        /// <summary>
        /// Новый логин
        /// </summary>
        public string NewEmail { get; set; }

        
    }
}
