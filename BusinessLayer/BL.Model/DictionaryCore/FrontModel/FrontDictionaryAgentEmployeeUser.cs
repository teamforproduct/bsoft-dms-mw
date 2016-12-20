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
    public class FrontDictionaryAgentEmployeeUser: FrontDictionaryAgentEmployee
    {
        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }
    } 
}
