using BL.Model.Enums;
using System.Collections.Generic;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalSystemActionCategoryForDocument
    {
        public EnumActionCategories? Category { get; set; }
        public string CategoryName { get; set; }
        public List<InternalSystemActionForDocument> Actions { get; set; }

    }
}
