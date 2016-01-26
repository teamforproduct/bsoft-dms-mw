using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore 
{
    internal class BaseGetDocument : Command
    {
        private readonly IContext _context;
        private readonly int _documentId;

        public BaseGetDocument(IContext context,  int documentId)
        {
            _context = context;
            _documentId = documentId;
        }

        public override void Execute()
        {
            var documentDb = DmsResolver.Current.Get<IDocumnetsDbProcess>();
            documentDb.GetDocument(_context, _documentId);
        }

        public override void Execute(object parameter)
        {
            throw new System.NotImplementedException();
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}