using BL.Database.DBModel.Template;

namespace BL.Database.DBModel.InternalModel
{
    public class DocumentQuery
    {
        public Document.Documents Doc { get; set; }
        public TemplateDocuments Templ { get; set; }
        public string DirName { get; set; }
        public string SubjName { get; set; }
        public string DocTypeName { get; set; }
        public string RegistrationJournalName { get; set; }
        public string RegistrationJournalNumerationPrefixFormula { get; set; }
        public string RegistrationJournalPrefixFormula { get; set; }
        public string RegistrationJournalSuffixFormula { get; set; }

        public string ExecutorPosName { get; set; }
        public string ExecutorPositionExecutorAgentName { get; set; }
        public string ExecutorPositionExecutorNowAgentName { get; set; }
        //public string ExecutorAgentName { get; set; }
        public string SenderAgentname { get; set; }
        public string SenderPersonName { get; set; }
    }
}