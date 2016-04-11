using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using System.Data;
using System;
using CrystalDecisions.CrystalReports.Engine;
using DMS_WebAPI.Reports;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BL.Model.Enums;
using CrystalDecisions.Shared;

namespace DMS_WebAPI.Controllers.Reports
{
    [Authorize]
    public class ReportsController : ApiController
    {
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var report = DmsResolver.Current.Get<Report>();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var doc = docProc.GetDocumentByReport(cxt, id);

            report.SaveToFile(EnumReportTypes.RegistrationCardInternalDocument, doc, ExportFormatType.PortableDocFormat, "~/Reports/rep.pdf");
            
            return new JsonResult(null, this);
        }
    }
}
