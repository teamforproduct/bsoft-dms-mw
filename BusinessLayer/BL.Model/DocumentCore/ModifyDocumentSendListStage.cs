﻿using System;
using BL.Model.Enums;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore
{
    public class ModifyDocumentSendListStage
    {
        public int DocumentId { get; set; }
        public int Stage { get; set; }
    }
}
