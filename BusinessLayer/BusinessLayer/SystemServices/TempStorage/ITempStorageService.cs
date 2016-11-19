using BL.Model.Enums;

namespace BL.Logic.SystemServices.TempStorage
{
    public interface ITempStorageService
    {
        void AddToStore(EnumObjects ownerType, int ownerId, int objectId, object storeobject);
        object GetStoreObject(EnumObjects ownerType, int ownerId, int objectId);
        object ExtractStoreObject(EnumObjects ownerType, int ownerId, int objectId);
    }
}