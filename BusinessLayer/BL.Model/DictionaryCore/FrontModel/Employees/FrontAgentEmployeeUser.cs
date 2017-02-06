using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// сотрудник - пользователь
    /// </summary>
    public class FrontAgentEmployeeUser: FrontAgentEmployee
    {
        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Дата последнего успешного логина
        /// </summary>
        public DateTime? LastSuccessLogin { get { return _LastSuccessLogin; } set { _LastSuccessLogin = value.ToUTC(); } }
        private DateTime? _LastSuccessLogin;
        /// <summary>
        /// Дата последней неуспешной попытки входа
        /// </summary>
        public DateTime? LastErrorLogin { get { return _LastErrorLogin; } set { _LastErrorLogin = value.ToUTC(); } }
        private DateTime? _LastErrorLogin;
        /// <summary>
        /// Количество неуспешных попыток входа
        /// </summary>
        public int? CountErrorLogin { get; set; }

    } 
}
