using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddJournalsInOrg
    {
        public string IncomingJournalIndex { get; set; }
        public string IncomingJournalName { get; set; }

        public string OutcomingJournalIndex { get; set; }
        public string OutcomingJournalName { get; set; }

        public string InternalJournalIndex { get; set; }
        public string InternalJournalName { get; set; }
        

    }
}
