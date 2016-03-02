﻿using System;
using BL.Model.Common;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentSavedFilter : LastChangeInfo
    {
        public int Id { get; set; }
        public Nullable<int> PositionId { get; set; }
        public string Icon { get; set; }
        public string Filter { get; set; }
        public bool IsCommon { get; set; }
    }
}