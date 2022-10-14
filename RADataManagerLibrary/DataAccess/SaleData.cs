using Microsoft.Extensions.Configuration;
using RADataManagerLibrary.Internal.DataAccess;
using RADataManagerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RADataManagerLibrary.DataAccess
{
    public class SaleData : ISaleData
    {
        private readonly IProductData _productData;
        private readonly ISqlDataAccess _sql;
        private readonly IConfiguration _config;

        public SaleData(IProductData productData, ISqlDataAccess sql, IConfiguration config)
        {
            _productData = productData;
            _sql = sql;
            _config = config;
        }

        public decimal GetTaxRate()
        {
            decimal output = 0;

            string rateText = _config.GetValue<string>("TaxRate");

            bool IsValidTaxRate = Decimal.TryParse(rateText, out output);

            if (IsValidTaxRate == false)
            {
                throw new ConfigurationErrorsException("The tax rate is not set up properly");
            }

            output = output / 100;

            return output;
        }

        //Save sale to DB
        public void SaveSale(SaleModel cartInfo, string cashierId)
        {
            //Filling in the sale detail models 

            //Model that is gonna be populated from foreach
            //And items from this list gonna be inserted into DB
            List<SaleDetailDBModel> saleDetails = new List<SaleDetailDBModel>();

            //Get the tax rate from appsettings
            var taxRate = GetTaxRate();

            //sale.SaleDetails got populated from frontend cart
            foreach (var item in cartInfo.SaleDetails)
            {
                var cartItem = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                //Get information about this product from database Qcall
                var productInfo = _productData.GetById(cartItem.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {cartItem.ProductId} was not found in the database");
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
            try
            {
                _sql.StartTransaction("RAData");

                //Saving the sale                    
                _sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                //Get id from the sale model
                sale.Id = _sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_LookUp",
                                                        new { sale.CashierId, sale.SaleDate })
                                                        .FirstOrDefault();

                //Finish filling in the sale detail models
                foreach (var item in saleDetails)
                {
                    item.SaleId = sale.Id;

                    //Save the sale detail Model
                    _sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }

                _sql.CommitTransaction();
            }
            catch
            {
                _sql.RollbackTransaction();
                throw;
            }
        }

        public List<SaleReportModel> GetSaleReport()
        {
            var output = _sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "RAData");

            return output;
        }
    }
}
