using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.Database.IncomingModel;
using BL.Model.Database.FrontModel;
using BL.Logic.DocumentCore.Interfaces;
using System.Data;
using System;
using CrystalDecisions.CrystalReports.Engine;
using DMS_WebAPI.Reports;
using System.Collections.Generic;
using System.Reflection;

namespace DMS_WebAPI.Controllers.Reports
{
    [Authorize]
    public class ReportsController : ApiController
    {
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var doc = docProc.GetDocumentByReport(cxt, id);

            ///////////////////
            DataSet1 ds = new DataSet1();
            ds.Tables.Add(ToDataTable<BL.Model.DocumentCore.ReportModel.ReportDocument>("Documents", new List<BL.Model.DocumentCore.ReportModel.ReportDocument> { doc }));
            //DataTable table = new DataTable("Documents");
            //table.Columns.Add("Id", Type.GetType("System.Int32"));
            //table.Columns.Add("Description", Type.GetType("System.String"));

            //DataRow row;

            //row = table.NewRow();
            //row["Id"] = doc.Id;
            //row["Description"] = doc.Description;
            //table.Rows.Add(row);

            ReportDocument crystalReport = new ReportDocument();
            crystalReport.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/CrystalReport1.rpt"));
            crystalReport.SetDataSource(ds);

            crystalReport.SaveAs("rep.pdf");
            ///////////////////

            return new JsonResult(null, this);
        }

        public static DataTable ToDataTable<T>(string dataTableName, IEnumerable<T> collection)
        {
            DataTable dt = new DataTable(dataTableName);
            Type t = typeof(T);
            PropertyInfo[] pia = t.GetProperties();

            //Inspect the properties and create the columns in the DataTable
            foreach (PropertyInfo pi in pia)
            {
                Type ColumnType = pi.PropertyType;
                if ((ColumnType.IsGenericType))
                {
                    ColumnType = ColumnType.GetGenericArguments()[0];
                }
                dt.Columns.Add(pi.Name, ColumnType);
            }

            //Populate the data table
            foreach (T item in collection)
            {
                DataRow dr = dt.NewRow();
                dr.BeginEdit();
                foreach (PropertyInfo pi in pia)
                {
                    if (pi.GetValue(item, null) != null)
                    {
                        dr[pi.Name] = pi.GetValue(item, null);
                    }
                }
                dr.EndEdit();
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
