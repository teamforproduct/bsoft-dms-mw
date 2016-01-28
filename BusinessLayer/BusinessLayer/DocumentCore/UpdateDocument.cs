using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    internal class UpdateDocument : Command
    {
        private readonly IContext _context;
        private readonly BaseDocument _document;

        public UpdateDocument(IContext context, BaseDocument document)
        {
            _context = context;
            _document = document;
        }

        public override object Execute(object parameter)
        {
            throw new System.NotImplementedException();
        }

        public override object Execute()
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            documentDb.UpdateDocument(_context, _document);
            return null;
        }

        public override bool CanExecute()
        {
            return true;
        }

    }
}