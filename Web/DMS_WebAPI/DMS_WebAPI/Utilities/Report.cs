using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using System.Reflection;
using BL.Model.Reports.Interfaces;
using DMS_WebAPI.Reports;

namespace DMS_WebAPI.Utilities
{
    public class Report
    {
        private Dictionary<EnumReportTypes, string> _crystalReportFilePaths { get; set; }
        private string _methodNameByClassListToDataTable => "ClassListToDataTable";
        private string _methodNameByClassDataToDataTable => "ClassDataToDataTable";
        public Report()
        {
            _crystalReportFilePaths = new Dictionary<EnumReportTypes, string>();
            _crystalReportFilePaths.Add(EnumReportTypes.RegistrationCardInternalDocument, "~/Reports/RegistrationCardInternalDocument.rpt");
        }

        public void SaveFromDataSetToFile(EnumReportTypes reportType, DataSet dataSet, ExportFormatType exportFormatType, string resultFilePath) 
        {
            var crystalReport = SetReport(reportType, dataSet);
            crystalReport.ExportToDisk(exportFormatType, HttpContext.Current.Server.MapPath(resultFilePath));
        }

        public void SaveToFile<T>(EnumReportTypes reportType, T data, ExportFormatType exportFormatType, string resultFilePath) where T : class
        {
            DataSet1 dataSet = new DataSet1();
            ClassDataToDataTable(data, dataSet);
            SaveFromDataSetToFile(reportType, dataSet, exportFormatType, resultFilePath);
        }
        public void SaveToFile<T>(EnumReportTypes reportType, List<T> classList, ExportFormatType exportFormatType, string resultFilePath) where T : class
        {
            DataSet1 dataSet = new DataSet1();
            ClassListToDataTable(classList, dataSet);
            SaveFromDataSetToFile(reportType, dataSet, exportFormatType, resultFilePath);
        }
        public Stream SaveFromDataSetToStream(EnumReportTypes reportType, DataSet dataSet, ExportFormatType exportFormatType)
        {
            var crystalReport = SetReport(reportType, dataSet);
            return crystalReport.ExportToStream(exportFormatType);
        }
        public Stream SaveToStream<T>(EnumReportTypes reportType, T data, ExportFormatType exportFormatType) where T : class
        {
            DataSet1 dataSet = new DataSet1();
            ClassDataToDataTable(data, dataSet);
            return SaveFromDataSetToStream(reportType, dataSet, exportFormatType);
        }
        public Stream SaveToStream<T>(EnumReportTypes reportType, List<T> classList, ExportFormatType exportFormatType) where T : class
        {
            DataSet1 dataSet = new DataSet1();
            ClassListToDataTable(classList, dataSet);
            return SaveFromDataSetToStream(reportType, dataSet, exportFormatType);
        }


        /// <summary>
        /// Создания объекта Crystal Report, загрузка шаблока, загрузка данных
        /// </summary>
        /// <param name="reportType">Тип шаблона отчета</param>
        /// <param name="dataSet"></param>
        /// <returns>Crystal Report</returns>
        private ReportDocument SetReport(EnumReportTypes reportType, DataSet dataSet)
        {
            ReportDocument crystalReport = new ReportDocument();
            crystalReport.Load(HttpContext.Current.Server.MapPath(_crystalReportFilePaths[reportType]));
            crystalReport.SetDataSource(dataSet);
            return crystalReport;
        }

        /// <summary>
        /// Возвращает DataTable по названию класса, если в DataSet не найдено такой таблици, тогда она создается
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSet"></param>
        /// <returns>DataTable</returns>
        private DataTable ClassToDataTable<T>(DataSet dataSet) where T : class
        {
            Type classType = typeof(T);

            DataTable dataTable = dataSet.Tables[classType.UnderlyingSystemType.Name];
            if (dataTable != null)
                return dataTable;

            List<PropertyInfo> propertyList = classType.GetProperties().ToList();
            if (propertyList.Count < 1)
            {
                return new DataTable();
            }

            string className = classType.UnderlyingSystemType.Name;
            DataTable result = new DataTable(className);

            foreach (PropertyInfo property in propertyList)
            {
                DataColumn col = new DataColumn();
                col.ColumnName = property.Name;

                Type dataType = property.PropertyType;

                if (IsNullable(dataType))
                {
                    if (dataType.IsGenericType)
                    {
                        dataType = dataType.GenericTypeArguments.FirstOrDefault();
                    }
                }
                else
                {   // True by default
                    col.AllowDBNull = false;
                }

                col.DataType = dataType;

                if (dataType.IsValueType)
                {
                    result.Columns.Add(col);
                }
            }

            dataSet.Tables.Add(result);

            return result;
        }

