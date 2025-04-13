using RetailManagementSystem.Models.Models.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.ResultModels
{
    public class DeliveryDetailedResult
    {
        public int DeliveryId { get; set; }
        public string Instructions { get; set; }
        public string GoogleMapLocation { get; set; }
        public string Status { get; set; } = "Pending";
        public StoreData storeData { get; set; }
        public List<OrderFromBill> OrdersFromBill { get; set; }
    }

    public class RetailerData
    {
        public string RetailName { get; set; }
        public string OwnerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
    public class StoreData
    {
        public string StoreName { get; set; }
        public string PhotoOfStore { get; set; }
        public string ProfilePhoto { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string IndexForBill { get; set; }

    }

    public class OrderFromBill
    {
        public StockData Stock { get; set; }
        public int Quantity { get; set; }
        public float TotalAmount { get; set; }
        public RetailerData Retailer { get; set; }
    }

    public class StockData
    {
        public int Index { get; set; }
        public ProductData Product { get; set; }
        public int Quantity { get; set; }
        public float MarginPercentage { get; set; }
        public bool IsReturnable { get; set; }
        public int WarehouseId { get; set; }
    }

    public class ProductData
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal MRP { get; set; }
        public string Category { get; set; }
    }

}
