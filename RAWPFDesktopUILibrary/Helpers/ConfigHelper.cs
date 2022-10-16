using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Helpers
{
    //For app.config work (TODO: move this from  config to the API)
    public class ConfigHelper : IConfigHelper
    {
        public decimal GetTaxRate()
        {
            decimal output = 0;

            string rateText = ConfigurationManager.AppSettings["TaxRate"];

            bool IsValidTaxRate = Decimal.TryParse(rateText, out output);

            if (IsValidTaxRate == false)
            {
                throw new ConfigurationErrorsException("The tax rate is not set up properly");
            }

            output = output / 100;

            return output;
        }
    }
}
