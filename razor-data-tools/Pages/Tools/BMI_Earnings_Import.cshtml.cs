using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor_data_tools.Utils;


namespace razor_data_tools.Pages.Tools
{
    public class BMI_Earnings_ImportModel : PageModel
    {

        
        [BindProperty]
        public int[] ImportedCount { get; set; } = new int[0];
        [BindProperty]
        public string[] ImportedFile { get; set; } = new string[0];

        [BindProperty]
        public List<Models.FileProperty> ImportedFileProperties { get; set; }

        public void OnGet()
        {

        }

        public void OnPostFileUpload(IFormFile[] files)
        {
            
            int importedRecordCount = 0;
            int counter = 0;
            if (files != null && files.Length > 0)
                try
                {

                    foreach (var file in files)
                    {
                        string importFileName = file.FileName;

                        using (var stream = new MemoryStream())
                        {
                            file.CopyTo(stream);
                            //await file.CopyToAsync(stream);
                            stream.Position = 0;
                            importedRecordCount = Utils.Csv.ImportBMIEarningsCsvAndInsertIntoDatabase(stream, importFileName);
                            //append the filename and amount of records imported to their arrays
                            //TODO ADD FILE PROP INFO HERE
                            //_customerItems.Add(new SelectListItem(customer.PARTICIPANT_NAME, customer.PARTICIPANT_NUM));
                            ImportedFileProperties.Add(General.FactoryFileProperties(importFileName, importedRecordCount));

                            //increment the counter
                            counter++;
                            TempData["SuccessMessage"] = "File uploaded successfully. The page is ready for another upload.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    TempData["SuccessMessage"] = "There was an issue uploading the file.  Please contact support with this message:  " + ex.ToString();

                    //Message = "Error while parsing the file. Check the column order and format.";
                    //return Page();
                }

            //return RedirectToPage("./Index");

            //Message = "File has been processed.";
        }



    }
}