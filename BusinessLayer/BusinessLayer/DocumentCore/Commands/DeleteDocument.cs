using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.Exception;
using System;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.DocumentCore.Commands
{
    internal class DeleteDocument : Command
    {
        private readonly IContext _context;
        private readonly InternalDocument _document;

        public DeleteDocument(IContext context, InternalDocument document)
        {
            _context = context;
            _document = document;
        }

        public override object Execute()
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.DeleteDocument(_context, _document.Id);
            return null;
        }

        public override object Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public override bool CanExecute()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            if (_document.IsRegistered)
            {
                throw new DocumentCannotBeModifiedOrDeleted();
            }
            return true;
        }
    }
}