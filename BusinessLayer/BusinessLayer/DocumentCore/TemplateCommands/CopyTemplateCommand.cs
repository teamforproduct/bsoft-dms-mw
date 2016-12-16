
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore.InternalModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class CopyTemplateCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private InternalTemplateDocument _templateDoc;

        public CopyTemplateCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            _templateDoc = _operationDb.CopyTemplatePrepare(_context, Model);
            if (_templateDoc == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }

        public override object Execute()
        {
            //CommonDocumentUtilities.SetLastChange(_context, Model);

            var filterTemplate = CommonDocumentUtilities.GetFilterTemplateByTemplateDocument(new InternalTemplateDocument
            {
                DocumentTypeId = _templateDoc.DocumentTypeId,
                DocumentDirection = _templateDoc.DocumentDirection,
                DocumentSubjectId = _templateDoc.DocumentSubjectId,
            }).ToArray();

            var properties = new List<InternalPropertyValue>();

            //if (_templateDoc.Properties != null)
            //{
            //    properties = CommonDocumentUtilities.GetNewPropertyValues(_templateDoc.Properties).ToList();

            //    CommonDocumentUtilities.SetLastChange(_context, properties);

            //    var model = new InternalPropertyValues { Object = EnumObjects.TemplateDocument, PropertyValues = properties };

            //    CommonSystemUtilities.VerifyPropertyValues(_context, model, filterTemplate);

            //    properties = model.PropertyValues.ToList();
            //}
            //else
            //{
            //    var model = new InternalPropertyValues { Object = EnumObjects.TemplateDocument, PropertyValues = new List<InternalPropertyValue>() };

            //    CommonSystemUtilities.VerifyPropertyValues(_context, model, filterTemplate);
            //}

            //return _operationDb.AddOrUpdateTemplate(_context, new InternalTemplateDocument(Model), properties);
            return null;

        }


    }
}