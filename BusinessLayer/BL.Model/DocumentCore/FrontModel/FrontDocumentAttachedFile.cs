using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentAttachedFile: InternalDocumentAttachedFile
    {
        public string LastChangeUserName { get; set; }
    }
}