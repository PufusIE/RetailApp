using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Api
{
    public class SaleEndpoint : ISaleEndpoint
    {
        private readonly IAPIHelper _apiHelper;

        public SaleEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task PostSale(SaleModel sale)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Sale", sale))
            {
                if (response.IsSuccessStatusCode == true)
                {
                    // Log result?
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<List<SaleReportModel>> GetAllSales()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/Sale/GetSalesReport"))
            {
                if (response.IsSuccessStatusCode == true)
                {
                    var result = await response.Content.ReadAsAsync<List<SaleReportModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
