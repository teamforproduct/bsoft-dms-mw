using System;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Extensions;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontMainTag
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        public int? PositionId { get; set; }
        public string PositionName { get; set; }
        public bool IsSystem { get; set; }
        public string Color { get; set; }
        public int LastChangeUserId { get; set; }

        public DateTime LastChangeDate { get { return _LastChangeDate; } set { _LastChangeDate = value.ToUTC(); } }
        private DateTime _LastChangeDate;

        public string LastChangeUserName { get; set; }
        public int? DocCount { get; set; }

        /// <summary>
        /// Активный
        /// </summary>
        public bool IsActive { get; set; }

    }
}