using RADataManagerLibrary.Helpers;
using RADataManagerLibrary.Internal.DataAccess;
using RADataManagerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RADataManagerLibrary.DataAccess
{
    public class SaleData
    {
        //Save sale to DB
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            //TODO: make it SOLID/Better
            //Filling in the sale detail models 

            //Model that is gonna be populated from foreach
            List<SaleDetailDBModel> cartDetails = new List<SaleDetailDBModel>();
            //For Qcalls
            ProductData products = new ProductData();
            //Get the tax rate from appsettings
            var taxRate = ConfigHelper.GetTaxRate()/100;

            //sale.SaleDetails got populated from frontend cart
            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                //Get information about this product from database Qcall
                var productInfo = products.GetById(detail.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"The product Id of { detail.ProductId} was not found in the database");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }

                cartDetails.Add(detail);
            }

            //Sale to save
            SaleDBModel sale = new SaleDBModel
            {
                Subtotal = cartDetails.Sum(x => x.PurchasePrice),
                Tax = cartDetails.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.Subtotal + sale.Tax;

            //Saving the sale
            SqlDataAccess sql = new SqlDataAccess();
            sql.SaveData("dbo.spSale_Insert", sale, "RAData");

            //Get id from the sale model
            sale.Id = sql.LoadData<int, dynamic>("dbo.spSale_LookUp",
                                                    new { sale.CashierId, sale.SaleDate },
                                                    "RAData").FirstOrDefault();

            //Finish filling in the sale detail models
            foreach (var item in cartDetails)
            {
                item.SaleId = sale.Id;
                //Save the sale detail Model
                sql.SaveData("dbo.spSaleDetail_Insert", item, "RAData");
            }
        }
    }
}
