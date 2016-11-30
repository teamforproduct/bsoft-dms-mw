﻿using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentPaperList
    {
        public int Id { get; set; }
		
        private DateTime  _Date; 
        public DateTime Date { get { return _Date; } set { _Date=value.ToUTC(); } }
		
        public string Description { get; set; }
    }
}
