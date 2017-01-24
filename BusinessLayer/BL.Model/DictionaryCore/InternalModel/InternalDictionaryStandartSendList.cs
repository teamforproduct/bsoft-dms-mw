using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryStandartSendList : LastChangeInfo
    {

        public InternalDictionaryStandartSendList()
        {
        }

        public InternalDictionaryStandartSendList(ModifyStandartSendList model)
        {
            Id = model.Id;
            Name = model.Name;
            PositionId = model.PositionId;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? PositionId { get; set; }

    }
}
