using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontFilterDocumentAttachedFile: InternalFilterDocumentAttachedFile
    {
        public string LastChangeUserName { get; set; }
    }
}