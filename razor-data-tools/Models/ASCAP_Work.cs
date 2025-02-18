using System;
namespace razor_data_tools.Models
{   

    public class ASCAP_Work
    {
        public string Party_ID_1 { get; set; } = string.Empty;
        public string Party_ID_2 { get; set; } = string.Empty;
        public string Party_ID { get; set; } = string.Empty;
        public string Total_Number_Of_Works { get; set; } = string.Empty;
        public string Member_Name { get; set; } = string.Empty;
        public string Work_Title { get; set; } = string.Empty;
        public string ASCAP_Work_ID { get; set; } = string.Empty;
        public string Interested_Parties { get; set; } = string.Empty;
        public string IPI_Number { get; set; } = string.Empty;
        public string Interested_Party_Status { get; set; } = string.Empty;
        public string PartyRole { get; set; } = string.Empty;
        public string Society { get; set; } = string.Empty;
        public string OwnershipPct { get; set; } = string.Empty;
        public string CollectPct { get; set; } = string.Empty;
        public string Registration_Date { get; set; } = string.Empty;
        public string Registration_Status { get; set; } = string.Empty;
        public string Surveyed_Work { get; set; } = string.Empty;
        public string ISWC_Number { get; set; } = string.Empty;
        public string IMPORT_FILENAME { get; set; } = string.Empty;


    }

    /// <summary>
    /// Simple class for select list
    /// </summary>
    public class ASCAP_Dupe
    {
        //TODO:  Implement this...
        //Query for the values listed, get a hash of the values and compare with the row being inserted.
        //[Party_ID_1], [Party_ID_2], [Party_ID], [Total_Number_Of_Works], [Member_Name], [Work_Title], [ASCAP_Work_ID],
        //[Interested_Parties], [IPI_Number], [Interested_Party_Status], [PartyRole], [Society], [OwnershipPct], [CollectPct],
        //[Registration_Date], [Registration_Status], [Surveyed_Work], [ISWC_Number],  [IMPORT_FILENAME]) ");

        public string PARTICIPANT_NAME { get; set; }
        public string PARTICIPANT_NUM { get; set; }

    }





}
