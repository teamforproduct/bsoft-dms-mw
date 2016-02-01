using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumnetsDbProcess
    {
        void AddDocument(IContext ctx, FullDocument document);
        void UpdateDocument(IContext ctx, FullDocument document);
        IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters);
        FullDocument GetDocument(IContext ctx, int documentId);
        int AddRestrictedSendList(IContext ctx, ModifyDocumentRestrictedSendList restrictedSendList);
        void DeleteRestrictedSendList(IContext ctx, int restrictedSendListId);
    }
}