using BL.Database.DBModel.Template;
using BL.Database.DBModel.Document;

namespace BL.Database.DBModel.InternalModel
{
    public class DocumentQuery
    {
        public Document.Documents Doc { get; set; }
        public DocumentAccesses Acc { get; set; }
        public TemplateDocuments Templ { get; set; }
        public string DirName { get; set; }
        public string AccLevName { get; set; }
        public string SubjName { get; set; }
        public string DocTypeName { get; set; }
        public string RegJurnalName { get; set; }
        public string ExecutorPosName { get; set; }
        public string ExecutorAgentName { get; set; }
        public string SenderAgentname { get; set; }
        public string SenderPersonName { get; set; }
    }
}