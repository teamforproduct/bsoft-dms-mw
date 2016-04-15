﻿using System.Collections.Generic;

namespace BL.Model.AdminCore.FilterModel
{
    public class FilterAdminLanguageValue
    {
        public List<int> LanguageValueId { get; set; }
        public int? LanguageId { get; set; }
        public string LanguageName { get; set; }
        public string Label { get; set; }
        public List<string> Labels { get; set; }
    }
}
