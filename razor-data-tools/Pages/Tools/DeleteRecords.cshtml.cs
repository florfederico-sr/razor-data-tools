using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace razor_data_tools.Pages.Tools
{
    public class DeleteRecordsModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DeleteRecordsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public List<DropdownItem> Artists { get; set; }
        [BindProperty]
        public string SelectedArtist { get; set; }
        [BindProperty]
        public string SelectedPayor { get; set; }
        [BindProperty]
        public string SelectedFileName { get; set; }

        public void OnGet()
        {
            Artists = GetArtistsFromDatabase();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Delete records based on selected artist, payor, and file name IDs
            if (!string.IsNullOrEmpty(SelectedArtist) && !string.IsNullOrEmpty(SelectedPayor) && !string.IsNullOrEmpty(SelectedFileName))
            {
                DeleteRecordsFromDatabase(SelectedArtist, SelectedPayor, SelectedFileName);
                return RedirectToPage("/Tools/DeleteRecords");
            }

            return RedirectToPage("/Tools/DeleteRecords");
        }

        public IActionResult OnGetGetPayors(string artistId)
        {
            var payors = GetPayorsFromDatabase(artistId);
            return new JsonResult(payors);
        }

        public IActionResult OnGetGetFileNames(string payorId, string artistId)
        {
            var fileNames = GetFileNamesFromDatabase(payorId, artistId);
            return new JsonResult(fileNames);
        }

        private List<DropdownItem> GetArtistsFromDatabase()
        {
            var artists = new List<DropdownItem>();
            string connectionString = Data.Services.CorpusDataConnection().ConnectionString;
            //string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT ape.[ARTIST_ID] as Id, a.[ARTIST_NAME_FILE] as ArtistName  FROM [dbo].[ArtistPayor_Earnings] ape INNER JOIN [dbo].[Artist] a ON ape.[ARTIST_ID] = a.[ARTIST_ID]  INNER JOIN [dbo].[Hist_ProcessedFiles] hpf ON hpf.[Artist_Id] = a.[Artist_Id] GROUP BY a.[ARTIST_NAME_FILE] ,ape.[ARTIST_ID] ORDER BY a.[ARTIST_NAME_FILE]";
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            artists.Add(new DropdownItem { Id = reader["Id"].ToString(), Name = reader["ArtistName"].ToString() });
                        }
                    }
                }
            }

            return artists;
        }

        private List<DropdownItem> GetPayorsFromDatabase(string artistId)
        {
            var payors = new List<DropdownItem>();
            string connectionString = Data.Services.CorpusDataConnection().ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT pe.PayorEarnigs_Id as Id, pe.PAYOR_NAME as PayorName FROM [dbo].[ArtistPayor_Earnings] ape INNER JOIN [dbo].[Artist] a ON ape.[ARTIST_ID] = a.[ARTIST_ID] INNER JOIN [dbo].[Payor_Earnings] pe ON pe.PayorEarnigs_Id = ape.PAYOR_ID INNER JOIN [dbo].[Hist_ProcessedFiles] hpf ON hpf.[Artist_Id] = a.[Artist_Id] WHERE ape.[ARTIST_ID] = @ArtistId GROUP BY pe.PayorEarnigs_Id, pe.PAYOR_NAME ORDER BY pe.PAYOR_NAME";
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ArtistId", artistId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payors.Add(new DropdownItem { Id = reader["Id"].ToString(), Name = reader["PayorName"].ToString() });
                        }
                    }
                }
            }

            return payors;
        }

        private List<DropdownItem> GetFileNamesFromDatabase(string payorId, string artistId)
        {
            int iArtistId = 0;
            int iPayorId = 0;
            
            int.TryParse(artistId, out iArtistId);
            int.TryParse(payorId, out iPayorId);
            
            
            var fileNames = new List<DropdownItem>();
            string connectionString = Data.Services.CorpusDataConnection().ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"select IMPORT_FILENAME as Id, IMPORT_FILENAME as FileName from [dbo].[Hist_ProcessedFiles] where Artist_id = @ArtistId AND Payor_Id = @PayorId"; //and payor_name ='ASCAP'
                //string query = @"SELECT DISTINCT IMPORT_FILENAME as FileId, IMPORT_FILENAME as FileName from " + payorId + " where Artist_Id = @ArtistId GROUP BY IMPORT_FILENAME";
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    
                    command.Parameters.Add("@PayorId", SqlDbType.BigInt).Value = iPayorId;
                    command.Parameters.Add("@ArtistId", SqlDbType.BigInt).Value = iArtistId;

                    // command.Parameters.AddWithValue("@PayorId", payorId);
                    // command.Parameters.AddWithValue("@ArtistId", artistId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fileNames.Add(new DropdownItem { Id = reader["Id"].ToString(), Name = reader["FileName"].ToString() });
                        }
                    }
                }
            }

            return fileNames;
        }

        private void DeleteRecordsFromDatabase(string artistId, string payorId, string fileNameId)
        {
            string connectionString = Data.Services.CorpusDataConnection().ConnectionString;

            //need to get the table to use in the delete statement based off the payorId
            GetPayorEarningsTableName(connectionString, payorId, out var payorEarningsTableName);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM  " + payorEarningsTableName + "  WHERE Artist_Id = @ArtistId AND IMPORT_FILENAME = @FileNameId"; // Replace 'Records' with your actual table name
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ArtistId", artistId);
                    command.Parameters.AddWithValue("@FileNameId", fileNameId);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void GetPayorEarningsTableName(string connectionString,string payorId, out string payorEarningsTableName)
        {
            payorEarningsTableName = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"select TABLE_NAME_PROD as table_name from [dbo].[Payor_Earnings] where PayorEarnigs_Id = @PayorId;"; //and payor_name ='ASCAP'
                
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PayorId", payorId);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payorEarningsTableName = reader["table_name"].ToString();
                            //fileNames.Add(new DropdownItem { Id = reader["Id"].ToString(), Name = reader["FileName"].ToString() });
                        }
                    }
                }
            }
        }

        public class DropdownItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
