using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контрагент - сотрудник
    /// </summary>
    public class FrontDictionaryAgentUserPicture
    {

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Картинка
        /// </summary>
        public string FileContent { get; set; }

    }
}
