using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using razor_data_tools.Models;
using Radzen.Blazor.Rendering;
using Newtonsoft.Json.Linq;
using razor_data_tools.Data;
using razor_data_tools.Utils;
using System.IO;
using System.Diagnostics;

namespace razor_data_tools.Pages.Tools
{
	public class Tunecore_Earnings_Import_2Model : PageModel
    {
        [BindProperty]
        public string SelectedArtist { get; set; }
        [BindProperty]
        public string NewArtistName { get; set; }
        public SelectList Artists { get; set; }

        [BindProperty]
        public List<Models.FileProperty> ImportedFileProperties { get; set; }

        public void OnGet()
        {
            InitPageData();
        }

        private void InitPageData()
        {
            var artists = GetArtistsFromDatabase().ToList();
            artists.Insert(0, new SelectListItem { Text = "New Artist", Value = "New Artist" });
            Artists = new SelectList(artists, "Value", "Text");
        }

        public PageResult OnPostFileUpload(IFormFile[] files)
        {

            if (files != null && files.Length > 0)
            {
                if (SelectedArtist == "New Artist")
                {
                    if (string.IsNullOrEmpty(NewArtistName))
                    {
                        ModelState.AddModelError("", "Please enter a name for the new artist.");
                        //return Task.FromResult<IActionResult>(Page());
                    }

                    try { 
                    // Insert New Artist into Database and Get New Artist ID
                        var newArtistId = InsertNewArtist(NewArtistName);
                        // Process File Upload for New Artist
                        ProcessFileUpload(files, newArtistId.ToString());
                    
                        // After processing files successfully:
                        TempData["SuccessMessage"] = "File uploaded successfully. The page is ready for another upload.";
                        // Instead of returning Task.FromResult, return the appropriate action result
                        InitPageData();
                        return Page(); // This will reload the page and display the success message.
                        
                    }
                    catch(Exception ex)
                    {
                        TempData["SuccessMessage"] = "There was an issue uploading the file.  Please contact support with this message:  " + ex.ToString();
                        // Instead of returning Task.FromResult, return the appropriate action result
                        InitPageData();
                        return Page(); // This will reload the page and display the success message.
                    }
                }
                else if (!string.IsNullOrEmpty(SelectedArtist))
                {
                    try
                    {
                        // Process File Upload for Existing Artist
                        ProcessFileUpload(files, SelectedArtist);
                        // After processing files successfully:
                        TempData["SuccessMessage"] = "File uploaded successfully. The page is ready for another upload.";
                        InitPageData();
                        // Instead of returning Task.FromResult, return the appropriate action result
                        return Page(); // This will reload the page and display the success message.

                    }
                    catch (Exception ex)
                    {
                        TempData["SuccessMessage"] = "There was an issue uploading the file.  Please contact support with this message:  " + ex.ToString();
                        // Instead of returning Task.FromResult, return the appropriate action result
                        InitPageData();
                        return Page(); // This will reload the page and display the success message.
                    }
                }
                else
                {
                    
                    TempData["SuccessMessage"] = "Please select an artist.";
                    // Instead of returning Task.FromResult, return the appropriate action result
                    InitPageData();
                    return Page(); // This will reload the page and display the success message.

                    //return Task.FromResult<IActionResult>(Page());
                }

                //return Task.FromResult<IActionResult>(RedirectToPage("./Index"));
            }
            else //there was no file uploaded
            {
                //ModelState.AddModelError("", "Please upload a file.");
                TempData["SuccessMessage"] = "Please upload a file.";
                // Instead of returning Task.FromResult, return the appropriate action result
                InitPageData();
                return Page(); // This will reload the page and display the success message.
                //return Task.FromResult<IActionResult>(Page());
            }
        }


        private IEnumerable<SelectListItem> GetArtistsFromDatabase()
        {

            List<Artist> artists = Utils.General.ConvertArtistDataTableToList(Data.Services.SelectAllArtists());
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            foreach (var a in artists)
            {
                var selectListItem = new SelectListItem
                { Text = a.Artist_Name,
                  Value = a.Artist_Id.ToString()
                };
                selectListItems.Add(selectListItem);
            }

            return selectListItems;
        }

        private int InsertNewArtist(string newArtistName)
        {
            //Insert the artist
            int iNewArtistId = -1;
            string cleanNewArtistName = Utils.General.RemovePunctuation(newArtistName.ToUpper());

            iNewArtistId = Services.InsertArtist(cleanNewArtistName);

            return iNewArtistId; // Return the new artist's ID
        }

        private void ProcessFileUpload(IFormFile[] files, string artistId)
        {
            int importedRecordCount = 0;
            

            foreach (var file in files)
            {
                string importFileName = file.FileName;
                
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    //await file.CopyToAsync(stream);
                    stream.Position = 0;
                    importedRecordCount = Csv.ImportTunecoreEarningsCsvAndInsertIntoDatabase(stream, importFileName, artistId);
                    //append the filename and amount of records imported to their arrays

                    //_customerItems.Add(new SelectListItem(customer.PARTICIPANT_NAME, customer.PARTICIPANT_NUM));
                    ImportedFileProperties.Add(General.FactoryFileProperties(importFileName, importedRecordCount));

                    //increment the counter
                    //counter++;
                }
            }
        }


    }
}
