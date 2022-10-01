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
        public void SaveSale(SaleModel cartInfo, string cashierId)
        {
            //TODO: Add DI to dll's

            //Filling in the sale detail models 

            //Model that is gonna be populated from foreach
            //And items from this list gonna be inserted into DB
            List<SaleDetailDBModel> saleDetails = new List<SaleDetailDBModel>();
            
            //For Qcalls
            ProductData products = new ProductData();
            
            //Get the tax rate from appsettings
            var taxRate = ConfigHelper.GetTaxRate()/100;

            //sale.SaleDetails got populated from frontend cart
            foreach (var item in cartInfo.SaleDetails)
            {
                var cartItem = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                //Get information about this product from database Qcall
                var productInfo = products.GetById(cartItem.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"The product Id of { cartItem.ProductId} was not found in the database");
                }

                cartItem.PurchasePrice = (productInfo.RetailPrice * cartItem.Quantity);

                if (productInfo.IsTaxable)
                {
                    cartItem.Tax = (cartItem.PurchasePrice * taxRate);
                }

                saleDetails.Add(cartItem);
            }

            //Sale to save
            SaleDBModel sale = new SaleDBModel
            {
                Subtotal = saleDetails.Sum(x => x.PurchasePrice),
                Tax = saleDetails.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.Subtotal + sale.Tax;

            //Making a query
            using (SqlDataAccess sql = new SqlDataAccess())
            {
                try
                {
                    sql.StartTransaction("RAData");

                    //Saving the sale                    
                    sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                    //Get id from the sale model
                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_LookUp",
                                                            new { sale.CashierId, sale.SaleDate })
                                                            .FirstOrDefault();

                    //Finish filling in the sale detail models
                    foreach (var item in saleDetails)
                    {
                        item.SaleId = sale.Id;

                        //Save the sale detail Model
                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                    }

                    sql.CommitTransaction();
                }
                catch
                {
                    sql.RollbackTransaction();
                    throw;
                }
            }
        }

        public List<SaleReportModel> GetSaleReport()
        {
            SqlDataAccess sql = new SqlDataAccess();

            var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "RAData");

            return output;
        }
    }
}
