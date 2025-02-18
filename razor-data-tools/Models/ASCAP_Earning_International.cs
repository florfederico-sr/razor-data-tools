using System;
namespace razor_data_tools.Models
{
    public class ASCAP_Earning_International
    {
        public string File_Type { get; set; }
        public string Statement_Recipient_Name { get; set; }
        public string Statement_Recipient_ID { get; set; }
        public string Party_Name { get; set; }
        public string Party_ID { get; set; }
        public string Legal_Earner_Party_ID { get; set; }
        public string Legal_Earner_Party_Name { get; set; }
        public string Distribution_Date { get; set; }
        public string Country_Name { get; set; }
        public string Licensor { get; set; }
        public string Licensor_Code { get; set; }
        public string Performance_Start_Date { get; set; }
        public string Performance_End_Date { get; set; }
        public string Work_Title { get; set; }
        public string Work_ID { get; set; }
        public string Member_Share { get; set; }
        public string Series_Name { get; set; }
        public string Program_Name { get; set; }

        //old international
        public string Revenue_Class_Code { get; set; }
        public string Revenue_Class_Description { get; set; }
        public string Dollar_Amount { get; set; }   //should be mapped to Dollar_Total

        //new international
        public string Dollar_GENERAL { get; set; }
        public string Dollar_RADIO { get; set; }
        public string Dollar_TV { get; set; }
        public string Dollar_TV_FILM { get; set; }
        public string Dollar_CINEMA { get; set; }
        public string Dollar_TOTAL { get; set; }   //Should include Dollar_Amount

        public string Adjustment_Distribution_Date { get; set; }
        public string Adjustment_Indicator { get; set; }
        public string Role_Type { get; set; }
        public string Type_Of_Right { get; set; }
        public string Territory { get; set; }
        public string Import_Filename { get; set; }

    }
}
