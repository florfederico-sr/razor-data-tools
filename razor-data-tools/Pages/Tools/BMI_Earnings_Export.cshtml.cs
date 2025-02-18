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
    public class BMI_Earnings_ExportModel : PageModel
    {

        //list containing the select items bound OnGet
        private List<SelectListItem> _customerItems { get; set; }
        //The selectlist that will be filled with CustomerItems and bound to the control on the page with Model.Customers
        public SelectList Customers { get; set; }
        //property to receive the selected value
        [BindProperty(SupportsGet = true)]
        public string sSelectedCustomer { get; set; }

        //constructor
        public BMI_Earnings_ExportModel()
        {
            _customerItems = new List<SelectListItem>();
            List<BMI_Earnings_Customer> ListOfEarningsCustomers = General.ConvertBMICustomerToList(Data.Services.SelectAllBMIEarningsParticipantNameAndNumber());

            //Loop through the customers and create the select list
            foreach (var customer in ListOfEarningsCustomers)
            {
                _customerItems.Add(new SelectListItem(customer.PARTICIPANT_NAME, customer.PARTICIPANT_NUM));

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



        public FileResult DownloadCsv(string sParticipantNum)
        {

            var lstData = Utils.General.ConvertBMIDataTableToList(Data.Services.SelectBMIEarnings(sParticipantNum));
            var sb = new StringBuilder();
            sb.AppendLine("PERIOD, W_OR_P, PARTICIPANT_NAME, PARTICIPANT_NUM, IP_NUM, TITLE_NAME, TITLE_NUM, PERF_SOURCE, COUNTRY_OF_PERFORMANCE, SHOW_NAME, EPISODE_NAME, SHOW_NUM, USE_CODE, TIMING, PARTICIPANT_PCT, PERF_COUNT, BONUS_LEVEL, ROYALTY_AMOUNT, WITHHOLD, PERF_PERIOD, CURRENT_ACTIVITY_AMT, HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS, STANDARDS_OR_TV_NET_THEME_BONUS, FOREIGN_SOCIETY_ADJUSTMENT, COMPANY_CODE, COMPANY_NAME");
            foreach (var data in lstData)
            {
                sb.AppendLine(data.PERIOD + "," +
                                data.W_OR_P + "," +
                                "\"" + data.PARTICIPANT_NAME + "\"" + "," +
                                data.PARTICIPANT_NUM + "," +
                                data.IP_NUM + "," +
                                "\"" + data.TITLE_NAME + "\"" +"," +
                                data.TITLE_NUM + "," +
                                data.PERF_SOURCE + "," +
                                data.COUNTRY_OF_PERFORMANCE + "," +
                                data.SHOW_NAME + "," +
                                data.EPISODE_NAME + "," +
                                data.SHOW_NUM + "," +
                                data.USE_CODE + "," +
                                data.TIMING + "," +
                                data.PARTICIPANT_PCT + "," +
                                data.PERF_COUNT + "," +
                                data.BONUS_LEVEL + "," +
                                data.ROYALTY_AMOUNT + "," +
                                data.WITHHOLD + "," +
                                data.PERF_PERIOD + "," +
                                data.CURRENT_ACTIVITY_AMT + "," +
                                data.HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS + "," +
                                data.STANDARDS_OR_TV_NET_THEME_BONUS + "," +
                                data.FOREIGN_SOCIETY_ADJUSTMENT + "," +
                                data.COMPANY_CODE + "," +
                                data.COMPANY_NAME);
            }
            //Message = "Returning File...";
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(sb.ToString());

            return File(bytes, "text/csv", "export.csv");

        }
    }
}