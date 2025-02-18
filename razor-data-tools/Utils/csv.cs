using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using SoftCircuits.CsvParser;
using razor_data_tools.Models;
using System.Linq;
using System.Security.Cryptography;
using razor_data_tools.Data;

namespace razor_data_tools.Utils
{
    public static class Csv
    {
        public static int ImportBMIEarningsCsvAndInsertIntoDatabase(MemoryStream csvStream, string importFileName)
        {
            int importedCount = -1;

            List<BMI_Earning> earnings = new List<BMI_Earning>();
            try
            {
                //TODO: First query the database for the FILEHASH to determine if it has been uploaded already
                // IF the file has been uploaded exit method
                // Implement the file hash then set the FILEHASH property in the earning object
                // will need to give the users the ability to delete files from the database
                // so they can re-upload

                string sFileHash = string.Empty;


                //TODO: now query the database for the has just created and determine next steps (import or not)
                
                // Read the data from disk
                string[] columns = null;
                try
                {
                    using (CsvReader reader = new CsvReader(csvStream))
                    {
                        int row = 0;
                        while (reader.ReadRow(ref columns))
                        {


                            //remove all empty elements from an array **BEWARE this will shorten the length of the array**
                            //not needed here
                            //columns = columns.Where(x => !string.IsNullOrEmpty(x)).ToArray();


                            //check to see if the file has been imported before
                            //skip the header and check if the importedCount is -1 then this is the first time through the loop
                            //otherwise there is no need to check again - skip this section
                            //if (row != 0 && importedCount < 0)
                            //{
                            //    string participant_num_to_check = columns[3].ToString();
                            //    importedCount = Data.Services.SelectCountOfBMIEarningsImportedFromFile(participant_num_to_check, importFileName);
                            //    //if records have been imported from this file return the count and exit the method
                            //    if (importedCount > 0)
                            //    {
                            //        return importedCount;//exit the method
                            //    }
                            //}


                            if (row != 0 && columns.Length > 25)  //skip the header because BMI malforms the header by adding a comma at the end of the header row
                            {
                                //Debug.WriteLine(string.Join(", ", columns));
                                BMI_Earning earning = new BMI_Earning();

                                earning.PERIOD = columns[0].ToString();
                                earning.W_OR_P = columns[1].ToString();
                                earning.PARTICIPANT_NAME = columns[2].ToString();
                                earning.PARTICIPANT_NUM = columns[3].ToString();
                                earning.IP_NUM = columns[4].ToString();
                                earning.TITLE_NAME = columns[5].ToString();
                                earning.TITLE_NUM = columns[6].ToString();
                                earning.PERF_SOURCE = columns[7].ToString();
                                earning.COUNTRY_OF_PERFORMANCE = columns[8].ToString();
                                earning.SHOW_NAME = columns[9].ToString();
                                earning.EPISODE_NAME = columns[10].ToString();
                                earning.SHOW_NUM = columns[11].ToString();
                                earning.USE_CODE = columns[12].ToString();
                                earning.TIMING = columns[13].ToString();
                                earning.PARTICIPANT_PCT = columns[14].ToString();
                                earning.PERF_COUNT = columns[15].ToString();
                                earning.BONUS_LEVEL = columns[16].ToString();
                                earning.ROYALTY_AMOUNT = columns[17].ToString();
                                earning.WITHHOLD = columns[18].ToString();
                                earning.PERF_PERIOD = columns[19].ToString();
                                earning.CURRENT_ACTIVITY_AMT = columns[20].ToString();
                                earning.HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS = columns[21].ToString();
                                earning.STANDARDS_OR_TV_NET_THEME_BONUS = columns[22].ToString();
                                earning.FOREIGN_SOCIETY_ADJUSTMENT = columns[23].ToString();
                                earning.COMPANY_CODE = columns[24].ToString();
                                earning.COMPANY_NAME = columns[25].ToString();

                                earnings.Add(earning);


                            }
                            row++; //increment the row
                        }
                        importedCount = row;  //get the number of rows we are attempting to insert

                        //compute the hash NOTE this has to be after the parsing or the SoftCircuts csv parser breaks (unknown reason)
                        csvStream.Position = 0; //have to reset the stream position back to 0
                        sFileHash = Csv.ComputeFileHashAsync(csvStream);
                        if (Services.SelectFileHashForDupeCheck("BMI_Earnings", sFileHash))
                        {
                            return importedCount;
                        }
                    }
                    
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                
                //insert the list of earnings into the database
                Data.Services.InsertBMIEarnings(earnings, importFileName, sFileHash);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return importedCount;
        }


        //SR_Tunecore_Artist_ID

        public static int ImportTunecoreEarningsCsvAndInsertIntoDatabase(MemoryStream csvStream, string importFileName, string sArtistId)
        {
            int importedCount = -1;
            string SR_Tunecore_Artist_ID = string.Empty;
            string artist;
            string sFileHash = string.Empty;


           


            List<Tunecore_Earning> earnings = new List<Tunecore_Earning>();
            try
            {
                //CsvSettings settings = new CsvSettings();
                //settings.QuoteCharacter = '"';
                // Read the data from disk
                string[] columns = null;

                using (CsvReader reader = new CsvReader(csvStream))
                {
                    int row = 0;
                    while (reader.ReadRow(ref columns))
                    {
                        
                        if (row >= 1)
                        {
                            //Debug.WriteLine(string.Join(", ", columns));
                            Tunecore_Earning earning = new Tunecore_Earning();
                            //earning.SR_Tunecore_Artist_ID = SR_Tunecore_Artist_ID;
                            earning.Sales_Period = columns[0].ToString();
                            earning.Posted_Date = columns[1].ToString();
                            earning.Store_Name = columns[2].ToString();
                            earning.Country_Of_Sale = columns[3].ToString();
                            earning.Artist = columns[4].ToString();
                            earning.Release_Type = columns[5].ToString();
                            earning.Release_Title = columns[6].ToString();
                            earning.Song_Title = columns[7].ToString();
                            earning.Label = columns[8].ToString();
                            earning.UPC = columns[9].ToString();
                            earning.Optional_UPC = columns[10].ToString();
                            earning.TC_Song_ID = columns[11].ToString();
                            earning.Optional_ISRC = columns[12].ToString();
                            earning.Sales_Type = columns[13].ToString();
                            earning.Num_Units_Sold = columns[14].ToString();
                            earning.Per_Unit_Price = columns[15].ToString();
                            earning.Net_Sales = columns[16].ToString();
                            earning.Net_Sales_Currency = columns[17].ToString();
                            earning.Exchange_Rate = columns[18].ToString();
                            earning.Total_Earned = columns[19].ToString();
                            earning.Currency = columns[20].ToString();
                            earning.IMPORT_FILENAME = importFileName;
                            earning.Artist_Id = sArtistId;

                            earnings.Add(earning);

                        }
                        row++; //increment the row
                    }
                    importedCount = row - 1;  //get the number of rows we are attempting to insert

                    //compute the hash NOTE this has to be after the parsing or the SoftCircuts csv parser breaks (unknown reason)
                    csvStream.Position = 0;  //reset the postion of the stream to ensure that the entire stream is hashed
                    sFileHash = Csv.ComputeFileHashAsync(csvStream);
                    //query the database for the hash  if hasExists then exit - if not continue parsing                   
                    if(Services.SelectFileHashForDupeCheck("Tunecore_Earnings", sFileHash))
                    {
                        return importedCount;
                    }
                }
                //insert the list of earnings into the database
                Data.Services.InsertTunecoreEarnings(earnings, importFileName, sFileHash);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return importedCount;
        }


        public static int ImportASCAPWorksCsvAndInsertIntoDatabase(MemoryStream csvStream, string importFileName)
        {
            int importedCount = -1;
            string sFileHash = string.Empty;


            List<ASCAP_Work> ascap_works = new List<ASCAP_Work>();
            try
            {
                //CsvSettings settings = new CsvSettings();
                //settings.QuoteCharacter = '"';
                // Read the data from disk
                string[] columns = null;

                using (CsvReader reader = new CsvReader(csvStream))
                {
                    int row = 0;
                    while (reader.ReadRow(ref columns))
                    {


                        //check to see if the file has been imported before

                        //if it's the second row and we have not determined if the file has or has not been imported before
                        //if (row == 1 && importedCount < 0)
                        //{
                        //    //The only way a duplicate file can be prevented from imported again is by hashing the entire first rowl, which creates a unique identifier, then searching the staging.ASCAP_Works table for that hashed id.
                        //    //If it returns a row then it has been imported, if it doesn't then it's ok to import the file.

                        //    //create the has of the first row with values in the file.
                        //    StringBuilder sb = new StringBuilder();
                        //    //earning.SR_Tunecore_Artist_ID = SR_Tunecore_Artist_ID;
                        //    sb.Append(columns[0].ToString().Trim());
                        //    sb.Append(columns[1].ToString().Trim());
                        //    sb.Append(columns[2].ToString().Trim());
                        //    sb.Append(columns[3].ToString().Trim());
                        //    sb.Append(columns[4].ToString().Trim());
                        //    sb.Append(columns[5].ToString().Trim());
                        //    sb.Append(columns[6].ToString().Trim());
                        //    sb.Append(columns[7].ToString().Trim());
                        //    sb.Append(columns[8].ToString().Trim());
                        //    sb.Append(columns[9].ToString().Trim());
                        //    sb.Append(columns[10].ToString().Trim());
                        //    sb.Append(columns[11].ToString().Trim());
                        //    sb.Append(columns[12].ToString().Trim());
                        //    sb.Append(columns[13].ToString().Trim());
                        //    sb.Append(columns[14].ToString().Trim());
                        //    sb.Append(columns[15].ToString().Trim());
                        //    sb.Append(columns[16].ToString().Trim());
                        //    sb.Append(columns[17].ToString().Trim());
                        //    sb.Append(importFileName.Trim());
                                
                        //    //TODO:  This is where I should check for duplicate works imported....
                        //    importedCount = Data.Services.SelectCountOfASCAPWorksImportedFromFile(General.CreateMD5HashII(sb.ToString()));
                        //    //if records have been imported from this file return the count and exit the method
                        //    if (importedCount > 0)
                        //    {
                        //        return importedCount;//exit the method
                        //    }
                           
                        //}


                        if (row >= 1)
                        {
                            //Debug.WriteLine(string.Join(", ", columns));
                            ASCAP_Work work = new ASCAP_Work();
                            //earning.SR_Tunecore_Artist_ID = SR_Tunecore_Artist_ID;
                            work.Party_ID_1 = columns[0].ToString();
                            work.Party_ID_2 = columns[1].ToString();
                            work.Party_ID = columns[2].ToString();
                            work.Total_Number_Of_Works = columns[3].ToString();
                            work.Member_Name = columns[4].ToString();
                            work.Work_Title = columns[5].ToString();
                            work.ASCAP_Work_ID = columns[6].ToString();
                            work.Interested_Parties = columns[7].ToString();
                            work.IPI_Number = columns[8].ToString();
                            work.Interested_Party_Status = columns[9].ToString();
                            work.PartyRole = columns[10].ToString();
                            work.Society = columns[11].ToString();
                            work.OwnershipPct = columns[12].ToString();
                            work.CollectPct = columns[13].ToString();
                            work.Registration_Date = columns[14].ToString();
                            work.Registration_Status = columns[15].ToString();
                            work.Surveyed_Work = columns[16].ToString();
                            work.ISWC_Number = columns[17].ToString();
                            work.IMPORT_FILENAME = importFileName;
                           

                            ascap_works.Add(work);

                        }
                        row++; //increment the row
                    }
                    importedCount = row - 1;  //get the number of rows we are attempting to insert

                    //compute the hash NOTE this has to be after the parsing or the SoftCircuts csv parser breaks (unknown reason)
                    csvStream.Position = 0;  //reset the postion of the stream to ensure that the entire stream is hashed
                    sFileHash = Csv.ComputeFileHashAsync(csvStream);
                    //query the database for the hash  if hasExists then exit - if not continue parsing                   
                    if (Services.SelectFileHashForDupeCheck("ASCAP_Earnings", sFileHash))
                    {
                        return importedCount;
                    }
                }
                //insert the list of earnings into the database
                Data.Services.InsertASCAPWorks(ascap_works, importFileName);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return importedCount;
        }



        //TODO:  Refactor...
        public static int ImportASCAPEarningsCsvAndInsertIntoDatabase(MemoryStream csvStream, string importFileName)
        {
            int importedCount = -1;
            int iMappingChoice = -1;
            string sFileYear = string.Empty;
            string sFileQuarter = string.Empty;
            string sImportFileString = string.Empty;

            List<ASCAP_Earning> earnings = new List<ASCAP_Earning>();
            try
            {
                // Read the data from disk
                string[] columns = null;
                using (CsvReader reader = new CsvReader(csvStream))
                {
                    int row = 0;  //index of the first row in the file
                    while (reader.ReadRow(ref columns))
                    {
                        //first row - determine which mapping to use based on the column headers in the file
                        if (row == 0  && columns != null)
                        {                            
                            iMappingChoice = DetermineASCAPFileMappingFromHeader(columns);
                            row++;
                            continue;  //skip the rest of the code in this loop and move to next iteration
                            
                        }

                        //if it's the second row and we have not determined if the file has or has not been imported before
                        if (row == 1 && importedCount < 0)
                        {
                            string sPartyIdToCheck = columns[4].ToString();
                            if (!string.IsNullOrWhiteSpace(sPartyIdToCheck))
                            {
                                importedCount = Data.Services.SelectCountOfASCAPEarningsImportedFromFile(sPartyIdToCheck, importFileName);
                            }
                            else
                            {
                                //can't get the ASCAP Party ID to check if file exists.  Exit method and throw error;
                                return importedCount;
                            }
                            //if records have been imported from this file return the count and exit the method
                            if (importedCount > 0)
                            {
                                return importedCount;//exit the method because this file has been imported before.
                            }

                            //TODO: determine the year and quarter for this file
                            #region Filename Year Quarter logic
                            //Get the filename to determine what the year/quarter should be                                                      

                            int index = importFileName.IndexOf(".");
                            if (index > 0)
                                sImportFileString = importFileName.Substring(0, index);

                            string[] sFilenameArray = sImportFileString.Split(" ");

                            //check to see that there are two elements to the array if not, return with error
                            if(sFilenameArray.Length == 2) //all is well
                            {
                                sFileYear = sFilenameArray[0];  //Year is straight forward
                                //determine what the quarter should be
                                switch (sFilenameArray[1])
                                {
                                    case "01":
                                    case "02":
                                    case "03":
                                        sFileQuarter = "01";
                                        break;
                                    case "04":
                                    case "05":
                                    case "06":
                                        sFileQuarter = "02";
                                        break;
                                    case "07":
                                    case "08":
                                    case "09":
                                        sFileQuarter = "03";
                                        break;
                                    case "10":
                                    case "11":
                                    case "12":
                                        sFileQuarter = "04";
                                        break;
                                    
                                }
                                
                            }
                            else  //TODO:  Move this functionality to client side to give feedback before the file is imported so the user can make changes then.
                            {
                                return importedCount;
                            }
                            #endregion

                        }

                        //passed all checks and is ready to import
                        if (row >= 1)
                        { 
                            ASCAP_Earning earning = new ASCAP_Earning();

                            //file has not been imported  process according to the iMappingChoice value
                            switch (iMappingChoice)
                            {
                                //Domestic
                                case 1:
                                    #region Domestic Mapping
                                    earning.Distribution_Year = columns[0].ToString();
                                    earning.Distribution_Quarter = columns[1].ToString();
                                    earning.Statement_Recipient_ID = columns[2].ToString();
                                    earning.Statement_Recipient_Name = columns[3].ToString();
                                    earning.Party_ID = columns[4].ToString();
                                    earning.Party_Name = columns[5].ToString();
                                    earning.Legal_Earner_Party_ID = columns[6].ToString();
                                    earning.Legal_Earner_Party_Name = columns[7].ToString();
                                    earning.Performance_Source = columns[8].ToString();
                                    earning.Music_User_Genre = columns[9].ToString();
                                    earning.Music_User = columns[10].ToString();
                                    earning.Network_Service = columns[11].ToString();
                                    earning.Performance_Start_Date = columns[12].ToString();
                                    earning.Performance_End_Date = columns[13].ToString();
                                    earning.Survey_Type = columns[14].ToString();
                                    earning.Day_Part_Code = columns[15].ToString();
                                    earning.Series_or_Film = columns[16].ToString();
                                    earning.Program_Name = columns[17].ToString();
                                    earning.Work_ID = columns[18].ToString();
                                    earning.Work_Title = columns[19].ToString();
                                    earning.CA_Pct = columns[20].ToString();
                                    earning.Classification_Code = columns[21].ToString();
                                    earning.Number_of_Plays = columns[22].ToString();
                                    earning.Performance_Type = columns[23].ToString();
                                    earning.Duration = columns[24].ToString();
                                    earning.Performing_Artist = columns[25].ToString();
                                    earning.Composer_Name = columns[26].ToString();
                                    earning.EE_Share = columns[27].ToString();
                                    earning.Credits = columns[28].ToString();
                                    earning.Dollar_TOTAL = columns[29].ToString();
                                    earning.Premium_Credits = columns[30].ToString();
                                    earning.Premium_Dollars = columns[31].ToString();
                                    earning.Adjustment_Indicator = columns[32].ToString();
                                    earning.Adjustment_Reason_Code = columns[33].ToString();
                                    earning.Original_Distribution_Date = columns[34].ToString();
                                    earning.Role_Type = columns[35].ToString();
                                    earning.Licensor_Flag = columns[36].ToString();

                                    earning.File_Distribution_Year = sFileYear;
                                    earning.File_Distribution_Quarter = sFileQuarter;
                                    earning.Import_Filename = importFileName;

                                    earnings.Add(earning);
                                    row++; //increment the row and move to the next record

                                    #endregion

                                    break;                                    
                                case 2:  //international old

                                    #region International Old Mapping
                                    //Debug.WriteLine(string.Join(", ", columns));

                                    earning.File_Type = columns[0].ToString();
                                    earning.Statement_Recipient_Name = columns[1].ToString();
                                    earning.Statement_Recipient_ID = columns[2].ToString();
                                    earning.Party_Name = columns[3].ToString();
                                    earning.Party_ID = columns[4].ToString();
                                    earning.Legal_Earner_Party_ID = columns[5].ToString();
                                    earning.Legal_Earner_Party_Name = columns[6].ToString();
                                    earning.Distribution_Date = columns[7].ToString();
                                    earning.Country_Name = columns[8].ToString();
                                    earning.Licensor = columns[9].ToString();
                                    earning.Licensor_Code = columns[10].ToString();
                                    earning.Performance_Start_Date = columns[11].ToString();
                                    earning.Performance_End_Date = columns[12].ToString();
                                    earning.Work_Title = columns[13].ToString();
                                    earning.Work_ID = columns[14].ToString();
                                    earning.Member_Share = columns[15].ToString();
                                    earning.Series_Name = columns[16].ToString();
                                    earning.Program_Name = columns[17].ToString();
                                    earning.Dollar_GENERAL = columns[18].ToString();  //  <-- differences start here with the international files
                                    earning.Dollar_RADIO = columns[19].ToString();
                                    earning.Dollar_TV = columns[20].ToString();
                                    earning.Dollar_TV_FILM = columns[21].ToString();
                                    earning.Dollar_CINEMA = columns[22].ToString();
                                    earning.Dollar_TOTAL = columns[23].ToString();
                                    earning.Adjustment_Distribution_Date = columns[24].ToString();
                                    earning.Adjustment_Indicator = columns[25].ToString();
                                    earning.Role_Type = columns[26].ToString();
                                    earning.Type_Of_Right = columns[27].ToString();
                                    earning.Territory = columns[28].ToString();

                                    earning.File_Distribution_Year = sFileYear;
                                    earning.File_Distribution_Quarter = sFileQuarter;
                                    earning.Import_Filename = importFileName;

                                    earnings.Add(earning);
                                    row++; //increment the row and move to the next record

                                    #endregion

                                    break;


                                case 3:  //international old

                                    #region International New Mapping

                                    //Debug.WriteLine(string.Join(", ", columns));
                                    
                                    earning.File_Type = columns[0].ToString();
                                    earning.Statement_Recipient_Name = columns[1].ToString();
                                    earning.Statement_Recipient_ID = columns[2].ToString();
                                    earning.Party_Name = columns[3].ToString();
                                    earning.Party_ID = columns[4].ToString();
                                    earning.Legal_Earner_Party_ID = columns[5].ToString();
                                    earning.Legal_Earner_Party_Name = columns[6].ToString();
                                    earning.Distribution_Date = columns[7].ToString();
                                    earning.Country_Name = columns[8].ToString();
                                    earning.Licensor = columns[9].ToString();
                                    earning.Licensor_Code = columns[10].ToString();
                                    earning.Performance_Start_Date = columns[11].ToString();
                                    earning.Performance_End_Date = columns[12].ToString();
                                    earning.Work_Title = columns[13].ToString();
                                    earning.Work_ID = columns[14].ToString();
                                    earning.Member_Share = columns[15].ToString();
                                    earning.Series_Name = columns[16].ToString();
                                    earning.Program_Name = columns[17].ToString();

                                    //differences start here with

                                    earning.Revenue_Class_Code = columns[18].ToString();
                                    earning.Revenue_Class_Description = columns[19].ToString();
                                    earning.Dollar_TOTAL = columns[20].ToString();   //$ Amount

                                    earning.Adjustment_Distribution_Date = columns[21].ToString();
                                    earning.Adjustment_Indicator = columns[22].ToString();
                                    earning.Role_Type = columns[23].ToString();
                                    earning.Type_Of_Right = columns[24].ToString();
                                    earning.Territory = columns[25].ToString();

                                    earning.File_Distribution_Year = sFileYear;
                                    earning.File_Distribution_Quarter = sFileQuarter;
                                    earning.Import_Filename = importFileName;

                                    earnings.Add(earning);
                                    row++; //increment the row and move to the next record

                                    #endregion

                                    break;
                                case 4:
                                    #region Domestic - 2 Mapping
                                    earning.Distribution_Year = columns[0].ToString();
                                    earning.Distribution_Quarter = columns[1].ToString();
                                    earning.Statement_Recipient_ID = columns[2].ToString();
                                    earning.Statement_Recipient_Name = columns[3].ToString();
                                    earning.Party_ID = columns[4].ToString();
                                    earning.Party_Name = columns[5].ToString();
                                    earning.Legal_Earner_Party_ID = columns[6].ToString();
                                    earning.Legal_Earner_Party_Name = columns[7].ToString();
                                    earning.Performance_Source = columns[8].ToString();
                                    earning.Music_User_Genre = columns[9].ToString();
                                    earning.Music_User = columns[10].ToString();
                                    earning.Network_Service = columns[11].ToString();
                                    earning.Performance_Start_Date = columns[12].ToString();
                                    earning.Performance_End_Date = columns[13].ToString();
                                    earning.Survey_Type = columns[14].ToString();
                                    earning.Day_Part_Code = columns[15].ToString();
                                    earning.Series_or_Film = columns[16].ToString();
                                    earning.Program_Name = columns[17].ToString();
                                    earning.Work_ID = columns[18].ToString();
                                    earning.Work_Title = columns[19].ToString();
                                    earning.CA_Pct = columns[20].ToString();
                                    earning.Classification_Code = columns[21].ToString();
                                    earning.Number_of_Plays = columns[22].ToString();
                                    earning.Performance_Type = columns[23].ToString();
                                    earning.Duration = columns[24].ToString();
                                    earning.Performing_Artist = columns[25].ToString();
                                    earning.Composer_Name = columns[26].ToString();
                                    earning.EE_Share = columns[27].ToString();
                                    earning.Credits = columns[28].ToString();
                                    earning.Dollar_TOTAL = columns[29].ToString();
                                    earning.Premium_Credits = columns[30].ToString();
                                    earning.Premium_Dollars = columns[31].ToString();
                                    earning.Adjustment_Indicator = columns[32].ToString();
                                    earning.Adjustment_Reason_Code = columns[33].ToString();
                                    earning.Original_Distribution_Date = columns[34].ToString();
                                    earning.Role_Type = columns[35].ToString();
                                    earning.Type_Of_Right = columns[36].ToString();
                                    earning.Territory = columns[37].ToString();
                                    earning.Licensor = columns[38].ToString();

                                    earning.File_Distribution_Year = sFileYear;
                                    earning.File_Distribution_Quarter = sFileQuarter;
                                    earning.Import_Filename = importFileName;

                                    earnings.Add(earning);
                                    row++; //increment the row and move to the next record

                                    #endregion

                                    break;

                            }

                            importedCount = row - 1;  //get the number of rows we are attempting to insert


                        }
                      
                    }//end while read....
                    //insert the list of earnings into the database
                    Data.Services.InsertASCAPEarnings(earnings);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return importedCount;
        }



        // Computes the SHA256 hash of the file.
        public static string ComputeFileHashAsync(Stream fileStream)
        {
           
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(fileStream);
                return Convert.ToBase64String(hash);
            }
        }


        // Dummy implementation for checking if a hash exists.
        // Replace this with your actual logic for checking against stored hashes.
        public static bool HashExists(string fileHash)
        {
            // Implement your logic here to check against existing file hashes
            // For example, querying a database or a collection in memory
            return false; // Replace with actual check
        }

        /// <summary>
        /// Determines which mapping method should be used to correctly map an ASCAP Earnings csv file.
        /// </summary>
        /// <param name="csvHeaderColumnNames">The column header from the csv file in a string array.</param>
        /// <returns>
        /// -1 = Error in method
        /// 0 = no match found
        /// 1 = Domestic
        /// 2 = International Old
        /// 3 = International New
        /// </returns>
        public static int DetermineASCAPFileMappingFromHeader(string[] csvHeaderColumnNames)
        {
            int iMappingType = -1;

            if(csvHeaderColumnNames[20].Replace(" ", String.Empty).ToLower() == "$amount") // 2 = International New
            {
                iMappingType = 3;
            }
            else if(csvHeaderColumnNames[20].Replace(" ", String.Empty).ToLower() == "$tv") // 3 = International Old
            {

                iMappingType = 2;
            }
            else if (csvHeaderColumnNames[36].Replace(" ", String.Empty).ToLower() == "licensorflag") // 1 = Domestic
            {
                iMappingType = 1;
            }
            else if (csvHeaderColumnNames[36].Replace(" ", String.Empty).ToLower() == "typeofright") // 4 = Domestic #2
            {
                iMappingType = 4;
            }

            return iMappingType;

        }



        public static void CreateCSV(this DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers  
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }




    }
}
