using BL.Model.Enums;

namespace BL.Logic.SystemServices.TempStorage
{
    public interface ITempStorageService
    {
        int AddToStore(EnumObjects ownerType, int ownerId, int objectId, object storeobject);
        object GetStoreObject(EnumObjects ownerType, int ownerId, int objectId);
        object GetStoreObject(int objectId);
        object ExtractStoreObject(EnumObjects ownerType, int ownerId, int objectId);
        object ExtractStoreObject(int objectId);
        void RemoveStoreObject(int objectId);
    }
}