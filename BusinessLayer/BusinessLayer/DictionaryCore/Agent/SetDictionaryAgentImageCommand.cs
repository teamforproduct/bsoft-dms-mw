using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class SetDictionaryAgentImageCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryAgentImage Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentImage))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentImage)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType,false,true);

            return true;
        }

        public override object Execute()
        {
            try
            {
                byte[] buffer = new byte[Model.PostedFileData.ContentLength];
                Model.PostedFileData.InputStream.Read(buffer, 0, Model.PostedFileData.ContentLength);

                var newPers = new InternalDictionaryAgentImage
                {
                    Id = Model.AgentId,
                    Picture = buffer,
                };
                CommonDocumentUtilities.SetLastChange(_context, newPers);
                _dictDb.SetAgentImage(_context, newPers);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }
}
