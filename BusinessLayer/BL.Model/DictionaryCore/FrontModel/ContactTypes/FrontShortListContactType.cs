using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    ///
    /// </summary>
    public class FrontShortListContactType : ListItem
    {
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Key - можно использовать для подстановки картинок или вызова сторонних сервисов
        /// </summary>
        public string SpecCode { get; set; }

    }
}