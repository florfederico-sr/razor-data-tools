using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings;
using System.Text.RegularExpressions;
using razor_data_tools.Models;




namespace razor_data_tools.Utils
{
    public static class General
    {

        public static FileProperty FactoryFileProperties(string sFileName, int iRecordCount)
        {
            FileProperty f = new FileProperty();

            f.FileName = sFileName;
            f.RecordCount = iRecordCount;

            return f;            
        }


        public static string CreateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }


        public static string CreateMD5HashII(string input)
        {
            var estEncoding = Encoding.GetEncoding(1252);

            // Creates an instance of the default implementation of the MD5 hash algorithm.
            using (var md5Hash = MD5.Create())
            {
                //have to add System.Text.Encodings.CodePages via nuGet for encoding 1252 to work.
                // Byte array representation of source string
                var sourceBytes = System.Text.Encoding.GetEncoding(1252).GetBytes(input);
                

                // Generate hash value(Byte Array) for input data
                var hashBytes = md5Hash.ComputeHash(sourceBytes);

                // Convert hash byte array to string
                 var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                // Output the MD5 hash
                Console.WriteLine("The MD5 hash of " + input + " is: " + hash);

                return hash;
            }

            


        }

        /// <summary>
        /// Takes a list of BMI_Earning objects and creates a comma delimited string that can be converted to a csv file.
        /// </summary>
        /// <param name="earnings">List of BMI_Earning objects.</param>
        /// <returns>delimited string that can be converted to a csv file</returns>
        public static string ConvertBMIEarningToCsvString(List<BMI_Earning> earnings)
        {
            string retVal = string.Empty;

            IEnumerable<string> personTexts = earnings
                                                .Select(e => String.Join(",",
                                                                            e.PERIOD,
                                                                            e.W_OR_P,
                                                                            e.PARTICIPANT_NAME,
                                                                            e.PARTICIPANT_NUM,
                                                                            e.IP_NUM,
                                                                            e.TITLE_NAME,
                                                                            e.TITLE_NUM,
                                                                            e.PERF_SOURCE,
                                                                            e.COUNTRY_OF_PERFORMANCE,
                                                                            e.SHOW_NAME,
                                                                            e.EPISODE_NAME,
                                                                            e.SHOW_NUM,
                                                                            e.USE_CODE,
                                                                            e.TIMING,
                                                                            e.PARTICIPANT_PCT,
                                                                            e.PERF_COUNT,
                                                                            e.BONUS_LEVEL,
                                                                            e.ROYALTY_AMOUNT,
                                                                            e.WITHHOLD,
                                                                            e.PERF_PERIOD,
                                                                            e.CURRENT_ACTIVITY_AMT,
                                                                            e.HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS,
                                                                            e.STANDARDS_OR_TV_NET_THEME_BONUS,
                                                                            e.FOREIGN_SOCIETY_ADJUSTMENT,
                                                                            e.COMPANY_CODE,
                                                                            e.COMPANY_NAME));

            retVal = String.Join(Environment.NewLine, personTexts);
            return retVal;



        }


        public static List<BMI_Earnings_Customer> ConvertBMICustomerToList(DataTable dt)
        {
            var convertedList = (from row in dt.AsEnumerable()
                                 select new BMI_Earnings_Customer()
                                 {
                                     PARTICIPANT_NAME = Convert.ToString(row["PARTICIPANT_NAME"]),
                                     PARTICIPANT_NUM = Convert.ToString(row["PARTICIPANT_NUM"])
                                 }).ToList();
            return convertedList;
        }

        public static List<ASCAP_Earnings_Customer> ConvertASCAPCustomerToList(DataTable dt)
        {
            var convertedList = (from row in dt.AsEnumerable()
                                 select new ASCAP_Earnings_Customer()
                                 {
                                     Party_Name = Convert.ToString(row["Party_Name"]),
                                     Party_Id = Convert.ToString(row["Party_Id"])
                                 }).ToList();
            return convertedList;
        }

        public static List<Tunecore_Earnings_Customer> ConvertTunecoreCustomerToList(DataTable dt)
        {
            var convertedList = (from row in dt.AsEnumerable()
                                 select new Tunecore_Earnings_Customer()
                                 {
                                     Artist = Convert.ToString(row["Artist"])
                                    // ,SR_Tunecore_Artist_ID = Convert.ToString(row["SR_Tunecore_Artist_ID"])
                                 }).ToList();
            return convertedList;
        }


