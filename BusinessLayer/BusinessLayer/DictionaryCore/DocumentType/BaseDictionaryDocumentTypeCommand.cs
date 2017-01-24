using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryDocumentTypeCommand : BaseDictionaryCommand
    {
        private AddDocumentType Model { get { return GetModel<AddDocumentType>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Name?.Trim();

            var filter = new FilterDictionaryDocumentType
            {
                NameExact = Model.Name,
            };

            if (TypeModelIs<ModifyDocumentType>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyDocumentType>().Id }; }

            var spr = _dictDb.GetInternalDictionaryDocumentTypes(_context, filter).FirstOrDefault();

            if (spr != null) throw new DictionaryDocumentTypeNameNotUnique(Model.Name);

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}