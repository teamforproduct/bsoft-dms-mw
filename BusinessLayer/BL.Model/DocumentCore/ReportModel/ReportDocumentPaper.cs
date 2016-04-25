using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.Reports.Interfaces;

namespace BL.Model.DocumentCore.ReportModel
{
    public class ReportDocumentPaper : IReports
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
