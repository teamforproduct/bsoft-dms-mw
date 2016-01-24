using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents
{
    public interface IDocumnetsDbProcess
    {
        void AddDocument(IContext ctx, BaseDocument document);
        void UpdateDocument(IContext ctx, BaseDocument document);
    }
}