﻿using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryDocumentSubjectCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryDocumentSubject Model { get { return GetModel<ModifyDictionaryDocumentSubject>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            //DictionaryModelVerifying.VerifyDocumentSubject(_context, _dictDb, Model);

            return true;
        }

        public override object Execute()
        {
            var dds = CommonDictionaryUtilities.DocumentSubjectModifyToInternal(_context, Model);

            _dictDb.UpdateDocumentSubject(_context, dds);
            return null;
        }
    }
}