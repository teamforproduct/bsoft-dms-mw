using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BL.Model.Enums;
using BL.Model.SystemCore;

namespace BL.Logic.SystemServices.TempStorage
{
    public class TempStorageService : ITempStorageService
    {
        private List<TempStoreObject> storeObjects;
        private readonly Timer _cleanTimer;
        private object lockObject = new object();
        private readonly int timeToStore = 10; // minutes

        public TempStorageService()
        {
            var timerRefresh = timeToStore * 60 * 1000;
            storeObjects = new List<TempStoreObject>();
            _cleanTimer = new Timer(OnSinchronize, null, timerRefresh, timerRefresh);
        }

        public int AddToStore(EnumObjects ownerType, int ownerId, int objectId, object storeobject)
        {
            storeObjects.RemoveAll(x => x.ObjectId == objectId && x.OwnerId == ownerId && x.OwnerType == ownerType);
            var newObj = new TempStoreObject
            {
                Id = 0,
                LastUsage = DateTime.Now,
                OwnerId = ownerId,
                OwnerType = ownerType,
                ObjectId = objectId,
                StoreObject = storeobject
            };
            lock (lockObject)
            {
                storeObjects.Add(newObj);

                // ToDo переполнение переменной mod int.MaxValue()
                newObj.Id = storeObjects.Max(x => x.Id) + 1;
            }
            return newObj.Id;

        }

        public object GetStoreObject(EnumObjects ownerType, int ownerId, int objectId)
        {
            var obj = storeObjects.FirstOrDefault(x => x.ObjectId == objectId && x.OwnerId == ownerId && x.OwnerType == ownerType);
            if (obj!=null) obj.LastUsage = DateTime.Now;
            return obj?.StoreObject;
        }

        public object GetStoreObject(int objectId)
        {
            var obj = storeObjects.FirstOrDefault(x => x.Id == objectId);
            if (obj != null) obj.LastUsage = DateTime.Now;
            return obj?.StoreObject;
        }

        public object ExtractStoreObject(EnumObjects ownerType, int ownerId, int objectId)
        {
            var obj = storeObjects.FirstOrDefault(x => x.ObjectId == objectId && x.OwnerId == ownerId && x.OwnerType == ownerType);
            if (obj!=null) storeObjects.Remove(obj);
            return obj?.StoreObject;
        }

        public object ExtractStoreObject(int objectId)
        {
            var obj = storeObjects.FirstOrDefault(x => x.Id == objectId);
            if (obj != null) storeObjects.Remove(obj);
            return obj?.StoreObject;
        }

        public void RemoveStoreObject(int objectId)
        {
            var obj = storeObjects.FirstOrDefault(x => x.Id == objectId);
            if (obj != null) storeObjects.Remove(obj);
        }

        private void OnSinchronize(object state)
        {
            storeObjects.RemoveAll(x => (DateTime.Now - x.LastUsage).TotalMinutes > timeToStore);
        }
    }
}