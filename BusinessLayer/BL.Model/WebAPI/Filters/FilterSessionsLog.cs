using BL.Model.Enums;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.Filters
{
    public class FilterSessionsLog
    {
        [IgnoreDataMember]
        public List<int> IDs { get; set; }

        [IgnoreDataMember]
        public string Session { get; set; }

        public string Message { get; set; }

        [IgnoreDataMember]
        public string UserId { get; set; }

        public string UserFullName { get; set; }

        [IgnoreDataMember]
        public string IPExact { get; set; }

        [IgnoreDataMember]
        public List<EnumLogTypes> Types { get; set; }

        /// <summary>
        /// Дата записи лог с
        /// </summary>
        public DateTime? DateFrom { get { return _DateFrom; } set { _DateFrom = value.ToUTC(); } }
        private DateTime? _DateFrom;

        /// <summary>
        /// Дата записи лог по
        /// </summary>
        public DateTime? DateTo { get { return _DateTo; } set { _DateTo = value.ToUTC(); } }
        private DateTime? _DateTo;

        public bool? IsActive { get; set; }

        public bool? IsEnabled { get; set; }
        /// <summary>
        /// Поисковая фраза для полнотекстового поиска
        /// </summary>
        [IgnoreDataMember]
        public string FullTextSearchString { get; set; }
    }
}
