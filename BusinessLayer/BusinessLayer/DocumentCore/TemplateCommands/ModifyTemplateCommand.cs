
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
        private readonly ITemplateDbProcess _operationDb;

        private InternalTemplate _templateDoc;

        public ModifyTemplateCommand(ITemplateDbProcess operationDb)
        {
            _operationDb = operationDb;
           
        }

        private ModifyTemplate Model
        {
            get
            {
                if (!(_param is ModifyTemplate))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplate)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType, false);

            //if (!_operationDb.CanModifyTemplate(_context, Model))
            //{
            //    throw new CouldNotModifyTemplate();
            //}

            return true;
        }

        public override object Execute()
        {

            var tModel = new InternalTemplate
            {
                DocumentTypeId = Model.DocumentTypeId,
                DocumentDirection = Model.DocumentDirection,
                DocumentSubject = Model.DocumentSubject,
            };

            CommonDocumentUtilities.SetLastChange(_context, tModel);

            var filterTemplate= CommonDocumentUtilities.GetFilterTemplateByTemplate(tModel).ToArray();

            var properties = new List<InternalPropertyValue>();

            if (Model.Properties != null)
            {
                properties = CommonDocumentUtilities.GetNewPropertyValues(Model.Properties).ToList();

                CommonDocumentUtilities.SetLastChange(_context, properties);

                var model = new InternalPropertyValues { Object = EnumObjects.Template, PropertyValues = properties };

                CommonSystemUtilities.VerifyPropertyValues(_context, model, filterTemplate);

                properties = model.PropertyValues.ToList();
            }
            else
            {
                var model = new InternalPropertyValues { Object = EnumObjects.Template, PropertyValues = new List<InternalPropertyValue>() };

                CommonSystemUtilities.VerifyPropertyValues(_context, model, filterTemplate);
            }
            _templateDoc = new InternalTemplate(Model) { Properties = properties };
            CommonDocumentUtilities.SetLastChange(_context, _templateDoc);
            return _operationDb.AddOrUpdateTemplate(_context, _templateDoc);
            
        }

       
    }
}