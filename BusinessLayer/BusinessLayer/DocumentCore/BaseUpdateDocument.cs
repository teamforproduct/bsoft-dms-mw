using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    internal class BaseUpdateDocument : Command
    {
        private readonly IContext _context;
        private readonly BaseDocument _document;

        public BaseUpdateDocument(IContext context, BaseDocument document)
        {
            _context = context;
            _document = document;
        }

        public override void Execute(object parameter)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute()
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            documentDb.UpdateDocument(_context, _document);
        }

        public override bool CanExecute()
        {
            return true;
        }

    }
}