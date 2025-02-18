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
    public class ASCAP_Earnings_ExportModel : PageModel
    {
        //list containing the select items bound OnGet
        private List<SelectListItem> _customerItems { get; set; }
        //The selectlist that will be filled with CustomerItems and bound to the control on the page with Model.Customers
        public SelectList Customers { get; set; }
        //property to receive the selected value
        [BindProperty(SupportsGet = true)]
        public string sSelectedCustomer { get; set; }

        //constructor
        public ASCAP_Earnings_ExportModel()
        {
            _customerItems = new List<SelectListItem>();
            List<ASCAP_Earnings_Customer> ListOfEarningsCustomers = General.ConvertASCAPCustomerToList(Data.Services.SelectAllASCAPEarningsParticipantNameAndNumber());

            //Loop through the customers and create the select list
            foreach (var customer in ListOfEarningsCustomers)
            {
                _customerItems.Add(new SelectListItem(customer.Party_Name, customer.Party_Id));

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

            var lstData = Utils.General.ConvertASCAPDataTableToList(Data.Services.SelectASCAPEarnings(sParticipantNum));
            var sb = new StringBuilder();
            sb.AppendLine("File_Type ,Statement_Recipient_Name ,Statement_Recipient_ID ,Party_Name ,Party_ID ,Legal_Earner_Party_ID ,Legal_Earner_Party_Name ,Distribution_Date ,Distribution_Year" +
                            ", Distribution_Quarter, File_Distribution_Year, File_Distribution_Quarter, Country_Name, Licensor, Licensor_Code, Performance_Start_Date, Performance_End_Date, Duration, Number_of_Plays" +
                            ", Work_Title, Work_ID, Member_Share, Series_Name, Program_Name, Dollar_GENERAL, Dollar_RADIO, Dollar_TV, Dollar_TV_FILM, Dollar_CINEMA, Dollar_TOTAL, Revenue_Class_Code" +
                            ", Revenue_Class_Description, Adjustment_Distribution_Date, Adjustment_Indicator, Adjustment_Reason_Code, Role_Type, Type_Of_Right, Territory, Performance_Source, Music_User, Network_Service" +
                            ", Survey_Type, Day_Part_Code, CA_Pct, Classification_Code, Performance_Type, Performing_Artist, Composer_Name, EE_Share, Credits, Premium_Credits, Premium_Dollars, Original_Distribution_Date ");
            foreach (var data in lstData)
            {
                sb.AppendLine(data.File_Type + "," +
                                data.Statement_Recipient_Name + "," +
                                data.Statement_Recipient_ID + "," +
                                data.Party_Name + "," +
                                data.Party_ID + "," +
                                data.Legal_Earner_Party_ID + "," +
                                data.Legal_Earner_Party_Name + "," +
                                data.Distribution_Date + "," +
                                data.Distribution_Year + "," +
                                data.Distribution_Quarter + "," +
                                data.File_Distribution_Year + "," +
                                data.File_Distribution_Quarter + "," +
                                data.Country_Name + "," +
                                data.Licensor + "," +
                                data.Licensor_Code + "," +
                                data.Performance_Start_Date + "," +
                                data.Performance_End_Date + "," +
                                data.Duration + "," +
                                data.Number_of_Plays + "," +
                                "\"" + data.Work_Title + "\"" +"," +
                                data.Work_ID + "," +
                                data.Member_Share + "," +
                                data.Series_Name + "," +
                                data.Program_Name + "," +
                                data.Dollar_GENERAL + "," +
                                data.Dollar_RADIO + "," +
                                data.Dollar_TV + "," +
                                data.Dollar_TV_FILM + "," +
                                data.Dollar_CINEMA + "," +
                                data.Dollar_TOTAL + "," +
                                data.Revenue_Class_Code + "," +
                                data.Revenue_Class_Description + "," +
                                data.Adjustment_Distribution_Date + "," +
                                data.Adjustment_Indicator + "," +
                                data.Adjustment_Reason_Code + "," +
                                data.Role_Type + "," +
                                data.Type_Of_Right + "," +
                                data.Territory + "," +
                                data.Performance_Source + "," +
                                data.Music_User + "," +
                                data.Network_Service + "," +
                                data.Survey_Type + "," +
                                data.Day_Part_Code + "," +
                                data.CA_Pct + "," +
                                data.Classification_Code + "," +
                                data.Performance_Type + "," +
                                data.Performing_Artist + "," +
                                data.Composer_Name + "," +
                                data.EE_Share + "," +
                                data.Credits + "," +
                                data.Premium_Credits + "," +
                                data.Premium_Dollars + "," +
                                data.Original_Distribution_Date);

            }
            //Message = "Returning File...";
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(sb.ToString());

            return File(bytes, "text/csv", "export.csv");

        }
    }
}