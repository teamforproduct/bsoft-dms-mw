using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		
        private DateTime?  _DocumentDate; 
        public DateTime? DocumentDate { get { return _DocumentDate; } set { _DocumentDate=value.ToUTC(); } }
    }
}
