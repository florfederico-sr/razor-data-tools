using System;
using SoftCircuits.CsvParser;
namespace razor_data_tools.Models
{
    public class ASCAP_Earning
    {
        //Names can be addressed in the file as they are below by replacing underscores with a space

        //TODO:  Add ColumnMap - column mapping attributes to the data class


        public string File_Type { get; set; } = string.Empty;

        public string Statement_Recipient_Name { get; set; } = string.Empty;
        public string Statement_Recipient_ID { get; set; } = string.Empty;
        public string Party_Name { get; set; } = string.Empty;
        public string Party_ID { get; set; } = string.Empty;
        public string Legal_Earner_Party_ID { get; set; } = string.Empty;
        public string Legal_Earner_Party_Name { get; set; } = string.Empty;
        public string Distribution_Date { get; set; } = string.Empty;
        public string Distribution_Year { get; set; } = string.Empty;
        public string Distribution_Quarter { get; set; } = string.Empty;
        //File_Distribution_Year and File_Distribution_Quarter are derived by the below
        //Writer:            
        //    2020 Q1:  	2020 01 D  
        //                2020 02 I
        //    2020 Q2:  	2020 04 D
        //                2020 05 I
        //    2020 Q3:  	2020 07 D  
        //                2020 08 I
        //    2020 Q4:  	2020 10 D
        //                2020 11 I
        //Publisher:
        //    2020 Q1:  	2020 02 I
        //                2020 03 D
        //    2020 Q2:  	2020 05 I
        //                2020 06 D
        //    2020 Q3:  	2020 08 I  
        //                2020 09 D
        //    2020 Q4:  	2020 11 I
        //                  2020 12 D
        public string File_Distribution_Year { get; set; } = string.Empty;
        public string File_Distribution_Quarter { get; set; } = string.Empty;

        public string Country_Name { get; set; } = "USA";
        public string Licensor { get; set; } = string.Empty;
        public string Licensor_Code { get; set; } = string.Empty;
        public string Performance_Start_Date { get; set; } = string.Empty;
        public string Performance_End_Date { get; set; } = string.Empty;

        public string Duration { get; set; } = string.Empty;
        public string Number_of_Plays { get; set; } = string.Empty;

        public string Work_Title { get; set; } = string.Empty;
        public string Work_ID { get; set; } = string.Empty;
        public string Member_Share { get; set; } = string.Empty;
        public string Series_Name { get; set; } = string.Empty;  //aka Series or Film/Attraction or Series Name
        public string Program_Name { get; set; } = string.Empty;

        //old international
        public string Dollar_GENERAL { get; set; } = string.Empty;  //All that start with Dollar are $ in the file
        public string Dollar_RADIO { get; set; } = string.Empty;
        public string Dollar_TV { get; set; } = string.Empty;
        public string Dollar_TV_FILM { get; set; } = string.Empty;
        public string Dollar_CINEMA { get; set; } = string.Empty;

        public string Dollar_TOTAL { get; set; } = string.Empty;   //aka $ TOTAL	or $ Amount or 	Dollars

        //new international
        public string Revenue_Class_Code { get; set; } = string.Empty;
        public string Revenue_Class_Description { get; set; } = string.Empty;

        public string Adjustment_Distribution_Date { get; set; } = string.Empty;
        public string Adjustment_Indicator { get; set; } = string.Empty;

        public string Adjustment_Reason_Code { get; set; } = string.Empty;

        public string Role_Type { get; set; } = string.Empty;
        public string Type_Of_Right { get; set; } = string.Empty;
        public string Territory { get; set; } = string.Empty;

        public string Performance_Source { get; set; } = string.Empty;//Performance Source/Broadcast Medium
        public string Music_User { get; set; } = string.Empty;///Music User
        public string Music_User_Genre { get; set; } = string.Empty;///Music User
        public string Network_Service { get; set; } = string.Empty;//Network Service
        public string Survey_Type { get; set; } = string.Empty;//Survey Type
        public string Day_Part_Code { get; set; } = string.Empty;//Day Part Code
        public string Series_or_Film { get; set; } = string.Empty;//Day Part Code
        public string CA_Pct { get; set; } = string.Empty;//CA%
        public string Classification_Code { get; set; } = string.Empty;//Classification Code
        public string Performance_Type { get; set; } = string.Empty;//Performance Type(Usage)
        public string Performing_Artist { get; set; } = string.Empty;//Performing Artist
        public string Composer_Name { get; set; } = string.Empty;//Composer Name
        public string EE_Share { get; set; } = string.Empty;//EE Share
        public string Credits { get; set; } = string.Empty;//Credits
        public string Premium_Credits { get; set; } = string.Empty;//Premium Credits
        public string Premium_Dollars { get; set; } = string.Empty;//Premium Dollars
        public string Original_Distribution_Date { get; set; } = string.Empty;//Original Distribution Date
        public string Licensor_Flag { get; set; } = string.Empty;//Original Distribution Date

        public string Import_Filename { get; set; } = string.Empty;

    }


    /// <summary>
    /// Simple class for select list
    /// </summary>
    public class ASCAP_Earnings_Customer
    {
        public string Party_Name { get; set; }
        public string Party_Id { get; set; }
    }







}
