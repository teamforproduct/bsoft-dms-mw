using System.Collections.Generic;
using BL.Model.Enums;

namespace BL.Logic.SystemServices.TempStorage
{
    public interface ITempStorageService
    {
        int AddToStore(object storeobject, EnumObjects? ownerType = null, int? ownerId = null, int? objectId = null);
        object GetStoreObject(EnumObjects ownerType, int ownerId, int objectId);
        object GetStoreObject(int objectId);
        object ExtractStoreObject(EnumObjects ownerType, int ownerId, int objectId);
        object ExtractStoreObject(int objectId);
        void RemoveStoreObject(int objectId);
        List<object> ExtractStoreObjectList(EnumObjects ownerType, int ownerId, int objectId);
        List<object> GetStoreObjectList(EnumObjects ownerType, int ownerId, int objectId);
    }
}