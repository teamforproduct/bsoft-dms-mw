using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Enums
{
    /// <summary>
    /// Список типов контрагентов
    /// </summary>
    public enum EnumDictionaryAgentTypes
    {
        /// <summary>
        /// Юридическое лицо
        /// </summary>
        isCompany,
        /// <summary>
        /// Физичиеское лицо
        /// </summary>
        isIndividual,
        /// <summary>
        /// Сотрудник
        /// </summary>
        isEmployee,
        /// <summary>
        /// Банк
        /// </summary>
        isBank
    }
}