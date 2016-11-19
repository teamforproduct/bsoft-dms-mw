using BL.Model.Enums;

namespace BL.Model.SystemCore
{
    public class TempStoreObject:StoreInfo
    {
        public int OwnerId { get; set; }
        public int ObjectId { get; set; }
        public EnumObjects OwnerType { get; set; }
    }
}