using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Exception;
using BL.Model.SystemCore;

    namespace BL.Logic.DictionaryCore
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
                _adminService.VerifyAccess(_context, CommandType,false,true);

//                FrontDictionaryAgent tmp = _dictDb.GetAgent(_context, Model);

            // Удалить можно только контрагента без роли. 
//            if (tmp != null)
//            {
//                if (tmp.IsBank || tmp.IsCompany || tmp.IsEmployee || tmp.IsIndividual)
//                {
//                    throw new DictionaryRecordCouldNotBeDeleted();
//                }
//            }   
                return true;
            }

            public override object Execute()
            {
                try
                {
                    _dictDb.DeleteAgent(_context, Model);
                    return null;
                }
                catch (Exception ex)
                {
                    throw new DictionaryRecordCouldNotBeDeleted(ex);
                }
            }
        }
}
