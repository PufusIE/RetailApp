using RADataManagerLibrary.Models;
using System.Collections.Generic;

namespace RADataManagerLibrary.DataAccess
{
    public interface IInventoryData
    {
        List<InventoryModel> GetInventory();
        void SaveInventoryRecord(InventoryModel item);
    }
}