using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using System;

namespace BL.Logic.DocumentCore 
{
    internal class AddDocument : Command
    {
        private readonly IContext _context;
        private readonly FullDocument _document;

        public AddDocument(IContext context,  FullDocument document)
        {
            _context = context;
            _document = document;
        }

        public override object Execute()
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.AddDocument(_context, _document);
            return null;
        }

        public override object Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}