        public static List<Artist> ConvertArtistDataTableToList(DataTable dt)
        {
            var convertedList = (from row in dt.AsEnumerable()
                                 select new Artist()
                                 {
                                     Artist_Id = Convert.ToInt32(row["Artist_id"]),
                                     Artist_Name = Convert.ToString(row["ARTIST_NAME_FILE"])
                                     // ,SR_Tunecore_Artist_ID = Convert.ToString(row["SR_Tunecore_Artist_ID"])
                                 }).ToList();
            return convertedList;
        }

        /// <summary>
        /// Converts a DataTable with BMI Earnings to a list of BMI_Earning objects
        /// </summary>
        /// <param name="dt">Data from staging.BMI_Earnings in a DataTable. </param>
        /// <returns></returns>
        public static List<BMI_Earning> ConvertBMIDataTableToList(DataTable dt)
        {

            var convertedList = (from rw in dt.AsEnumerable()
                                 select new BMI_Earning()
                                 {
                                     PERIOD = Convert.ToString(rw[0]),
                                     W_OR_P = Convert.ToString(rw[1]),
                                     PARTICIPANT_NAME = Convert.ToString(rw[2]),
                                     PARTICIPANT_NUM = Convert.ToString(rw[3]),
                                     IP_NUM = Convert.ToString(rw[4]),
                                     TITLE_NAME = Convert.ToString(rw[5]),
                                     TITLE_NUM = Convert.ToString(rw[6]),
                                     PERF_SOURCE = Convert.ToString(rw[7]),
                                     COUNTRY_OF_PERFORMANCE = Convert.ToString(rw[8]),
                                     SHOW_NAME = Convert.ToString(rw[9]),
                                     EPISODE_NAME = Convert.ToString(rw[10]),
                                     SHOW_NUM = Convert.ToString(rw[11]),
                                     USE_CODE = Convert.ToString(rw[12]),
                                     TIMING = Convert.ToString(rw[13]),
                                     PARTICIPANT_PCT = Convert.ToString(rw[14]),
                                     PERF_COUNT = Convert.ToString(rw[15]),
                                     BONUS_LEVEL = Convert.ToString(rw[16]),
                                     ROYALTY_AMOUNT = Convert.ToString(rw[17]),
                                     WITHHOLD = Convert.ToString(rw[18]),
                                     PERF_PERIOD = Convert.ToString(rw[19]),
                                     CURRENT_ACTIVITY_AMT = Convert.ToString(rw[20]),
                                     HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS = Convert.ToString(rw[21]),
                                     STANDARDS_OR_TV_NET_THEME_BONUS = Convert.ToString(rw[22]),
                                     FOREIGN_SOCIETY_ADJUSTMENT = Convert.ToString(rw[23]),
                                     COMPANY_CODE = Convert.ToString(rw[24]),
                                     COMPANY_NAME = Convert.ToString(rw[25])
                                 }).ToList();
            
            return convertedList;
        }





