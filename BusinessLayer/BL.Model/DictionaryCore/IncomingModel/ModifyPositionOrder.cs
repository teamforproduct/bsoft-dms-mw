using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class ModifyPositionOrder
    {
        /// <summary>
        /// Должность
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int Order { get; set; }

    }
}