using BL.Model.Reports.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BL.CrossCutting.Helpers
{
    public class ConvertToDataSet: IConvertToDataSet
    {
        private const string _methodNameByClassListToDataTable = "ClassListToDataTable";
        private const string _methodNameByClassDataToDataTable = "ClassDataToDataTable";

        /// <summary>
        /// Возвращает DataTable по названию класса, если в DataSet не найдено такой таблици, тогда возвращает new DataTable() 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSet"></param>
        /// <returns>DataTable</returns>
        private DataTable ClassToDataTable<T>(DataSet dataSet) where T : class
        {
            if (dataSet == null)
                return new DataTable();

            Type classType = typeof(T);

            DataTable dataTable = dataSet.Tables[classType.UnderlyingSystemType.Name];
            if (dataTable != null)
                return dataTable;
            else
                return new DataTable();

        }

        /// <summary>
        /// Заполняет данными запись в таблице из связей в экземпляре класса
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="dataSet"></param>
        private void ClassReferenceToDataTable<T>(T data, DataSet dataSet) where T : class
        {
            if (data == null || dataSet == null)
                return;

            Type classType = typeof(T);

            List<PropertyInfo> propertyList = classType.GetProperties().ToList();
            if (propertyList.Count < 1)
            {
                return;
            }

            foreach (PropertyInfo property in propertyList)
            {
                Type dataType = property.PropertyType;

                    if (dataType.IsGenericType)
                    {
                        dataType = dataType.GenericTypeArguments.FirstOrDefault();
                        if (dataType.GetInterfaces().Any(x => x == typeof(IReports)))
                        {
                            MethodInfo method = typeof(ConvertToDataSet).GetMethod(_methodNameByClassListToDataTable);
                            MethodInfo generic = method.MakeGenericMethod(dataType);
                            generic.Invoke(null, new object[] { property.GetValue(data), dataSet, true });
                        }
                    }else if (dataType.GetInterfaces().Any(x => x == typeof(IReports)))
                    {
                        MethodInfo method = typeof(ConvertToDataSet).GetMethod(_methodNameByClassDataToDataTable);
                        MethodInfo generic = method.MakeGenericMethod(dataType);
                        generic.Invoke(null, new object[] { property.GetValue(data), dataSet, true });
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
            if (data == null || dataSet == null)
                return;

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
            if (classList == null || dataSet == null)
                return;

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

            string key = string.Empty;
            try
            {
                key = table.PrimaryKey.First().ColumnName;
                if (row[key] != null && table.Rows.Find(row[key]) == null)
                {
                    table.Rows.Add(row);
                    return true;
                }
            }
            catch
            {

            }
            if (string.IsNullOrEmpty(key))
            {
                table.Rows.Add(row);
                return true;
            }
            return false;
        }
    }
}
