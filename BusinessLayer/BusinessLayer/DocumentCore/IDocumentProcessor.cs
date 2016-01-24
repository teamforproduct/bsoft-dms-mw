using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    public interface IDocumentProcessor
    {
        void SaveDocument (IContext context, BaseDocument document);
    }
}