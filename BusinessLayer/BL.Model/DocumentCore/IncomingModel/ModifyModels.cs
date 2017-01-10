﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.SystemCore.IncomingModel;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class ModifyTemplateDocument : AddTemplateDocument
    {
        /// <summary>
        /// ИД шаблона
        /// </summary>
        [Required]
        public int Id { get; set; }
    }

    public class ModifyTemplateDocumentTask : AddTemplateDocumentTask
    {
        /// <summary>
        /// ИД задачи
        /// </summary>
        [Required]
        public int Id { get; set; }
    }

    public class ModifyTemplateDocumentPaper : AddTemplateDocumentPaper
    {
        /// <summary>
        /// ИД бн
        /// </summary>
        [Required]
        public int Id { get; set; }
    }

    /// <summary>
    /// Модель для добавления/изменения записи плана работы для шаблона
    /// </summary>
    public class ModifyTemplateDocumentSendLists : AddTemplateDocumentSendLists
    {
        /// <summary>
        /// ИД бн
        /// </summary>
        [Required]
        public int Id { get; set; }
    }

    public class ModifyTemplateDocumentRestrictedSendLists : AddTemplateDocumentRestrictedSendLists
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int Id { get; set; }
    }

    /// <summary>
    /// Изменение файла, прикрепленного к шаблону документа
    /// </summary>
    public class ModifyTemplateAttachedFile : AddTemplateAttachedFile
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public int Id { get; set; }
    }

}