using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentAdditional;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    public interface IDocumentFileService
    {
        IEnumerable<DocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId);
        IEnumerable<DocumentAttachedFile> GetDocumentFileVersions(IContext ctx, int documentId, int orderNumber);
        DocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber);
        DocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id);
        DocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, int documentId, int orderNumber);


        void DeleteDocumentFile(IContext ctx, int documentId, int ordNumber);
        void DeleteDocumentFileVersion(IContext ctx, int documentId, int ordNumber, int versionId);
        byte[] GetUserFile(IContext ctx, DocumentAttachedFile attFile);
        int AddUserFile(IContext ctx, int documentId, string fileName, byte[] fileData, bool isAdditional);
        int AddUserFile(IContext ctx, ModifyDocumentFile model);
        int AddNewVersion(IContext ctx, int documentId, int fileOrder, string fileName, byte[] fileData);
        bool UpdateCurrentFileVersion(IContext ctx, int documentId, int fileOrder, string fileName, byte[] fileData, int version = 0);
    }
}