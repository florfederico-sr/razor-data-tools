using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using razor_data_tools.Models;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace razor_data_tools.Data
{
    public class Services
    {
        //TODO:  Fix the database connection to be environment based
        /// <summary>
        /// Dev database connection
        /// </summary>
        /// <returns></returns>
        public static SqlConnectionStringBuilder CorpusDataConnection()
        {
            //Production - yes I know it says dev in the server namne...  Need to rename it
            // SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            // {
            //     DataSource = "sr-azr-dev-sqldb-01.database.windows.net",
            //     UserID = "sqldbadmin",
            //     Password = "b!.H9Vx7d.n",
            //     InitialCatalog = "Corpus"
            // };

            //Test
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "sr-azr-test-sqldb-01.database.windows.net",
                UserID = "sqldbadmin",
                Password = "8.QaPHQ6oTZh6",
                InitialCatalog = "Corpus"
            };


            //Prod II (Real Production
            //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            //{
            //    DataSource = "sr-azr-prod-sqldb-01.database.windows.net",
            //    UserID = "sqladmin",
            //    Password = "damper-seen-botany-FUDDLE-alder-knight",
            //    InitialCatalog = "Corpus"
            //};

            //sr-azr-prod-sqldb-01.database.windows.net

            return builder;
        }


        public static async Task<List<string>> GetPayorsFromDatabaseAsync()
        {
            var payors = new List<string>();
            SqlConnectionStringBuilder builder = CorpusDataConnection();

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT DISTINCT PAYOR_NAME FROM [dbo].[Payor_Earnings] ORDER BY PAYOR_NAME"; // Adjust to your actual table and column names

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            payors.Add(reader.GetString(0)); // Assumes "PayorName" is in the first column
                        }
                    }
                }
            }

            return payors;
        }
        
        
        private List<string> GetArtistsFromDatabase()
        {
            var artists = new List<string>();
            string connectionString = Data.Services.CorpusDataConnection().ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT DISTINCT a.[ARTIST_NAME_FILE] as ARTIST ,ape.[ARTIST_ID] FROM [dbo].[ArtistPayor_Earnings] ape INNER JOIN [dbo].[Artist] a ON ape.[ARTIST_ID] = a.[ARTIST_ID]  GROUP BY a.[ARTIST_NAME_FILE] ,ape.[ARTIST_ID] ORDER BY a.[ARTIST_NAME_FILE]"; // Replace 'YourTable' with your actual table name

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            artists.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return artists;
        }
        
        
        public static bool SelectFileHashForDupeCheck(string sTableName, string sFileHash)
        {
            bool retVal = false;
            int recordCount = 0;
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT count(*) FROM [staging].[" + sTableName + "] ");
                    sb.Append(" WHERE FILEHASH = @FILEHASH ");

                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        SqlParameter pFileHash = new SqlParameter("@FILEHASH", SqlDbType.NVarChar);
                        pFileHash.Value = sFileHash;
                        command.Parameters.Add(pFileHash);

                        try
                        {
                            var i = command.ExecuteScalar();
                            if (i != null)
                                int.TryParse(i.ToString(), out recordCount);
                            if (recordCount > 0)
                                retVal = true;

                        }
                        catch (SqlException sx)
                        {
                            Console.WriteLine(sx.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return retVal;
        }






        public static int InsertArtist(string sArtistName)
        {
            SqlConnectionStringBuilder builder = CorpusDataConnection();

            int retVal = -1;

            try
            {
                StringBuilder sbCommand = new StringBuilder();
                sbCommand.Append(" INSERT INTO [dbo].[Artist] ");
                sbCommand.Append(" ([ARTIST_NAME_FILE]) ");
                sbCommand.Append(" VALUES ");
                sbCommand.Append(" (@ARTIST_NAME_FILE); ");
                sbCommand.Append(" SELECT SCOPE_IDENTITY(); "); //return the artist id

                using (SqlConnection conn = new SqlConnection(builder.ToString()))
                {
                    conn.Open();
                    
                    SqlCommand cmd = new SqlCommand(sbCommand.ToString(), conn);

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.Add("@ARTIST_NAME_FILE", SqlDbType.NVarChar, 200);

                    cmd.Parameters["@ARTIST_NAME_FILE"].Value = sArtistName;

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        retVal = Convert.ToInt32(result); // Convert the result to int and assign it to retVal
                    }

                    conn.Close();

                }
            }
            catch (SqlException sqlEx)
            {
                Debug.WriteLine(sqlEx.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return retVal;
        }





        public static DataTable SelectAllArtists()
        {

            DataTable table = new DataTable();
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT [Artist_Id], [ARTIST_NAME_FILE] ");                   
                    sb.Append(" FROM [dbo].[Artist] ");
                    sb.Append(" ORDER BY [ARTIST_NAME_FILE] ");
         

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            try
                            {
                                adapter.Fill(table);
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return table;
        }




        /// <summary>
        /// Gets all of the rows from the BMI_Earnings table.
        /// TODO: Need to setup a filter for this by person and/or time range?
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAllBMIEarnings()
        {

            DataTable table = new DataTable();
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT PERIOD, W_OR_P, PARTICIPANT_NAME, PARTICIPANT_NUM, IP_NUM, TITLE_NAME, TITLE_NUM, PERF_SOURCE, COUNTRY_OF_PERFORMANCE, SHOW_NAME, EPISODE_NAME, SHOW_NUM, USE_CODE, TIMING, PARTICIPANT_PCT, PERF_COUNT, BONUS_LEVEL, ROYALTY_AMOUNT, WITHHOLD, PERF_PERIOD, CURRENT_ACTIVITY_AMT, HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS, STANDARDS_OR_TV_NET_THEME_BONUS, FOREIGN_SOCIETY_ADJUSTMENT, COMPANY_CODE, COMPANY_NAME ");
                    sb.Append("	FROM [staging].[BMI_Earnings] ");
                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        //SqlParameter pStatusTypesId = new SqlParameter("@StatusTypesId", SqlDbType.Int);
                        //pStatusTypesId.Value = iStatusTypesId;
                        //command.Parameters.Add(pStatusTypesId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            try
                            {
                                adapter.Fill(table);
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
           
            return table;
        }

        /// <summary>
        /// Gets all of the rows from the BMI_Earnings table.
        /// TODO: Need to setup a filter for this by person and/or time range?
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAllBMIEarningsParticipantNameAndNumber()
        {

            DataTable table = new DataTable();
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT PARTICIPANT_NAME, PARTICIPANT_NUM ");
                    sb.Append("	FROM [staging].[BMI_Earnings] ");
                    sb.Append("	GROUP BY  PARTICIPANT_NAME, PARTICIPANT_NUM ");
                    sb.Append("	ORDER BY PARTICIPANT_NAME ");
                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        //SqlParameter pStatusTypesId = new SqlParameter("@StatusTypesId", SqlDbType.Int);
                        //pStatusTypesId.Value = iStatusTypesId;
                        //command.Parameters.Add(pStatusTypesId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            try
                            {
                                adapter.Fill(table);
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return table;
        }


        /// <summary>
        /// Gets all of the rows from the Artist_Earnings table.
        /// TODO: Need to setup a filter for this by person and/or time range?
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAllTunecoreEarningsParticipantNameAndNumber()
        {

            DataTable table = new DataTable();
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT Artist ");
                    sb.Append("	FROM [staging].[Tunecore_Earnings] ");
                    sb.Append("	GROUP BY  Artist ");
                    sb.Append("	ORDER BY Artist ");
                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        //SqlParameter pStatusTypesId = new SqlParameter("@StatusTypesId", SqlDbType.Int);
                        //pStatusTypesId.Value = iStatusTypesId;
                        //command.Parameters.Add(pStatusTypesId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            try
                            {
                                adapter.Fill(table);
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return table;
        }




       

       


        // <summary>
        /// Gets all of the rows from the BMI_Earnings table.
        /// TODO: Need to setup a filter for this by person and/or time range?
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectBMIEarnings(string participant_num)
        {

            DataTable table = new DataTable();
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT PERIOD, W_OR_P, PARTICIPANT_NAME, PARTICIPANT_NUM, IP_NUM, TITLE_NAME, TITLE_NUM, PERF_SOURCE, COUNTRY_OF_PERFORMANCE, SHOW_NAME, EPISODE_NAME, SHOW_NUM, USE_CODE, TIMING, PARTICIPANT_PCT, PERF_COUNT, BONUS_LEVEL, ROYALTY_AMOUNT, WITHHOLD, PERF_PERIOD, CURRENT_ACTIVITY_AMT, HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS, STANDARDS_OR_TV_NET_THEME_BONUS, FOREIGN_SOCIETY_ADJUSTMENT, COMPANY_CODE, COMPANY_NAME ");
                    sb.Append("	FROM [staging].[BMI_Earnings] ");
                    sb.Append("	WHERE TRIM(PARTICIPANT_NUM) = @participant_num");

                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        SqlParameter pParticipantNum = new SqlParameter("@participant_num", SqlDbType.NVarChar);
                        pParticipantNum.Value = participant_num;
                        command.Parameters.Add(pParticipantNum);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            try
                            {
                                adapter.Fill(table);
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return table;
        }


        // <summary>
        /// Gets all of the rows from the Tunecore_Earnings table.
        /// TODO: Need to setup a filter for this by person and/or time range?
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectTunecoreEarnings(string sArtist)
        {

            DataTable table = new DataTable();
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT [Sales_Period]  ,[Posted_Date] ,[Store_Name] ,[Country_Of_Sale] ,[Artist] ,[Release_Type] ,[Release_Title] ,[Song_Title] ,[Label] ");
                    sb.Append(" ,[UPC] ,[Optional_UPC] ,[TC_Song_ID] ,[Optional_ISRC] ,[Sales_Type] ,[Num_Units_Sold] ,[Per_Unit_Price] ,[Net_Sales],[Net_Sales_Currency] ");
                    sb.Append(" ,[Exchange_Rate],[Total_Earned] ,[Currency] ,[IMPORT_FILENAME] ");
                    sb.Append(" FROM [staging].[Tunecore_Earnings] ");
                    sb.Append(" WHERE Artist = @Artist ");


                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        SqlParameter pArtist = new SqlParameter("@Artist", SqlDbType.NVarChar);
                        pArtist.Value = sArtist;
                        command.Parameters.Add(pArtist);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            try
                            {
                                adapter.Fill(table);
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return table;
        }






        /// <summary>
        /// Method to be used as a count of records imported from a file and a dupe check for BMI Earnings Statements
        /// </summary>
        /// <param name="participant_num">The BMI Participant Number</param>
        /// <param name="importFileName">The name of the file was/being imported</param>
        /// <returns>The count of records imported</returns>
        public static int SelectCountOfBMIEarningsImportedFromFile(string participant_num, string importFileName)
        {
            int recordCount = 0;
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT  COUNT(*) FROM [staging].[BMI_Earnings] ");
                    sb.Append("	WHERE PARTICIPANT_NUM = @PARTICIPANT_NUM  AND IMPORT_FILENAME = @IMPORT_FILENAME ");                    

                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        SqlParameter pParticipantNum = new SqlParameter("@PARTICIPANT_NUM", SqlDbType.NVarChar);
                        pParticipantNum.Value = participant_num;
                        command.Parameters.Add(pParticipantNum);

                        SqlParameter pImportFileName = new SqlParameter("@IMPORT_FILENAME", SqlDbType.NVarChar);
                        pImportFileName.Value = importFileName;
                        command.Parameters.Add(pImportFileName);


                        try
                        {   
                            var i = command.ExecuteScalar();  
                            if (i != null)
                                int.TryParse(i.ToString(), out recordCount);


                        }
                        catch (SqlException sx)
                        {
                            Console.WriteLine(sx.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return recordCount;
        }

        /// <summary>
        /// Method to be used as a count of records imported from a file and a dupe check for Tunecore Earnings.
        /// </summary>
        /// <param name="artist">The Tunecore Artist</param>
        /// <param name="importFileName">Name of the file being imported</param>
        /// <returns>Count of records imported</returns>
        public static int SelectCountOfTuneoreEarningsImportedFromFile(string artist, string importFileName)
        {
            int recordCount = 0;
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT  COUNT(*) FROM [staging].[Tunecore_Earnings] ");
                    sb.Append("	WHERE Artist = @artist  AND IMPORT_FILENAME = @IMPORT_FILENAME ");

                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        SqlParameter pArtist = new SqlParameter("@artist", SqlDbType.NVarChar);
                        pArtist.Value = artist;
                        command.Parameters.Add(pArtist);

                        SqlParameter pImportFileName = new SqlParameter("@IMPORT_FILENAME", SqlDbType.NVarChar);
                        pImportFileName.Value = importFileName;
                        command.Parameters.Add(pImportFileName);


                        try
                        {
                            var i = command.ExecuteScalar();
                            if (i != null)
                                int.TryParse(i.ToString(), out recordCount);


                        }
                        catch (SqlException sx)
                        {
                            Console.WriteLine(sx.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return recordCount;
        }



        public static bool InsertASCAPWorks(List<ASCAP_Work> ascap_works, string importFileName)
        {
            SqlConnectionStringBuilder builder = CorpusDataConnection();

            bool retVal = false;

            //int commitThreshold = 100;
            int recordCount = 0;           

            try
            {
                StringBuilder sbCommand = new StringBuilder();
                sbCommand.Append(" INSERT INTO [staging].[ASCAP_Works] ");
                sbCommand.Append(" ([Party_ID_1], [Party_ID_2], [Party_ID], [Total_Number_Of_Works], [Member_Name], [Work_Title], [ASCAP_Work_ID], [Interested_Parties], [IPI_Number], [Interested_Party_Status], [PartyRole], [Society], [OwnershipPct], [CollectPct], [Registration_Date], [Registration_Status], [Surveyed_Work], [ISWC_Number],  [IMPORT_FILENAME]) ");
                sbCommand.Append(" VALUES ");
                sbCommand.Append(" (@Party_ID_1, @Party_ID_2, @Party_ID, @Total_Number_Of_Works, @Member_Name, @Work_Title, @ASCAP_Work_ID, @Interested_Parties, @IPI_Number, @Interested_Party_Status, @PartyRole, @Society, @OwnershipPct, @CollectPct, @Registration_Date, @Registration_Status, @Surveyed_Work, @ISWC_Number, @IMPORT_FILENAME); ");
                //sbCommand.Append(" SELECT SCOPE_IDENTITY() ");
                using (SqlConnection conn = new SqlConnection(builder.ToString()))
                {

                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand(sbCommand.ToString(), conn, trans);

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.Add("@Party_ID_1", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Party_ID_2", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Party_ID", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Total_Number_Of_Works", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Member_Name", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Work_Title", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@ASCAP_Work_ID", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Interested_Parties", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@IPI_Number", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Interested_Party_Status", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@PartyRole", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Society", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@OwnershipPct", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@CollectPct", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Registration_Date", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Registration_Status", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Surveyed_Work", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@ISWC_Number", SqlDbType.NVarChar, 500);

                    cmd.Parameters.Add("@IMPORT_FILENAME", SqlDbType.NVarChar, 500);

                    foreach (ASCAP_Work work in ascap_works)
                    {
                        try
                        {                       
                            cmd.Parameters["@Party_ID_1"].Value = work.Party_ID_1;
                            cmd.Parameters["@Party_ID_2"].Value = work.Party_ID_2;
                            cmd.Parameters["@Party_ID"].Value = work.Party_ID;
                            cmd.Parameters["@Total_Number_Of_Works"].Value = work.Total_Number_Of_Works;
                            cmd.Parameters["@Member_Name"].Value = work.Member_Name;
                            cmd.Parameters["@Work_Title"].Value = work.Work_Title;
                            cmd.Parameters["@ASCAP_Work_ID"].Value = work.ASCAP_Work_ID;
                            cmd.Parameters["@Interested_Parties"].Value = work.Interested_Parties;
                            cmd.Parameters["@IPI_Number"].Value = work.IPI_Number;
                            cmd.Parameters["@Interested_Party_Status"].Value = work.Interested_Party_Status;
                            cmd.Parameters["@PartyRole"].Value = work.PartyRole;
                            cmd.Parameters["@Society"].Value = work.Society;
                            cmd.Parameters["@OwnershipPct"].Value = work.OwnershipPct;
                            cmd.Parameters["@CollectPct"].Value = work.CollectPct;
                            cmd.Parameters["@Registration_Date"].Value = work.Registration_Date;
                            cmd.Parameters["@Registration_Status"].Value = work.Registration_Status;
                            cmd.Parameters["@Surveyed_Work"].Value = work.Surveyed_Work;
                            cmd.Parameters["@ISWC_Number"].Value = work.ISWC_Number;
                            cmd.Parameters["@IMPORT_FILENAME"].Value = importFileName;

                            cmd.ExecuteNonQuery();
                            recordCount++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                    }
                    //last commit to ensure that there are no abandonded records.
                    trans.Commit();
                    conn.Close();

                }
            }
            catch (SqlException sqlEx)
            {
                Debug.WriteLine(sqlEx.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return retVal;
        }



        public static DataTable SelectAllASCAPWorksForDupeCheck()
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT count(*) FROM [staging].[ASCAP_Works] ");
                    sb.Append(" WHERE HASHBYTES('MD5', CONCAT(TRIM([Party_ID_1]) ,TRIM([Party_ID_2]) ,TRIM([Party_ID]) ,TRIM([Total_Number_Of_Works]) ,TRIM([Member_Name]) ,TRIM([Work_Title]) ,TRIM([ASCAP_Work_ID]) ,TRIM([Interested_Parties]) ,TRIM([IPI_Number]) ,TRIM([Interested_Party_Status]) ,TRIM([PartyRole]) ,TRIM([Society]) ,TRIM([OwnershipPct]) ,TRIM([CollectPct]) ,TRIM([Registration_Date]) ,TRIM([Registration_Status]) ,TRIM([Surveyed_Work]) ,TRIM([ISWC_Number]), TRIM([IMPORT_FILENAME]))) =   @MD5hashedRow");

                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        //SqlParameter pMD5hashedRow = new SqlParameter("@MD5hashedRow", SqlDbType.NVarChar);
                        //pMD5hashedRow.Value = sMD5hashedRow;
                        //command.Parameters.Add(pMD5hashedRow);


                        try
                        {
                            //var i = command.ExecuteScalar();
                            //if (i != null)
                            //    int.TryParse(i.ToString(), out recordCount);


                        }
                        catch (SqlException sx)
                        {
                            Console.WriteLine(sx.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return dt;
        }






        public static int SelectCountOfASCAPWorksImportedFromFile(string sMD5hashedRow)
        {
            int recordCount = 0;

            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT count(*) FROM [staging].[ASCAP_Works] ");
                    sb.Append(" WHERE HASHBYTES('MD5', CONCAT(TRIM([Party_ID_1]) ,TRIM([Party_ID_2]) ,TRIM([Party_ID]) ,TRIM([Total_Number_Of_Works]) ,TRIM([Member_Name]) ,TRIM([Work_Title]) ,TRIM([ASCAP_Work_ID]) ,TRIM([Interested_Parties]) ,TRIM([IPI_Number]) ,TRIM([Interested_Party_Status]) ,TRIM([PartyRole]) ,TRIM([Society]) ,TRIM([OwnershipPct]) ,TRIM([CollectPct]) ,TRIM([Registration_Date]) ,TRIM([Registration_Status]) ,TRIM([Surveyed_Work]) ,TRIM([ISWC_Number]), TRIM([IMPORT_FILENAME]))) =   @MD5hashedRow");

                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        SqlParameter pMD5hashedRow = new SqlParameter("@MD5hashedRow", SqlDbType.NVarChar);
                        pMD5hashedRow.Value = sMD5hashedRow;
                        command.Parameters.Add(pMD5hashedRow);                        


                        try
                        {
                            var i = command.ExecuteScalar();
                            if (i != null)
                                int.TryParse(i.ToString(), out recordCount);


                        }
                        catch (SqlException sx)
                        {
                            Console.WriteLine(sx.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return recordCount;
        }



        public static DataTable SelectAllASCAPWorksParticipantNameAndNumber()
        {

            DataTable table = new DataTable();
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT Party_Id, Member_Name ");
                    sb.Append(" FROM [staging].[ASCAP_Works]  ");
                    sb.Append(" GROUP BY  Party_Id, Member_Name ");
                    sb.Append(" ORDER BY Member_Name  ");
                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        //SqlParameter pStatusTypesId = new SqlParameter("@StatusTypesId", SqlDbType.Int);
                        //pStatusTypesId.Value = iStatusTypesId;
                        //command.Parameters.Add(pStatusTypesId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            try
                            {
                                adapter.Fill(table);
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return table;
        }




        #region  BMI

        /// <summary>
        /// Inserts a List of BMI_Earning(s) into the database
        /// </summary>
        /// <param name="earnings">A Templated list of Models.BMI_Earning</param>
        /// <returns>bool true if inserted, false if not</returns>
        public static bool InsertBMIEarnings(List<BMI_Earning> earnings, string importFileName, string sFileHash)
        {
            SqlConnectionStringBuilder builder = CorpusDataConnection();

            bool retVal = false;

            int commitThreshold = 100;
            int recordCount = 0;


            try
            {
                StringBuilder sbCommand = new StringBuilder();
                sbCommand.Append(" INSERT INTO [staging].[BMI_Earnings] ");
                sbCommand.Append(" (IMPORT_FILENAME, PERIOD, W_OR_P, PARTICIPANT_NAME, PARTICIPANT_NUM, IP_NUM, TITLE_NAME, TITLE_NUM, PERF_SOURCE, COUNTRY_OF_PERFORMANCE, SHOW_NAME, EPISODE_NAME, SHOW_NUM, USE_CODE, TIMING, PARTICIPANT_PCT, PERF_COUNT, BONUS_LEVEL, ROYALTY_AMOUNT, WITHHOLD, PERF_PERIOD, CURRENT_ACTIVITY_AMT, HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS, STANDARDS_OR_TV_NET_THEME_BONUS, FOREIGN_SOCIETY_ADJUSTMENT, COMPANY_CODE, COMPANY_NAME, FILEHASH) ");
                sbCommand.Append(" VALUES ");
                sbCommand.Append(" (@IMPORT_FILENAME, @PERIOD, @W_OR_P, @PARTICIPANT_NAME, @PARTICIPANT_NUM, @IP_NUM, @TITLE_NAME, @TITLE_NUM, @PERF_SOURCE, @COUNTRY_OF_PERFORMANCE, @SHOW_NAME, @EPISODE_NAME, @SHOW_NUM, @USE_CODE, @TIMING, @PARTICIPANT_PCT, @PERF_COUNT, @BONUS_LEVEL, @ROYALTY_AMOUNT, @WITHHOLD, @PERF_PERIOD, @CURRENT_ACTIVITY_AMT, @HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS, @STANDARDS_OR_TV_NET_THEME_BONUS, @FOREIGN_SOCIETY_ADJUSTMENT, @COMPANY_CODE, @COMPANY_NAME, @FILEHASH); ");
                //sbCommand.Append(" SELECT SCOPE_IDENTITY() ");
                using (SqlConnection conn = new SqlConnection(builder.ToString()))
                {

                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand(sbCommand.ToString(), conn, trans);

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;
                    cmd.Parameters.Add("@IMPORT_FILENAME", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@PERIOD", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@W_OR_P", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@PARTICIPANT_NAME", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@PARTICIPANT_NUM", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@IP_NUM", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@TITLE_NAME", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@TITLE_NUM", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@PERF_SOURCE", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@COUNTRY_OF_PERFORMANCE", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@SHOW_NAME", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@EPISODE_NAME", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@SHOW_NUM", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@USE_CODE", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@TIMING", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@PARTICIPANT_PCT", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@PERF_COUNT", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@BONUS_LEVEL", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@ROYALTY_AMOUNT", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@WITHHOLD", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@PERF_PERIOD", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@CURRENT_ACTIVITY_AMT", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@STANDARDS_OR_TV_NET_THEME_BONUS", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@FOREIGN_SOCIETY_ADJUSTMENT", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@COMPANY_CODE", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@COMPANY_NAME", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@FILEHASH", SqlDbType.NVarChar, 200);

                    foreach (BMI_Earning earning in earnings)
                    {
                        cmd.Parameters["@IMPORT_FILENAME"].Value = importFileName;
                        cmd.Parameters["@PERIOD"].Value = earning.PERIOD;
                        cmd.Parameters["@W_OR_P"].Value = earning.W_OR_P;
                        cmd.Parameters["@PARTICIPANT_NAME"].Value = earning.PARTICIPANT_NAME;
                        cmd.Parameters["@PARTICIPANT_NUM"].Value = earning.PARTICIPANT_NUM;
                        cmd.Parameters["@IP_NUM"].Value = earning.IP_NUM;
                        cmd.Parameters["@TITLE_NAME"].Value = earning.TITLE_NAME;
                        cmd.Parameters["@TITLE_NUM"].Value = earning.TITLE_NUM;
                        cmd.Parameters["@PERF_SOURCE"].Value = earning.PERF_SOURCE;
                        cmd.Parameters["@COUNTRY_OF_PERFORMANCE"].Value = earning.COUNTRY_OF_PERFORMANCE;
                        cmd.Parameters["@SHOW_NAME"].Value = earning.SHOW_NAME;
                        cmd.Parameters["@EPISODE_NAME"].Value = earning.EPISODE_NAME;
                        cmd.Parameters["@SHOW_NUM"].Value = earning.SHOW_NUM;
                        cmd.Parameters["@USE_CODE"].Value = earning.USE_CODE;
                        cmd.Parameters["@TIMING"].Value = earning.TIMING;
                        cmd.Parameters["@PARTICIPANT_PCT"].Value = earning.PARTICIPANT_PCT;
                        cmd.Parameters["@PERF_COUNT"].Value = earning.PERF_COUNT;
                        cmd.Parameters["@BONUS_LEVEL"].Value = earning.BONUS_LEVEL;
                        cmd.Parameters["@ROYALTY_AMOUNT"].Value = earning.ROYALTY_AMOUNT;
                        cmd.Parameters["@WITHHOLD"].Value = earning.WITHHOLD;
                        cmd.Parameters["@PERF_PERIOD"].Value = earning.PERF_PERIOD;
                        cmd.Parameters["@CURRENT_ACTIVITY_AMT"].Value = earning.CURRENT_ACTIVITY_AMT;
                        cmd.Parameters["@HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS"].Value = earning.HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS;
                        cmd.Parameters["@STANDARDS_OR_TV_NET_THEME_BONUS"].Value = earning.STANDARDS_OR_TV_NET_THEME_BONUS;
                        cmd.Parameters["@FOREIGN_SOCIETY_ADJUSTMENT"].Value = earning.FOREIGN_SOCIETY_ADJUSTMENT;
                        cmd.Parameters["@COMPANY_CODE"].Value = earning.COMPANY_CODE;
                        cmd.Parameters["@COMPANY_NAME"].Value = earning.COMPANY_NAME;
                        cmd.Parameters["@FILEHASH"].Value = sFileHash;

                        cmd.ExecuteNonQuery();
                        recordCount++;


                    }
                    //last commit to ensure that there are no abandonded records.
                    trans.Commit();
                    conn.Close();

                }
            }
            catch (SqlException sqlEx)
            {
                Debug.WriteLine(sqlEx.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return retVal;
        }

        /// <summary>
        /// Inserts a List of Tunecore_Earning(s) into the database
        /// </summary>
        /// <param name="earnings">A Templated list of Models.Tunecore_Earning</param>
        /// <returns>bool true if inserted, false if not</returns>
        public static bool InsertTunecoreEarnings(List<Tunecore_Earning> earnings, string importFileName, string sFileHash)
        {
            SqlConnectionStringBuilder builder = CorpusDataConnection();

            bool retVal = false;

            int recordCount = 0;


            try
            {
                StringBuilder sbCommand = new StringBuilder();
                sbCommand.Append(" INSERT INTO [staging].[Tunecore_Earnings] ");
                sbCommand.Append(" ([Sales_Period] ,[Posted_Date] ,[Store_Name] ,[Country_Of_Sale] ,[Artist] ,[Release_Type] ,[Release_Title] ,[Song_Title] ,[Label] ,[UPC] ,[Optional_UPC] ,[TC_Song_ID] ,[Optional_ISRC] ,[Sales_Type] ,[Num_Units_Sold] ,[Per_Unit_Price] ,[Net_Sales] ,[Net_Sales_Currency] ,[Exchange_Rate] ,[Total_Earned] ,[Currency] ,[IMPORT_FILENAME], [Artist_Id],  [FILEHASH] ) ");  //,[SR_Tunecore_Artist_ID]
                sbCommand.Append(" VALUES ");
                sbCommand.Append(" ( @Sales_Period , @Posted_Date , @Store_Name , @Country_Of_Sale , @Artist , @Release_Type , @Release_Title , @Song_Title , @Label , @UPC , @Optional_UPC , @TC_Song_ID , @Optional_ISRC , @Sales_Type , @Num_Units_Sold , @Per_Unit_Price , @Net_Sales , @Net_Sales_Currency , @Exchange_Rate , @Total_Earned , @Currency , @IMPORT_FILENAME, @Artist_Id, @FILEHASH ); "); //, @SR_Tunecore_Artist_ID
                //sbCommand.Append(" SELECT SCOPE_IDENTITY() ");
                using (SqlConnection conn = new SqlConnection(builder.ToString()))
                {

                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand(sbCommand.ToString(), conn, trans);

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;
                    //cmd.Parameters.Add("@SR_Tunecore_Artist_ID", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Sales_Period", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Posted_Date", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Store_Name", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Country_Of_Sale", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Artist", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Release_Type", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Release_Title", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Song_Title", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Label", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@UPC", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Optional_UPC", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@TC_Song_ID", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Optional_ISRC", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Sales_Type", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Num_Units_Sold", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Per_Unit_Price", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Net_Sales", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Net_Sales_Currency", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Exchange_Rate", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Total_Earned", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Currency", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@IMPORT_FILENAME", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Artist_Id", SqlDbType.BigInt);
                    cmd.Parameters.Add("@FILEHASH", SqlDbType.NVarChar, 200);


                    foreach (Tunecore_Earning earning in earnings)
                    {
                        //cmd.Parameters["@SR_Tunecore_Artist_ID"].Value = earning.SR_Tunecore_Artist_ID;
                        cmd.Parameters["@Sales_Period"].Value = earning.Sales_Period;
                        cmd.Parameters["@Posted_Date"].Value = earning.Posted_Date;
                        cmd.Parameters["@Store_Name"].Value = earning.Store_Name;
                        cmd.Parameters["@Country_Of_Sale"].Value = earning.Country_Of_Sale;
                        cmd.Parameters["@Artist"].Value = earning.Artist;
                        cmd.Parameters["@Release_Type"].Value = earning.Release_Type;
                        cmd.Parameters["@Release_Title"].Value = earning.Release_Title;
                        cmd.Parameters["@Song_Title"].Value = earning.Song_Title;
                        cmd.Parameters["@Label"].Value = earning.Label;
                        cmd.Parameters["@UPC"].Value = earning.UPC;
                        cmd.Parameters["@Optional_UPC"].Value = earning.Optional_UPC;
                        cmd.Parameters["@TC_Song_ID"].Value = earning.TC_Song_ID;
                        cmd.Parameters["@Optional_ISRC"].Value = earning.Optional_ISRC;
                        cmd.Parameters["@Sales_Type"].Value = earning.Sales_Type;
                        cmd.Parameters["@Num_Units_Sold"].Value = earning.Num_Units_Sold;
                        cmd.Parameters["@Per_Unit_Price"].Value = earning.Per_Unit_Price;
                        cmd.Parameters["@Net_Sales"].Value = earning.Net_Sales;
                        cmd.Parameters["@Net_Sales_Currency"].Value = earning.Net_Sales_Currency;
                        cmd.Parameters["@Exchange_Rate"].Value = earning.Exchange_Rate;
                        cmd.Parameters["@Total_Earned"].Value = earning.Total_Earned;
                        cmd.Parameters["@Currency"].Value = earning.Currency;
                        cmd.Parameters["@IMPORT_FILENAME"].Value = earning.IMPORT_FILENAME;
                        cmd.Parameters["@Artist_Id"].Value = int.Parse(earning.Artist_Id);  //switch this to int.TryParse if there are issues with conversion
                        cmd.Parameters["@FILEHASH"].Value = sFileHash;

                        cmd.ExecuteNonQuery();
                        recordCount++;


                    }
                    //last commit to ensure that there are no abandonded records.
                    trans.Commit();
                    conn.Close();

                }
            }
            catch (SqlException sqlEx)
            {
                Debug.WriteLine(sqlEx.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return retVal;
        }



        // <summary>
        /// Gets all of the rows from the BMI_Earnings table.
        /// TODO: Need to setup a filter for this by person and/or time range?
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectASCAPEarnings(string Party_ID)
        {

            DataTable table = new DataTable();
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT [File_Type] ,[Statement_Recipient_Name] ,[Statement_Recipient_ID] ,[Party_Name] ,[Party_ID] ,[Legal_Earner_Party_ID] ,[Legal_Earner_Party_Name] ,[Distribution_Date] ,[Distribution_Year] ");
                    sb.Append(" ,[Distribution_Quarter] ,[File_Distribution_Year] ,[File_Distribution_Quarter] ,[Country_Name] ,[Licensor] ,[Licensor_Code] ,[Performance_Start_Date] ,[Performance_End_Date] ,[Duration] ,[Number_of_Plays] ");
                    sb.Append(" ,[Work_Title] ,[Work_ID] ,[Member_Share] ,[Series_Name] ,[Program_Name] ,[Dollar_GENERAL] ,[Dollar_RADIO] ,[Dollar_TV] ,[Dollar_TV_FILM] ,[Dollar_CINEMA] ,[Dollar_TOTAL] ,[Revenue_Class_Code] ");
                    sb.Append(" ,[Revenue_Class_Description] ,[Adjustment_Distribution_Date] ,[Adjustment_Indicator] ,[Adjustment_Reason_Code] ,[Role_Type] ,[Type_Of_Right] ,[Territory] ,[Performance_Source] ,[Music_User ] ,[Network_Service] ");
                    sb.Append(" ,[Survey_Type] ,[Day_Part_Code] ,[CA_Pct] ,[Classification_Code] ,[Performance_Type] ,[Performing_Artist] ,[Composer_Name] ,[EE_Share] ,[Credits] ,[Premium_Credits] ,[Premium_Dollars] ,[Original_Distribution_Date] ");
                    sb.Append(" FROM [staging].[ASCAP_Earnings] ");
                    sb.Append(" WHERE Party_ID = @Party_ID ");

                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        SqlParameter pPartyId = new SqlParameter("@Party_ID", SqlDbType.NVarChar);
                        pPartyId.Value = Party_ID;
                        command.Parameters.Add(pPartyId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            try
                            {
                                adapter.Fill(table);
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return table;
        }


        /// <summary>
        /// Gets all of the rows from the ASCAP_Earnings table.
        /// TODO: Need to setup a filter for this by person and/or time range?
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAllASCAPEarningsParticipantNameAndNumber()
        {

            DataTable table = new DataTable();
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT Party_Id, Party_Name ");
                    sb.Append(" FROM [staging].[ASCAP_Earnings]  ");
                    sb.Append(" GROUP BY  Party_Id, Party_Name ");
                    sb.Append(" ORDER BY Party_Name  ");
                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        //SqlParameter pStatusTypesId = new SqlParameter("@StatusTypesId", SqlDbType.Int);
                        //pStatusTypesId.Value = iStatusTypesId;
                        //command.Parameters.Add(pStatusTypesId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            try
                            {
                                adapter.Fill(table);
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return table;
        }



        /// <summary>
        /// Method to be used as a count of records imported from a file and a dupe check for ASCAP International Earnings Statements
        /// </summary>
        /// <param name="participant_num">The ASCAP International Party ID</param>
        /// <param name="importFileName">The name of the file was/being imported</param>
        /// <returns>The count of records imported</returns>
        public static int SelectCountOfASCAPEarningsImportedFromFile(string party_id, string importFileName)
        {
            int recordCount = 0;
            try
            {
                SqlConnectionStringBuilder builder = CorpusDataConnection();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" SELECT  COUNT(*) FROM [staging].[ASCAP_Earnings] ");
                    sb.Append("	WHERE Party_ID = @PARTY_ID  AND IMPORT_FILENAME = @IMPORT_FILENAME ");

                    //sb.Append("	WHERE PARTICIPANT_NAME =  @ParticipantName ");               

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        SqlParameter pPartyId = new SqlParameter("@PARTY_ID", SqlDbType.NVarChar);
                        pPartyId.Value = party_id;
                        command.Parameters.Add(pPartyId);

                        SqlParameter pImportFileName = new SqlParameter("@IMPORT_FILENAME", SqlDbType.NVarChar);
                        pImportFileName.Value = importFileName;
                        command.Parameters.Add(pImportFileName);


                        try
                        {
                            var i = command.ExecuteScalar();
                            if (i != null)
                                int.TryParse(i.ToString(), out recordCount);


                        }
                        catch (SqlException sx)
                        {
                            Console.WriteLine(sx.ToString());
                        }
                        catch (Exception ex)
                        {                         
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return recordCount;
        }


        /// <summary>
        /// Inserts a List of BMI_Earning(s) into the database
        /// </summary>
        /// <param name="earnings">A Templated list of Models.BMI_Earning</param>
        /// <returns>bool true if inserted, false if not</returns>
        public static bool InsertASCAPEarnings(List<ASCAP_Earning> earnings)
        {
            SqlConnectionStringBuilder builder = CorpusDataConnection();

            bool retVal = false;
            int recordCount = 0;


            try
            {
                StringBuilder sbCommand = new StringBuilder();
                sbCommand.Append(" INSERT INTO [staging].[ASCAP_Earnings]   ");
                sbCommand.Append(" ([File_Type] ,[Statement_Recipient_Name] ,[Statement_Recipient_ID] ,[Party_Name] ,[Party_ID] ,[Legal_Earner_Party_ID] ,[Legal_Earner_Party_Name] ,[Distribution_Date] ,[Distribution_Year] ,[Distribution_Quarter] ,[File_Distribution_Year] ,[File_Distribution_Quarter] ,[Country_Name]  ");
                sbCommand.Append(" ,[Licensor] ,[Licensor_Code] ,[Performance_Start_Date] ,[Performance_End_Date] ,[Duration] ,[Number_of_Plays] ,[Work_Title] ,[Work_ID] ,[Member_Share] ,[Series_Name] ,[Program_Name] ,[Dollar_GENERAL] ,[Dollar_RADIO] ,[Dollar_TV] ,[Dollar_TV_FILM] ,[Dollar_CINEMA] ,[Dollar_TOTAL]  ");
                sbCommand.Append(" ,[Revenue_Class_Code] ,[Revenue_Class_Description] ,[Adjustment_Distribution_Date] ,[Adjustment_Indicator] ,[Adjustment_Reason_Code] ,[Role_Type] ,[Type_Of_Right] ,[Territory] ,[Performance_Source] ,[Music_User ] ,[Network_Service] ,[Survey_Type] ,[Day_Part_Code] ,[CA_Pct]  ");
                sbCommand.Append(" ,[Classification_Code] ,[Performance_Type] ,[Performing_Artist] ,[Composer_Name] ,[EE_Share] ,[Credits] ,[Premium_Credits] ,[Premium_Dollars] ,[Original_Distribution_Date] ,[IMPORT_FILENAME])     ");
                sbCommand.Append(" VALUES     ");
                sbCommand.Append(" ( @File_Type ,@Statement_Recipient_Name ,@Statement_Recipient_ID ,@Party_Name ,@Party_ID ,@Legal_Earner_Party_ID ,@Legal_Earner_Party_Name ,@Distribution_Date ,@Distribution_Year ,@Distribution_Quarter ,@File_Distribution_Year ,@File_Distribution_Quarter ,@Country_Name  ");
                sbCommand.Append(" ,@Licensor ,@Licensor_Code ,@Performance_Start_Date ,@Performance_End_Date ,@Duration ,@Number_of_Plays ,@Work_Title ,@Work_ID ,@Member_Share ,@Series_Name ,@Program_Name ,@Dollar_GENERAL ,@Dollar_RADIO ,@Dollar_TV ,@Dollar_TV_FILM ,@Dollar_CINEMA ,@Dollar_TOTAL  ");
                sbCommand.Append(" ,@Revenue_Class_Code ,@Revenue_Class_Description ,@Adjustment_Distribution_Date ,@Adjustment_Indicator ,@Adjustment_Reason_Code ,@Role_Type ,@Type_Of_Right ,@Territory ,@Performance_Source ,@Music_User  ,@Network_Service ,@Survey_Type ,@Day_Part_Code ,@CA_Pct  ");
                sbCommand.Append(" ,@Classification_Code ,@Performance_Type ,@Performing_Artist ,@Composer_Name ,@EE_Share ,@Credits ,@Premium_Credits ,@Premium_Dollars ,@Original_Distribution_Date ,@IMPORT_FILENAME )   ");
                //sbCommand.Append(" SELECT SCOPE_IDENTITY() ");
                using (SqlConnection conn = new SqlConnection(builder.ToString()))
                {

                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand(sbCommand.ToString(), conn, trans);

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;
                    cmd.Parameters.Add("@File_Type", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Statement_Recipient_Name", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Statement_Recipient_ID", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Party_Name", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Party_ID", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Legal_Earner_Party_ID", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Legal_Earner_Party_Name", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Distribution_Date", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Distribution_Year", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Distribution_Quarter", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@File_Distribution_Year", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@File_Distribution_Quarter", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Country_Name", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Licensor", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Licensor_Code", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Performance_Start_Date", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Performance_End_Date", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Duration", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Number_of_Plays", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Work_Title", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Work_ID", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Member_Share", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Series_Name", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Program_Name", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Dollar_GENERAL", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Dollar_RADIO", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Dollar_TV", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Dollar_TV_FILM", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Dollar_CINEMA", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Dollar_TOTAL", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Revenue_Class_Code", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Revenue_Class_Description", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Adjustment_Distribution_Date", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Adjustment_Indicator", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Adjustment_Reason_Code", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Role_Type", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Type_Of_Right", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Territory", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Performance_Source", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Music_User ", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Network_Service", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Survey_Type", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Day_Part_Code", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@CA_Pct", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Classification_Code", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Performance_Type", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Performing_Artist", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Composer_Name", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@EE_Share", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Credits", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Premium_Credits", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Premium_Dollars", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@Original_Distribution_Date", SqlDbType.NVarChar, 500);
                    cmd.Parameters.Add("@IMPORT_FILENAME", SqlDbType.NVarChar, 500);



                    foreach (ASCAP_Earning earning in earnings)
                    {
                        cmd.Parameters["@File_Type"].Value = earning.File_Type;
                        cmd.Parameters["@Statement_Recipient_Name"].Value = earning.Statement_Recipient_Name;
                        cmd.Parameters["@Statement_Recipient_ID"].Value = earning.Statement_Recipient_ID;
                        cmd.Parameters["@Party_Name"].Value = earning.Party_Name;
                        cmd.Parameters["@Party_ID"].Value = earning.Party_ID;
                        cmd.Parameters["@Legal_Earner_Party_ID"].Value = earning.Legal_Earner_Party_ID;
                        cmd.Parameters["@Legal_Earner_Party_Name"].Value = earning.Legal_Earner_Party_Name;
                        cmd.Parameters["@Distribution_Date"].Value = earning.Distribution_Date;
                        cmd.Parameters["@Distribution_Year"].Value = earning.Distribution_Year;
                        cmd.Parameters["@Distribution_Quarter"].Value = earning.Distribution_Quarter;
                        cmd.Parameters["@File_Distribution_Year"].Value = earning.File_Distribution_Year;
                        cmd.Parameters["@File_Distribution_Quarter"].Value = earning.File_Distribution_Quarter;
                        cmd.Parameters["@Country_Name"].Value = earning.Country_Name;
                        cmd.Parameters["@Licensor"].Value = earning.Licensor;
                        cmd.Parameters["@Licensor_Code"].Value = earning.Licensor_Code;
                        cmd.Parameters["@Performance_Start_Date"].Value = earning.Performance_Start_Date;
                        cmd.Parameters["@Performance_End_Date"].Value = earning.Performance_Start_Date;
                        cmd.Parameters["@Duration"].Value = earning.Duration;
                        cmd.Parameters["@Number_of_Plays"].Value = earning.Number_of_Plays;
                        cmd.Parameters["@Work_Title"].Value = earning.Work_Title;
                        cmd.Parameters["@Work_ID"].Value = earning.@Work_ID;
                        cmd.Parameters["@Member_Share"].Value = earning.Member_Share;
                        cmd.Parameters["@Series_Name"].Value = earning.Series_Name;
                        cmd.Parameters["@Program_Name"].Value = earning.Program_Name;
                        cmd.Parameters["@Dollar_GENERAL"].Value = earning.Dollar_GENERAL;
                        cmd.Parameters["@Dollar_RADIO"].Value = earning.Dollar_RADIO;
                        cmd.Parameters["@Dollar_TV"].Value = earning.Dollar_TV;
                        cmd.Parameters["@Dollar_TV_FILM"].Value = earning.Dollar_TV_FILM;
                        cmd.Parameters["@Dollar_CINEMA"].Value = earning.Dollar_CINEMA;
                        cmd.Parameters["@Dollar_TOTAL"].Value = earning.Dollar_TOTAL;
                        cmd.Parameters["@Revenue_Class_Code"].Value = earning.Revenue_Class_Code;
                        cmd.Parameters["@Revenue_Class_Description"].Value = earning.Revenue_Class_Description;
                        cmd.Parameters["@Adjustment_Distribution_Date"].Value = earning.Adjustment_Distribution_Date;
                        cmd.Parameters["@Adjustment_Indicator"].Value = earning.Adjustment_Indicator;
                        cmd.Parameters["@Adjustment_Reason_Code"].Value = earning.Adjustment_Reason_Code;
                        cmd.Parameters["@Role_Type"].Value = earning.Role_Type;
                        cmd.Parameters["@Type_Of_Right"].Value = earning.Type_Of_Right;
                        cmd.Parameters["@Territory"].Value = earning.Territory;
                        cmd.Parameters["@Performance_Source"].Value = earning.Performance_Source;
                        cmd.Parameters["@Music_User "].Value = earning.Music_User;
                        cmd.Parameters["@Network_Service"].Value = earning.Network_Service;
                        cmd.Parameters["@Survey_Type"].Value = earning.Survey_Type;
                        cmd.Parameters["@Day_Part_Code"].Value = earning.Day_Part_Code;
                        cmd.Parameters["@CA_Pct"].Value = earning.CA_Pct;
                        cmd.Parameters["@Classification_Code"].Value = earning.Classification_Code;
                        cmd.Parameters["@Performance_Type"].Value = earning.Performance_Type;
                        cmd.Parameters["@Performing_Artist"].Value = earning.Performing_Artist;
                        cmd.Parameters["@Composer_Name"].Value = earning.Composer_Name;
                        cmd.Parameters["@EE_Share"].Value = earning.EE_Share;
                        cmd.Parameters["@Credits"].Value = earning.Credits;
                        cmd.Parameters["@Premium_Credits"].Value = earning.Premium_Credits;
                        cmd.Parameters["@Premium_Dollars"].Value = earning.Premium_Dollars;
                        cmd.Parameters["@Original_Distribution_Date"].Value = earning.Original_Distribution_Date;
                        cmd.Parameters["@IMPORT_FILENAME"].Value = earning.Import_Filename;



                        cmd.ExecuteNonQuery();
                        recordCount++;


                    }
                    //last commit to ensure that there are no abandonded records.
                    trans.Commit();
                    conn.Close();

                }
            }
            catch (SqlException sqlEx)
            {
                Debug.WriteLine(sqlEx.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return retVal;
        }

        #endregion



    }
}
