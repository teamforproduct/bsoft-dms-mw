using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class FrontAgentPerson : ModifyAgentPerson
    {

        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Паспортные данные
        /// </summary>
        public string Passport
        {
            get { string pass = PassportSerial?.Trim() + " " + PassportNumber + " " + PassportText?.Trim() + " " + PassportDate?.ToString("dd.MM.yyyy"); return pass.Trim(); }
        }

        /// <summary>
        /// Назване компании, контактным лицом которой является физическое лицо
        /// </summary>
        public string AgentCompanyName { get; set; }

    }
}
