using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Exception;
using BL.Model.SystemCore;

    namespace BL.Logic.DictionaryCore.Agent
{
    public class DeleteDictionaryAgentCommand : BaseDictionaryCommand
    {
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
                _admin.VerifyAccess(_context, CommandType,false,true);

                FrontDictionaryAgent tmp = _dictDb.GetDictionaryAgent(_context, Model);

            // Удалить можно только контрагента без роли. 
            if (tmp != null)
            {
                if (tmp.IsBank || tmp.IsCompany || tmp.IsEmployee || tmp.IsIndividual)
                {
                    throw new DictionaryRecordCouldNotBeDeleted();
                }
            }   
                return true;
            }

            public override object Execute()
            {
                try
                {
                    var newPers = new InternalDictionaryAgent
                    {
                        Id = Model

                    };
                    _dictDb.DeleteDictionaryAgent(_context, newPers);
                    return null;
                }
                catch (Exception ex)
                {
                    throw new DictionaryRecordCouldNotBeDeleted(ex);
                }
            }
        }
}
