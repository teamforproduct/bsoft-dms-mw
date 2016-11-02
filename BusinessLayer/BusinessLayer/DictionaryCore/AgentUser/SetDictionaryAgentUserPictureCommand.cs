using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class SetDictionaryAgentUserPictureCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryAgentUserPicture Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentUserPicture))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentUserPicture)_param;
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

                var newPers = new InternalDictionaryAgentUserPicture
                {
                    Id = Model.Id,
                    Picture = buffer,
                };
                CommonDocumentUtilities.SetLastChange(_context, newPers);
                _dictDb.SetAgentUserPicture(_context, newPers);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }
}
