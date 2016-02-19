﻿using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.SystemCore
{
    public class InternalSystemAction
    {
        public EnumActions Action { get; set; }
        public EnumObjects Object { get; set; }
        public string ActionCode { get; set; }
        public string ObjectCode { get; set; }
        public string API { get; set; }
        public string Description { get; set; }
        public IEnumerable<InternalActionRecord> ActionRecords { get; set; }
        
    }
}
