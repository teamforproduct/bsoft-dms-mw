using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    public interface IDocumentService
    {
        void SaveDocument (IContext context, BaseDocument document);
        IEnumerable<BaseDocument> GetDocuments();
    }
}