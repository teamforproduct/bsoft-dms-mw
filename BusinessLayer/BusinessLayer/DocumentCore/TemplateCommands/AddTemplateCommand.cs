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
    public class AddTemplateCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;


        public AddTemplateCommand(ITemplateDocumentsDbProcess operationDb)
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


            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, Model);

            var filterTemplate = CommonDocumentUtilities.GetFilterTemplateByTemplateDocument(new InternalTemplateDocument
            {
                DocumentTypeId = Model.DocumentTypeId,
                DocumentDirection = Model.DocumentDirection,
                DocumentSubjectId = Model.DocumentSubjectId,
            }).ToArray();

            var properties = new List<InternalPropertyValue>();

            if (Model.Properties != null)
            {
                properties = CommonDocumentUtilities.GetNewPropertyValues(Model.Properties).ToList();

                CommonDocumentUtilities.SetLastChange(_context, properties);

                var model = new InternalPropertyValues { Object = EnumObjects.TemplateDocuments, PropertyValues = properties };

                CommonSystemUtilities.VerifyPropertyValues(_context, model, filterTemplate);

                properties = model.PropertyValues.ToList();
            }
            else
            {
                var model = new InternalPropertyValues { Object = EnumObjects.TemplateDocuments, PropertyValues = new List<InternalPropertyValue>() };

                CommonSystemUtilities.VerifyPropertyValues(_context, model, filterTemplate);
            }

            return _operationDb.AddOrUpdateTemplate(_context, Model, properties);

        }


    }
}