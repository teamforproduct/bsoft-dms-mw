using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;

namespace DMS_WebAPI.Utilities
{
    public class Report
    {
        private Dictionary<EnumReportTypes, string> _CrystalReportFilePaths { get; set; }
        public Report()
        {
            _CrystalReportFilePaths = new Dictionary<EnumReportTypes, string>();
            _CrystalReportFilePaths.Add(EnumReportTypes.RegistrationCardInternalDocument, "~/Reports/RegistrationCardInternalDocument.rpt");
        }
        private ReportDocument SetReport(EnumReportTypes reportType, DataSet dataSet)
        {
            ReportDocument crystalReport = new ReportDocument();
            crystalReport.Load(HttpContext.Current.Server.MapPath(_CrystalReportFilePaths[reportType]));
            crystalReport.SetDataSource(dataSet);
            return crystalReport;
        }
        public void SaveToFile(EnumReportTypes reportType, DataSet dataSet, ExportFormatType exportFormatType, string resultFilePath)
        {
            var crystalReport = SetReport(reportType, dataSet);
            crystalReport.ExportToDisk(exportFormatType, HttpContext.Current.Server.MapPath(resultFilePath));
        }
        public Stream SaveToStream(EnumReportTypes reportType, DataSet dataSet, ExportFormatType exportFormatType)
        {
            var crystalReport = SetReport(reportType, dataSet);
            return crystalReport.ExportToStream(exportFormatType);
        }
        public void ToDataSet<T>(List<T> iList, DataSet dataSet)
        {
            string tableName = typeof(T).Name;
            DataTable dataTable = dataSet.Tables[tableName];
            if (dataTable != null)
                ToDataTable<T>(iList, dataTable);
        }

        public void ToDataTable<T>(List<T> iList, DataTable dataTable)
        {
            PropertyDescriptorCollection propertyDescriptorCollection =
                TypeDescriptor.GetProperties(typeof(T));

            var props = propertyDescriptorCollection.Cast<PropertyDescriptor>();
            var cols = dataTable.Columns.Cast<DataColumn>().Where(x => props.Any(y => y.Name.Equals(x.ColumnName)));

            foreach (T iListItem in iList)
            {
                var row = dataTable.NewRow();
                foreach (DataColumn col in cols)
                {
                    var prop = propertyDescriptorCollection[col.ColumnName];
                    if (prop != null)
                    {
                        object val = prop.GetValue(iListItem);
                        if (val != null)
                            row[col.ColumnName] = val;
                        else
                        {
                            row[col.ColumnName] = DBNull.Value;
                        }
                    }
                    else
                        row[col.ColumnName] = DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }
        }
    }
}