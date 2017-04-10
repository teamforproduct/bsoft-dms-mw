using System;
using BL.Model.Extensions;

namespace BL.Model.DictionaryCore.FrontModel.Employees
{
    /// <summary>
    /// сотрудник - пользователь
    /// </summary>
    public class FrontAgentEmployeeUser: FrontAgentEmployee
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

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
