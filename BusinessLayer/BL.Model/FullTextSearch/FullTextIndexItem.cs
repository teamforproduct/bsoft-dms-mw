﻿using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.FullTextSearch
{
    /// <summary>
    /// Model that describe one elements in FullText index and operation with it.
    /// </summary>
    public class FullTextIndexItem : ICloneable
    {
        
        /// <summary>
        /// ID record in DB table
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ID of the document part, which is indexed.
        /// </summary>
        public int ObjectId { get; set; }
        /// <summary>
        /// Which part of document should be updated
        /// </summary>
        public EnumObjects ObjectType { get; set; }
        /// <summary>
        /// ID of document
        /// </summary>
        public int ParentObjectId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EnumObjects ParentObjectType { get; set; }
        /// <summary>
        /// Id for filtering
        /// </summary>
        public int FilterId { get; set; }
        /// <summary>
        /// type of operation add/delete/update
        /// </summary>
        public EnumOperationType OperationType { get; set; }

        /// <summary>
        /// Text which should be added to full text search
        /// </summary>
        public string ObjectText { get; set; }
        /// <summary>
        /// Additional filters
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Date which should be added to full text search, data should convert in to string correctly
        /// </summary>
        public List<DateTime?> ObjectTextAddDateTime { get; set; }

        /// <summary>
        /// ID of the client
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Module ID 
        /// </summary>
        public int ModuleId { get; set; }
        /// <summary>
        /// Feature ID 
        /// </summary>
        public int FeatureId { get; set; }

        /// <summary>
        /// Доступы
        /// </summary>
        public List<FullTextIndexItemAccessInfo> Access { get; set; }
        /// <summary>
        /// Актуальность записи с
        /// </summary>
        public DateTime? DateFrom { get; set; }
        /// <summary>
        /// Актуальность записи по
        /// </summary>
        public DateTime? DateTo { get; set; }
        /// <summary>
        /// Клонирует объект
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}