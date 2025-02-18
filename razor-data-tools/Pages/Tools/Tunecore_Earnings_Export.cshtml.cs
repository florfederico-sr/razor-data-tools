using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using razor_data_tools.Models;
using razor_data_tools.Utils;

namespace razor_data_tools.Pages.Tools
{
    public class Tunecore_Earnings_ExportModel : PageModel
    {        //list containing the select items bound OnGet
        private List<SelectListItem> _customerItems { get; set; }
        //The selectlist that will be filled with CustomerItems and bound to the control on the page with Model.Customers
        public SelectList Customers { get; set; }
        //property to receive the selected value
        [BindProperty(SupportsGet = true)]
        public string sSelectedCustomer { get; set; }

        //constructor
        public Tunecore_Earnings_ExportModel()
        {
            _customerItems = new List<SelectListItem>();
            List<Tunecore_Earnings_Customer> ListOfEarningsCustomers = General.ConvertTunecoreCustomerToList(Data.Services.SelectAllTunecoreEarningsParticipantNameAndNumber());

            //Loop through the customers and create the select list
            foreach (var customer in ListOfEarningsCustomers)
            {
                _customerItems.Add(new SelectListItem(customer.Artist, customer.Artist));

            }
        }



        public void OnGet()
        {
            Customers = new SelectList(_customerItems, "Value", "Text");
        }

        public ActionResult OnPostFileDownload()
        {
            string sParticipantNum = sSelectedCustomer;
            return DownloadCsv(sParticipantNum);
        }



        public FileResult DownloadCsv(string sArtist)
        {

            var lstData = Utils.General.ConvertTunecoreDataTableToList(Data.Services.SelectTunecoreEarnings(sArtist));
            var sb = new StringBuilder();
            sb.AppendLine("Sales_Period ,Posted_Date ,Store_Name ,Country_Of_Sale ,Artist ,Release_Type ,Release_Title ,Song_Title ,Label ,UPC ,Optional_UPC ,TC_Song_ID ,Optional_ISRC ,Sales_Type ,Num_Units_Sold ,Per_Unit_Price ,Net_Sales ,Net_Sales_Currency ,Exchange_Rate ,Total_Earned ,Currency ,IMPORT_FILENAME ");
            foreach (var data in lstData)
            {
                sb.AppendLine(data.Sales_Period + "," +
                                    data.Posted_Date + "," +
                                    data.Store_Name + "," +
                                    data.Country_Of_Sale + "," +
                                    data.Artist + "," +
                                    data.Release_Type + "," +
                                   "\"" + data.Release_Title + "\"" + "," +
                                    "\"" + data.Song_Title + "\""  + "," +
                                    data.Label + "," +
                                    data.UPC + "," +
                                    data.Optional_UPC + "," +
                                    data.TC_Song_ID + "," +
                                    data.Optional_ISRC + "," +
                                    data.Sales_Type + "," +
                                    data.Num_Units_Sold + "," +
                                    data.Per_Unit_Price + "," +
                                    data.Net_Sales + "," +
                                    data.Net_Sales_Currency + "," +
                                    data.Exchange_Rate + "," +
                                    data.Total_Earned + "," +
                                    data.Currency + "," +                                    
                                    data.IMPORT_FILENAME );
            }
            //Message = "Returning File...";
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(sb.ToString());

            return File(bytes, "text/csv", "export.csv");

        }
    }
}