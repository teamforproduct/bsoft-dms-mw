using System;
using System.Transactions;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.Common;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentCommand : BaseDictionaryAgentCommand
    {
        public override object Execute()
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,new TransactionOptions {IsolationLevel = IsolationLevel.ReadUncommitted}))
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

                    var newAgent = new InternalDictionaryAgent(Model);
                    CommonDocumentUtilities.SetLastChange(_context, newAgent);
                    _dictDb.UpdateAgent(_context, newAgent);

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
