using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore.AgentPerson
{
    public class ModifyDictionaryAgentPersonCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryAgentPerson Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentPerson))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentPerson)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
             _admin.VerifyAccess(_context, CommandType,false,true);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newPerson = new InternalDictionaryAgentPerson
                {
                    Id = Model.Id,
                    FirstName = Model.FirstName,
                    LastName = Model.LastName,
                    MiddleName = Model.MiddleName,
                    TaxCode=Model.TaxCode,
                    IsMale = Model.IsMale,
                    PassportSerial=Model.PassportSerial,
                    PassportNumber=Model.PassportNumber,
                    PassportText=Model.PassportText,
                    PassportDate=Model.PassportDate,
                    IsActive = Model.IsActive,
                    BirthDate=Model.BirthDate,
                    Description=Model.Description
                };
                CommonDocumentUtilities.SetLastChange(_context, newPerson);
                _dictDb.UpdateDictionaryAgentPerson(_context, newPerson);
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}
