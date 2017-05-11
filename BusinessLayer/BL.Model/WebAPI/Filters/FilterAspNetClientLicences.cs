using System.Collections.Generic;

namespace BL.Model.WebAPI.Filters
{
    public class FilterAspNetClientLicences
    {
        public List<int> ClientLicenceIds { get; set; }
        public List<int> ClientIds { get; set; }
        /// <summary>
        /// Получить лицензии которые используют сейчас
        /// null - не применять
        /// true - лицензии которые используют сейчас
        /// false - лицензии которые не используют сейчас
        /// </summary>
        public bool? IsNowUsed { get; set; }
    }
}
