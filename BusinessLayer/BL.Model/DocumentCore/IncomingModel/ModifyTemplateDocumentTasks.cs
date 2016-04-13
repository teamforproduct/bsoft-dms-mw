﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class ModifyTemplateDocumentTasks
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Nullable<int> PositionId { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
    }
}