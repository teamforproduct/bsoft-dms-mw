using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore.AgentEmployee
{
    public class ModifyDictionaryAgentEmployeeCommand : BaseDictionaryAgentEmployeeCommand
    {
        private ModifyAgentEmployee Model { get { return GetModel<ModifyAgentEmployee>(); } }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {

                if (Model.PostedFileData != null)
                {
                    var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
                    var fileModel = new ModifyDictionaryAgentImage
                    {
                        Id = Model.Id,
                        PostedFileData = Model.PostedFileData
                    };
                    tmpDict.ExecuteAction(EnumDictionaryActions.SetAgentImage, _context, fileModel);
                }

                var item = new InternalDictionaryAgentEmployee(Model);

                CommonDocumentUtilities.SetLastChange(_context, item);

                _dictDb.UpdateAgentEmployee(_context, item);
                //_dictDb.SetAgentUserLanguage(_context, new InternalDictionaryAgentUser(item));
                transaction.Complete();
            }
            return null;
        }
    }
}
