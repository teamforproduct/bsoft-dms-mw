using System;
using System.Transactions;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.Common;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DictionaryCore.AgentEmployee
{
    public class ModifyDictionaryAgentEmployeeCommand : BaseDictionaryAgentEmployeeCommand
    {

        private ModifyDictionaryAgentEmployee Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentEmployee))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentEmployee)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                using (var transaction = Transactions.GetTransaction())
                {

                    if (Model.PostedFileData != null)
                    {
                        var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
                        var fileModel = new ModifyDictionaryAgentImage
                        {
                            AgentId = Model.Id,
                            PostedFileData = Model.PostedFileData
                        };
                        tmpDict.ExecuteAction(EnumDictionaryActions.SetAgentImage, _context, fileModel);
                    }

                    var item = new InternalDictionaryAgentEmployee(Model);

                    CommonDocumentUtilities.SetLastChange(_context, item);

                    _dictDb.UpdateAgentEmployee(_context, item);
                    _dictDb.SetAgentUserLanguage(_context, new InternalDictionaryAgentUser (item));
                    transaction.Complete();
                }
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
