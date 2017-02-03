using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Users
{
    public class ChangeNotifications
    {

        /// <summary>
        /// Рассылка на почту
        /// </summary>
        public bool IsSendEMail { get; set; }

        /// <summary>
        /// Адрес для нотификации
        /// </summary>
        public string EMailForNotifications { get; set; }
    }
}
