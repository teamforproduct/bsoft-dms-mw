using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using System;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore.Commands
{
    internal class UpdateDocument : Command
    {
        private readonly IContext _context;
        private readonly InternalDocument _document;

        public UpdateDocument(IContext context, InternalDocument document)
        {
            _context = context;
            _document = document;
        }

        public override object Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public override object Execute()
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.UpdateDocument(_context, _document);
            return null;
        }

        public override bool CanExecute()
        {
            return true;
        }

    }
}