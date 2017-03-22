using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontDictionaryStandartSendList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? PositionId { get; set; }
        //public int LastChangeUserId { get; set; }
        //public DateTime LastChangeDate { get { return _LastChangeDate; } set { _LastChangeDate=value.ToUTC(); } }
        //private DateTime  _LastChangeDate; 

        public string PositionName { get; set; }
        public string PositionExecutorName { get; set; }

        public string PositionExecutorTypeSuffix { get; set; }

        /// <summary>
        /// Отдел
        /// </summary>
        [IgnoreDataMember]
        public string DepartmentName { get; set; }

        /// <summary>
        /// Отдел. Код
        /// </summary>
        [IgnoreDataMember]
        public string DepartmentIndex { get; set; }

        //public virtual IEnumerable<FrontDictionaryStandartSendListContent> StandartSendListContents { get; set; }
    }
}
