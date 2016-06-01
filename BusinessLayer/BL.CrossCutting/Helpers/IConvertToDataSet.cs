﻿using BL.Model.Reports.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BL.CrossCutting.Helpers
{
    public interface IConvertToDataSet
    {
        /// <summary>
        /// Заполняет данными запись в таблице из экземпляра класса
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="dataSet"></param>
        /// <param name="isAddReference"></param>
        void ClassDataToDataTable<T>(T data, DataSet dataSet, bool isAddReference = true) where T : class;

        /// <summary>
        /// Заполняет данными записи в таблице из перечня экземпляров класса
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classList"></param>
        /// <param name="dataSet"></param>
        /// <param name="isAddReference"></param>
        void ClassListToDataTable<T>(List<T> classList, DataSet dataSet, bool isAddReference = true) where T : class;
    }
}