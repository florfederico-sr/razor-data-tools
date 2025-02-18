using System;
namespace razor_data_tools.Models
{
    public class Tunecore_Earning
    {
        
        public string SR_Tunecore_Artist_ID { get; set; } = string.Empty;  //TODO Generate a GUID in the import code and assign to the artist being imported
        public string Sales_Period { get; set; } = string.Empty;
        public string Posted_Date { get; set; } = string.Empty;
        public string Store_Name { get; set; } = string.Empty;
        public string Country_Of_Sale { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Release_Type { get; set; } = string.Empty;
        public string Release_Title { get; set; } = string.Empty;
        public string Song_Title { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string UPC { get; set; } = string.Empty;
        public string Optional_UPC { get; set; } = string.Empty;
        public string TC_Song_ID { get; set; } = string.Empty;
        public string Optional_ISRC { get; set; } = string.Empty;
        public string Sales_Type { get; set; } = string.Empty;
        public string Num_Units_Sold { get; set; } = string.Empty;
        public string Per_Unit_Price { get; set; } = string.Empty;
        public string Net_Sales { get; set; } = string.Empty;
        public string Net_Sales_Currency { get; set; } = string.Empty;
        public string Exchange_Rate { get; set; } = string.Empty;
        public string Total_Earned { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string IMPORT_FILENAME { get; set; } = string.Empty;
        public string Artist_Id { get; set; } = string.Empty;

    }

    /// <summary>
    /// Simple class for select list
    /// </summary>
    public class Tunecore_Earnings_Customer
    {
        public string Artist { get; set; }
        //public string SR_Tunecore_Artist_ID { get; set; }
    }
}
