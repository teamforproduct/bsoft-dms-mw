﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using BL.Model.Common;
using BL.Model.Enums;


namespace BL.Model.DocumentCore.IncomingModel
{
    public class ModifyTemplateDocument
    {
        public int Id { get; set; }
        public bool IsHard { get; set; }
        public EnumDocumentDirections DocumentDirection { get; set; }
        public int DocumentTypeId { get; set; }
        public int? DocumentSubjectId { get; set; }
        public string Description { get; set; }
        public int? RegistrationJournalId { get; set; }
        public int? SenderAgentId { get; set; }
        public int? SenderAgentPersonId { get; set; }
        public string Addressee { get; set; }
    }
}