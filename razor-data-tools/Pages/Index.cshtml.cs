using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SoftCircuits.CsvParser;
using razor_data_tools.Models;
using razor_data_tools.Utils;
using System.Text;


namespace razor_data_tools.Pages
{
   

    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }


        //#region Page Properties
        //[BindProperty]
        //public IFormFile UploadCsv { get; set; }

        //[BindProperty]
        //public IFormFile DownloadFile { get; set; }

        //[BindProperty]
        //public String Message { get; set; }

        //#endregion

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    return await Import(UploadCsv);

        //}

        //public ActionResult OnPostFileUpload(string var1, string var2)
        //{
        //    //Use elements from the row to perform an action
        //    var test = var1;
        //    var t2 = var2;

        //    return null;
        //}

        //public ActionResult OnPostFileDownload(string data)
        //{            
        //   return DownloadCsv();            
        //}


        //public FileResult DownloadCsv()
        //{
        //    var lstData = General.ConvertDataTableToList(Data.Services.SelectBMIEarnings());
        //    var sb = new StringBuilder();
        //    foreach (var data in lstData)
        //    {
        //        sb.AppendLine(  data.PERIOD + "," +
        //                        data.W_OR_P + "," +
        //                        data.PARTICIPANT_NAME + "," +
        //                        data.PARTICIPANT_NUM + "," +
        //                        data.IP_NUM + "," +
        //                        data.TITLE_NAME + "," +
        //                        data.TITLE_NUM + "," +
        //                        data.PERF_SOURCE + "," +
        //                        data.COUNTRY_OF_PERFORMANCE + "," +
        //                        data.SHOW_NAME + "," +
        //                        data.EPISODE_NAME + "," +
        //                        data.SHOW_NUM + "," +
        //                        data.USE_CODE + "," +
        //                        data.TIMING + "," +
        //                        data.PARTICIPANT_PCT + "," +
        //                        data.PERF_COUNT + "," +
        //                        data.BONUS_LEVEL + "," +
        //                        data.ROYALTY_AMOUNT + "," +
        //                        data.WITHHOLD + "," +
        //                        data.PERF_PERIOD + "," +
        //                        data.CURRENT_ACTIVITY_AMT + "," +
        //                        data.HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS + "," +
        //                        data.STANDARDS_OR_TV_NET_THEME_BONUS + "," +
        //                        data.FOREIGN_SOCIETY_ADJUSTMENT + "," +
        //                        data.COMPANY_CODE + "," +
        //                        data.COMPANY_NAME);
        //    }
        //    Message = "Returning File...";
        //    byte[] bytes = ASCIIEncoding.ASCII.GetBytes(sb.ToString());
            
        //    return File(bytes, "text/csv", "export.csv");

        //}




        //public async Task<IActionResult> Import(IFormFile formFile)
        //{
        //    if (formFile == null || formFile.Length <= 0)
        //    {
        //        Message = "This is not a valid file.";
        //        return Page();
        //    }

        //    if (formFile.Length > 500000)
        //    {
        //        Message = "File should be less then 0.5 Mb";
        //        return Page();
        //    }

        //    if (!Path.GetExtension(formFile.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        //    {
        //        Message = "Wrong file format. Should be csv.";
        //        return Page();
        //    }
   

        //    try
        //    {

        //        //using (var reader = new StreamReader(File.InputStream))
        //        //using (var csvReader = new CsvReader(reader))
        //        //{
        //        //    // Use While(csvReader.Read()); if you want to read all the rows in the records)
        //        //    csvReader.Read();
        //        //    return csvReader.GetRecords<Models.CsvBMI>().ToArray();
        //        //}

        //        using (var stream = new MemoryStream())
        //        {
        //            await formFile.CopyToAsync(stream);
        //            stream.Position = 0;
        //            Utils.Csv.ImportBMIEarningsCsvAndInsertIntoDatabase(stream);
        //            //Utils.Csv.ReadCsvAndMap(stream);
        //            //await bulkUpload.JobCsv.CopyToAsync(memoryStream);
        //            //memoryStream.Position = 0;
        //            //TextReader textReader = new StreamReader(memoryStream);

        //            //var csv = new CsvReader(textReader);

        //            //csv.Configuration.HasHeaderRecord = true;

        //            //bulkList = csv.GetRecords<BulkUploadDataModel>().ToList();

        //            //using (var package = new ExcelPackage(stream))
        //            //{
        //            //    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //            //    var rowCount = worksheet.Dimension.Rows;

        //            //    for (int row = 2; row <= rowCount; row++)
        //            //    {
        //            //        newList.Add(new InvoiceModel
        //            //        {
        //            //            //ID = row - 1,
        //            //            InvoiceNumber = int.Parse(worksheet.Cells[row, 1].Value.ToString().Trim()),
        //            //            Amount = float.Parse(worksheet.Cells[row, 2].Value.ToString().Trim()),
        //            //            CostCategory = worksheet.Cells[row, 3].Value.ToString().Trim(),
        //            //            Period = worksheet.Cells[row, 4].Value.ToString().Trim(),
        //            //        });
        //            //    }
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Message = "Error while parsing the file. Check the column order and format.";
        //        return Page();
        //    }


            

        //    return RedirectToPage("./Index");
        //}


    }
}
