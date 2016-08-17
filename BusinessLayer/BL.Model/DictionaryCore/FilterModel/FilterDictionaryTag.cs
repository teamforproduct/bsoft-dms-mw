using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryTag : DictionaryBaseFilterParameters
    {
        public string NameExact { get; set; }
        public bool WithDocCount { get; set; }

    }
}
