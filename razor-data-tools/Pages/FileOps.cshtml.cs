using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using razor_data_tools.Data;
using razor_data_tools.Models;
using razor_data_tools.Utils;


namespace razor_data_tools.Pages
{
    
    public class FileOpsModel : PageModel
    {
        
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".csv" };        
        private readonly string _targetFilePath;

        [ViewData]
        public List<BMI_Earnings_Customer> ListOfEarningsCustomers { get; set; }

     
        
        //list containing the select items bound OnGet
        private List<SelectListItem>  _customerItems{ get;  set; }
        //The selectlist that will be filled with CustomerItems and bound to the control on the page with Model.Customers
        public SelectList Customers { get; set; }
        //property to receive the selected value
        [BindProperty(SupportsGet = true)]
        public string sSelectedCustomer{ get; set; }

        [BindProperty]
        public BufferedMultipleFileUploadPhysical FileUpload { get; set; }
        //public List<SelectListItem> Options { get; set; }

        //public BufferedMultipleFileUploadPhysicalModel(IConfiguration config)
        //{
        //    _fileSizeLimit = config.GetValue<long>("FileSizeLimit");

        //    // To save physical files to a path provided by configuration:
        //    _targetFilePath = config.GetValue<string>("StoredFilesPath");

        //    // To save physical files to the temporary files folder, use:
        //    //_targetFilePath = Path.GetTempPath();
        //}

      


        public string Result { get; private set; }

        public FileOpsModel()
        {
            _customerItems = new List<SelectListItem>(); 
            List<BMI_Earnings_Customer> ListOfEarningsCustomers = General.ConvertBMICustomerToList(Data.Services.SelectAllBMIEarningsParticipantNameAndNumber());

            //Loop through the customers and create the select list
            foreach (var customer in ListOfEarningsCustomers)
            {
                _customerItems.Add(new SelectListItem(customer.PARTICIPANT_NAME, customer.PARTICIPANT_NUM));
                
            }
            
          
        }


        public void OnGet()
        {

            Customers = new SelectList(_customerItems, "Value", "Text");
            //ViewData["CustomerItems"] = CustomerItems;
            
        }

       
        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";

                return Page();
            }

            foreach (var formFile in FileUpload.FormFiles)
            {
                var formFileContent =  await Utils.FileHelpers.ProcessFormFile<BufferedMultipleFileUploadPhysical>(formFile, ModelState);

                if (!ModelState.IsValid)
                {
                    Result = "Please correct the form.";

                    return Page();
                }

                #region Commented Code



                #endregion
            }

            return RedirectToPage("./Index");
        }


        public ActionResult OnPostFileDownload()
        {
            string sParticipantNum = sSelectedCustomer;
            return DownloadCsv(sParticipantNum);
        }


        #region Upload/Download methods


        public FileResult DownloadCsv(string sParticipantNum)
        {

            var lstData = Utils.General.ConvertBMIDataTableToList(Data.Services.SelectBMIEarnings(sParticipantNum));
            var sb = new StringBuilder();
            foreach (var data in lstData)
            {
                sb.AppendLine(data.PERIOD + "," +
                                data.W_OR_P + "," +
                                data.PARTICIPANT_NAME + "," +
                                data.PARTICIPANT_NUM + "," +
                                data.IP_NUM + "," +
                                data.TITLE_NAME + "," +
                                data.TITLE_NUM + "," +
                                data.PERF_SOURCE + "," +
                                data.COUNTRY_OF_PERFORMANCE + "," +
                                data.SHOW_NAME + "," +
                                data.EPISODE_NAME + "," +
                                data.SHOW_NUM + "," +
                                data.USE_CODE + "," +
                                data.TIMING + "," +
                                data.PARTICIPANT_PCT + "," +
                                data.PERF_COUNT + "," +
                                data.BONUS_LEVEL + "," +
                                data.ROYALTY_AMOUNT + "," +
                                data.WITHHOLD + "," +
                                data.PERF_PERIOD + "," +
                                data.CURRENT_ACTIVITY_AMT + "," +
                                data.HITS_SONG_OR_TV_NET_SUPER_USAGE_BONUS + "," +
                                data.STANDARDS_OR_TV_NET_THEME_BONUS + "," +
                                data.FOREIGN_SOCIETY_ADJUSTMENT + "," +
                                data.COMPANY_CODE + "," +
                                data.COMPANY_NAME);
            }
            //Message = "Returning File...";
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(sb.ToString());

            return File(bytes, "text/csv", "export.csv");

        }

        #endregion


    }

    public class BufferedMultipleFileUploadPhysical
    {
        [Required]
        [Display(Name = "File")]
        public List<IFormFile> FormFiles { get; set; }

        [Display(Name = "Note")]
        [StringLength(50, MinimumLength = 0)]
        public string Note { get; set; }
    }
}