        public static List<ASCAP_Earning> ConvertASCAPDataTableToList(DataTable dt)
        {

            var convertedList = (from rw in dt.AsEnumerable()
                                 select new ASCAP_Earning()
                                 {
                                     File_Type = Convert.ToString(rw[0]),
                                     Statement_Recipient_Name = Convert.ToString(rw[1]),
                                     Statement_Recipient_ID = Convert.ToString(rw[2]),
                                     Party_Name = Convert.ToString(rw[3]),
                                     Party_ID = Convert.ToString(rw[4]),
                                     Legal_Earner_Party_ID = Convert.ToString(rw[5]),
                                     Legal_Earner_Party_Name = Convert.ToString(rw[6]),
                                     Distribution_Date = Convert.ToString(rw[7]),
                                     Distribution_Year = Convert.ToString(rw[8]),
                                     Distribution_Quarter = Convert.ToString(rw[9]),
                                     File_Distribution_Year = Convert.ToString(rw[10]),
                                     File_Distribution_Quarter = Convert.ToString(rw[11]),
                                     Country_Name = Convert.ToString(rw[12]),
                                     Licensor = Convert.ToString(rw[13]),
                                     Licensor_Code = Convert.ToString(rw[14]),
                                     Performance_Start_Date = Convert.ToString(rw[15]),
                                     Performance_End_Date = Convert.ToString(rw[16]),
                                     Duration = Convert.ToString(rw[17]),
                                     Number_of_Plays = Convert.ToString(rw[18]),
                                     Work_Title = Convert.ToString(rw[19]),
                                     Work_ID = Convert.ToString(rw[20]),
                                     Member_Share = Convert.ToString(rw[21]),
                                     Series_Name = Convert.ToString(rw[22]),
                                     Program_Name = Convert.ToString(rw[23]),
                                     Dollar_GENERAL = Convert.ToString(rw[24]),
                                     Dollar_RADIO = Convert.ToString(rw[25]),
                                     Dollar_TV = Convert.ToString(rw[26]),
                                     Dollar_TV_FILM = Convert.ToString(rw[27]),
                                     Dollar_CINEMA = Convert.ToString(rw[28]),
                                     Dollar_TOTAL = Convert.ToString(rw[29]),
                                     Revenue_Class_Code = Convert.ToString(rw[30]),
                                     Revenue_Class_Description = Convert.ToString(rw[31]),
                                     Adjustment_Distribution_Date = Convert.ToString(rw[32]),
                                     Adjustment_Indicator = Convert.ToString(rw[33]),
                                     Adjustment_Reason_Code = Convert.ToString(rw[34]),
                                     Role_Type = Convert.ToString(rw[35]),
                                     Type_Of_Right = Convert.ToString(rw[36]),
                                     Territory = Convert.ToString(rw[37]),
                                     Performance_Source = Convert.ToString(rw[38]),
                                     Music_User = Convert.ToString(rw[39]),
                                     Network_Service = Convert.ToString(rw[40]),
                                     Survey_Type = Convert.ToString(rw[41]),
                                     Day_Part_Code = Convert.ToString(rw[42]),
                                     CA_Pct = Convert.ToString(rw[43]),
                                     Classification_Code = Convert.ToString(rw[44]),
                                     Performance_Type = Convert.ToString(rw[45]),
                                     Performing_Artist = Convert.ToString(rw[46]),
                                     Composer_Name = Convert.ToString(rw[47]),
                                     EE_Share = Convert.ToString(rw[48]),
                                     Credits = Convert.ToString(rw[49]),
                                     Premium_Credits = Convert.ToString(rw[50]),
                                     Premium_Dollars = Convert.ToString(rw[51]),
                                     Original_Distribution_Date = Convert.ToString(rw[52])

                                 }).ToList();

            return convertedList;
        }




        /// <summary>
        /// Converts a DataTable with BMI Earnings to a list of BMI_Earning objects
        /// </summary>
        /// <param name="dt">Data from staging.BMI_Earnings in a DataTable. </param>
        /// <returns></returns>
        public static List<Tunecore_Earning> ConvertTunecoreDataTableToList(DataTable dt)
        {

            var convertedList = (from rw in dt.AsEnumerable()
                                 select new Tunecore_Earning()
                                 {
                                     Sales_Period = Convert.ToString(rw[0]),
                                     Posted_Date = Convert.ToString(rw[1]),
                                     Store_Name = Convert.ToString(rw[2]),
                                     Country_Of_Sale = Convert.ToString(rw[3]),
                                     Artist = Convert.ToString(rw[4]),
                                     Release_Type = Convert.ToString(rw[5]),
                                     Release_Title = Convert.ToString(rw[6]),
                                     Song_Title = Convert.ToString(rw[7]),
                                     Label = Convert.ToString(rw[8]),
                                     UPC = Convert.ToString(rw[9]),
                                     Optional_UPC = Convert.ToString(rw[10]),
                                     TC_Song_ID = Convert.ToString(rw[11]),
                                     Optional_ISRC = Convert.ToString(rw[12]),
                                     Sales_Type = Convert.ToString(rw[13]),
                                     Num_Units_Sold = Convert.ToString(rw[14]),
                                     Per_Unit_Price = Convert.ToString(rw[15]),
                                     Net_Sales = Convert.ToString(rw[16]),
                                     Net_Sales_Currency = Convert.ToString(rw[17]),
                                     Exchange_Rate = Convert.ToString(rw[18]),
                                     Total_Earned = Convert.ToString(rw[19]),
                                     Currency = Convert.ToString(rw[20]),
                                     IMPORT_FILENAME = Convert.ToString(rw[21])
                                 }).ToList();

            return convertedList;
        }









        public static void CodeThatIFound(string filePath, DataTable dt, string delimiter)
        {


            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (StreamWriter swr =
                     new StreamWriter(File.Open(filePath, FileMode.CreateNew), Encoding.Default, 1000000))
            // change buffer size and Encoding to your needs
            {
                foreach (DataRow dr in dt.Rows)
                {
                    swr.WriteLine(string.Join(delimiter, dr.ItemArray));
                }
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);








        }

        public static string RemovePunctuation(string input)
        {
            return Regex.Replace(input, @"[\p{P}\p{S}]", "");
        }



    }
}