        /// <summary>
        /// Заполняет данными запись в таблице из связей в экземпляре класса
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="dataSet"></param>
        private void ClassReferenceToDataTable<T>(T data, DataSet dataSet) where T : class
        {
            Type classType = typeof(T);

            List<PropertyInfo> propertyList = classType.GetProperties().ToList();
            if (propertyList.Count < 1)
            {
                return;
            }

            foreach (PropertyInfo property in propertyList)
            {
                Type dataType = property.PropertyType;

                if (IsNullable(dataType))
                {
                    if (dataType.IsGenericType)
                    {
                        dataType = dataType.GenericTypeArguments.FirstOrDefault();
                        if (dataType.GetInterfaces().Any(x => x == typeof(IReports)))
                        {
                            MethodInfo method = typeof(Report).GetMethod(_methodNameByClassListToDataTable);
                            MethodInfo generic = method.MakeGenericMethod(dataType);
                            generic.Invoke(this, new object[] { property.GetValue(data), dataSet, true });
                        }
                    }
                    else
                    {
                        if (dataType.GetInterfaces().Any(x => x == typeof(IReports)))
                        {
                            MethodInfo method = typeof(Report).GetMethod(_methodNameByClassDataToDataTable);
                            MethodInfo generic = method.MakeGenericMethod(dataType);
                            generic.Invoke(this, new object[] { property.GetValue(data), dataSet, true }); ;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Заполняет данными запись в таблице из экземпляра класса
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="dataSet"></param>
        /// <param name="isAddReference"></param>
        public void ClassDataToDataTable<T>(T data, DataSet dataSet, bool isAddReference = true) where T : class
        {
            if (data == null)
            {
                return;
            }

            DataTable result = ClassToDataTable<T>(dataSet);

            if (result.Columns.Count < 1)
            {
                return;
            }

            bool isAdd = ClassToDataRow(ref result, data);

            if (isAdd && isAddReference)
                ClassReferenceToDataTable(data, dataSet);
        }

        /// <summary>
        /// Заполняет данными записи в таблице из перечня экземпляров класса
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classList"></param>
        /// <param name="dataSet"></param>
        /// <param name="isAddReference"></param>
        public void ClassListToDataTable<T>(List<T> classList, DataSet dataSet, bool isAddReference = true) where T : class
        {
            if (classList == null)
            {
                return;
            }

            DataTable result = ClassToDataTable<T>(dataSet);

            if (result.Columns.Count < 1)
            {
                return;
            }
            if (classList.Count < 1)
            {
                return;
            }

            foreach (T item in classList)
            {
                var isAdd = ClassToDataRow(ref result, item);

                if (isAdd && isAddReference)
                    ClassReferenceToDataTable(item, dataSet);
            }
        }
        /// <summary>
        /// Заполняет данными запись в таблице из экземпляра класса
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="data"></param>
        private bool ClassToDataRow<T>(ref DataTable table, T data) where T : class
        {
            if (data == null)
                return false;

            Type classType = typeof(T);
            string className = classType.UnderlyingSystemType.Name;

            // Checks that the table name matches the name of the class. 
            // There is not required, and it may be desirable to disable this check.
            // Comment this out or add a boolean to the parameters to disable this check.
            if (!table.TableName.Equals(className))
            {
                return false;
            }

            DataRow row = table.NewRow();
            List<PropertyInfo> propertyList = classType.GetProperties().ToList();

            foreach (PropertyInfo prop in propertyList)
            {
                if (table.Columns.Contains(prop.Name))
                {
                    if (table.Columns[prop.Name] != null)
                    {
                        var val = prop.GetValue(data, null);
                        if (val != null)
                        {
                            row[prop.Name] = prop.GetValue(data, null);
                        }
                        else
                        {
                            row[prop.Name] = DBNull.Value;
                        }

                    }
                }
            }

            if (row["Id"] != null && table.Rows.Find(row["Id"]) == null)
            {
                table.Rows.Add(row);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsNullable(Type input)
        {
            if (!input.IsValueType) return true; // Is a ref-type, such as a class
            if (Nullable.GetUnderlyingType(input) != null) return true; // Nullable
            return false; // Must be a value-type
        }
    }
}