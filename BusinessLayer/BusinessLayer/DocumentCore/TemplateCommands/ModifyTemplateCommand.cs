﻿
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
    public class ModifyTemplateCommand: BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;

        private InternalTemplateDocument _templateDoc;

        public ModifyTemplateCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;
           
        }

        private ModifyTemplateDocument Model
        {
            get
            {
                if (!(_param is ModifyTemplateDocument))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateDocument)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            //if (!_operationDb.CanModifyTemplate(_context, Model))
            //{
            //    throw new CouldNotModifyTemplateDocument();
            //}

            return true;
        }

        public override object Execute()
        {

            var tModel = new InternalTemplateDocument
            {
                DocumentTypeId = Model.DocumentTypeId,
                DocumentDirection = Model.DocumentDirection,
                DocumentSubjectId = Model.DocumentSubjectId,
            };

            CommonDocumentUtilities.SetLastChange(_context, tModel);

            var filterTemplate= CommonDocumentUtilities.GetFilterTemplateByTemplateDocument(tModel).ToArray();

            var properties = new List<InternalPropertyValue>();

            if (Model.Properties != null)
            {
                properties = CommonDocumentUtilities.GetNewPropertyValues(Model.Properties).ToList();

                CommonDocumentUtilities.SetLastChange(_context, properties);

                var model = new InternalPropertyValues { Object = EnumObjects.TemplateDocument, PropertyValues = properties };

                CommonSystemUtilities.VerifyPropertyValues(_context, model, filterTemplate);

                properties = model.PropertyValues.ToList();
            }
            else
            {
                var model = new InternalPropertyValues { Object = EnumObjects.TemplateDocument, PropertyValues = new List<InternalPropertyValue>() };

                CommonSystemUtilities.VerifyPropertyValues(_context, model, filterTemplate);
            }
            _templateDoc = new InternalTemplateDocument(Model) { Properties = properties };
            CommonDocumentUtilities.SetLastChange(_context, _templateDoc);
            return _operationDb.AddOrUpdateTemplate(_context, _templateDoc);
            
        }

       
    }
}