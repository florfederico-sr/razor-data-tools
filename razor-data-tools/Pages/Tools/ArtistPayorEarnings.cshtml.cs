using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace razor_data_tools.Pages.Tools
{
    public class ArtistPayorEarningsModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ArtistPayorEarningsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public List<DropdownItem> Artists { get; set; }
        [BindProperty]
        public string SelectedArtist { get; set; }
        [BindProperty]
        public string SelectedPayor { get; set; }

        public void OnGet()
        {
            Artists = GetArtistsFromDatabase();
        }

        public IActionResult OnGetPayors(string artistId)
        {
            var payors = GetPayorsFromDatabase(artistId);
            return new JsonResult(payors);
        }

        // public async Task<IActionResult> OnGetExecuteQuery(string payorId, string artistId)
        // {
        //     var query = GetQueryFromDatabase(payorId);
        //     var dataTable = await ExecuteQueryAsync(query, artistId);
        //
        //     var fileContent = ExportToCsv(dataTable);
        //     return File(Encoding.UTF8.GetBytes(fileContent), "text/csv", "PayorEarnings.csv");
        // }
        
        public async Task<IActionResult> OnGetExecuteQuery(string payorId, string artistId)
        {
            try
            {
                var query = GetQueryFromDatabase(payorId);
                var dataTable = await ExecuteQueryAsync(query, artistId);

                if (dataTable.Rows.Count == 0)
                {
                    return StatusCode(204);  // No Content
                }

                var fileContent = ExportToCsv(dataTable);

                // Convert the file content to a byte array
                byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);

                // Return the file as a download (no need to manually set the Content-Length)
                return File(fileBytes, "text/csv", "PayorEarnings.csv");
            }
            catch (Exception ex)
            {
                // Log and return error
                Console.WriteLine("Error executing query: " + ex.Message);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }




       

        // Retrieves list of artists from dbo.Artist
        private List<DropdownItem> GetArtistsFromDatabase()
        {
            var artists = new List<DropdownItem>();
            string connectionString = Data.Services.CorpusDataConnection().ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT a.[Artist_Id] as ArtistId, a.[ARTIST_NAME_FILE] as ArtistName FROM [dbo].[Artist] a INNER JOIN [dbo].[Hist_ProcessedFiles] hpe ON a.Artist_Id = hpe.Artist_Id GROUP BY a.[Artist_Id], a.[ARTIST_NAME_FILE] ORDER BY a.[ARTIST_NAME_FILE]"; 
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        artists.Add(new DropdownItem
                        {
                            Id = reader["ArtistId"].ToString(),
                            Name = reader["ArtistName"].ToString()
                        });
                    }
                }
            }

            return artists;
        }

        // Retrieves list of payors based on the selected artist from dbo.ArtistPayor_Earnings
        private List<DropdownItem> GetPayorsFromDatabase(string artistId)
        {
            var payors = new List<DropdownItem>();
            string connectionString = Data.Services.CorpusDataConnection().ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT hpe.[Payor_Id] as PayorId, hpe.[PAYOR_NAME] as PayorName FROM [dbo].[Artist] a INNER JOIN [dbo].[Hist_ProcessedFiles] hpe ON a.Artist_Id = hpe.Artist_Id AND a.Artist_Id = @ArtistId GROUP BY hpe.[Payor_Id], hpe.[PAYOR_NAME] ORDER BY hpe.[PAYOR_NAME]";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ArtistId", artistId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payors.Add(new DropdownItem
                        {
                            Id = reader["PayorId"].ToString(),
                            Name = reader["PayorName"].ToString()
                        });
                    }
                }
            }

            return payors;
        }

        // Retrieves the query for the selected payor from dbo.DataToolQueries.[Query]
        private string GetQueryFromDatabase(string payorId)
        {
            string query = string.Empty;
            string connectionString = Data.Services.CorpusDataConnection().ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT [QueryString] FROM dbo.DataToolQueries WHERE PayorEarnings_Id = @PayorId";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@PayorId", payorId);

                connection.Open();
                query = command.ExecuteScalar()?.ToString();
            }

            return query;
        }

        // Executes the retrieved query asynchronously, with the provided ArtistId as a parameter
        private async Task<DataTable> ExecuteQueryAsync(string query, string artistId)
        {
            DataTable dataTable = new DataTable();
            string connectionString = Data.Services.CorpusDataConnection().ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ArtistId", artistId);  // Pass the ArtistId to the query as a parameter
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                connection.Open();
                await Task.Run(() => adapter.Fill(dataTable));
            }

            return dataTable;
        }

        // Exports the results to CSV format
        private string ExportToCsv(DataTable dataTable)
        {
            var csv = new StringBuilder();
            
            // Add column headers
            foreach (DataColumn column in dataTable.Columns)
            {
                csv.Append(column.ColumnName + ",");
            }
            csv.AppendLine();

            // Add row data
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    csv.Append(item.ToString() + ",");
                }
                csv.AppendLine();
            }

            return csv.ToString();
        }
    }

    public class DropdownItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
