using BL.Model.Extensions;
using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontRegistrationFullNumber
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        public int? DocumentId { get; set; }
        public string RegistrationFullNumber { get; set; }
        public int? RegistrationNumber { get; set; }
        public string RegistrationNumberSuffix { get; set; }
        public string RegistrationNumberPrefix { get; set; }
		
        public DateTime? DocumentDate { get { return _DocumentDate; } set { _DocumentDate=value.ToUTC(); } }
        private DateTime?  _DocumentDate; 
    }
}
