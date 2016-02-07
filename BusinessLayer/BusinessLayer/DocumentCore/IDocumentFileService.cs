using BL.CrossCutting.Interfaces;
using BL.Model.DocumentAdditional;

namespace BL.Logic.DocumentCore
{
    public interface IDocumentFileService
    {
        void DeleteDocumentFile(IContext ctx, int documentId, int ordNumber);
        void DeleteDocumentFileVersion(IContext ctx, int documentId, int ordNumber, int versionId);
        byte[] GetUserFile(IContext ctx, DocumentAttachedFile attFile);
        int AddUserFile(IContext ctx, int documentId, string fileName, byte[] fileData, bool isAdditional);
        int AddNewVersion(IContext ctx, int documentId, int fileOrder, string fileName, byte[] fileData);
        bool UpdateCurrentFileVersion(IContext ctx, int documentId, int fileOrder, string fileName, byte[] fileData, int version = 0);
    }
}