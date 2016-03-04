using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Enums
{
    public static class AccordanceDictionaries
    {
        public static Dictionary<EnumDocumentActions,List<EnumEventTypes>> PermissibleEventTypesForAction = 
            new Dictionary<EnumDocumentActions, List<EnumEventTypes>>
            {
                { EnumDocumentActions.ControlChange, new List<EnumEventTypes> { EnumEventTypes.ControlOn, EnumEventTypes.ControlChange } },
                { EnumDocumentActions.ControlOff, new List<EnumEventTypes> { EnumEventTypes.ControlOn, EnumEventTypes.ControlChange } },
                { EnumDocumentActions.RejectSigning, new List<EnumEventTypes> { EnumEventTypes.SendForSigning} },
                { EnumDocumentActions.RejectАpproval, new List<EnumEventTypes> { EnumEventTypes.SendForАpproval} },
            };

    }
}
