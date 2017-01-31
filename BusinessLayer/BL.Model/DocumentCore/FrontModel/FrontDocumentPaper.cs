using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentPaper : FrontDocumentInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public bool IsOriginal { get; set; }
        public bool IsCopy { get; set; }
        public int PageQuantity { get; set; }
        public int OrderNumber { get; set; }
        public int? LastPaperEventId { get; set; }
        public bool IsInWork { get; set; }
        public string OwnerAgentName { get; set; }
        public string OwnerPositionExecutorAgentName { get; set; }
        public string OwnerPositionName { get; set; }
        public DateTime? PaperPlanDate { get { return _PaperPlanDate; } set { _PaperPlanDate = value.ToUTC(); } }
        private DateTime? _PaperPlanDate;

        public DateTime? PaperSendDate { get { return _PaperSendDate; } set { _PaperSendDate = value.ToUTC(); } }
        private DateTime? _PaperSendDate;

        public DateTime? PaperRecieveDate { get { return _PaperRecieveDate; } set { _PaperRecieveDate = value.ToUTC(); } }
        private DateTime? _PaperRecieveDate;

    }
}
