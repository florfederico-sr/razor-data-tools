using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using razor_data_tools.Utils;

namespace razor_data_tools.Pages.Tools
{
    public class FileUploader : PageModel
    {
        [BindProperty]
        public bool UploadSuccess { get; set; }

        [BindProperty]
        public List<string> Payors { get; set; }

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string Surname { get; set; }

        [BindProperty]
        public string SelectedPayor { get; set; }

        public async Task OnGetAsync()
        {
            Payors = await Data.Services.GetPayorsFromDatabaseAsync();
        }
        private readonly IConfiguration _configuration;

        private string sBlobConnectionString = @"DefaultEndpointsProtocol=https;AccountName=srdevbpoc;AccountKey=FgiU6TTYzKvjAfznzYNVfMwjdvOkLRMfuqz6d33FF+Fij2ZBZGLuYAYuPvyjXgcYwfmrBIw5RH948vpjTMYzgQ==;EndpointSuffix=core.windows.net";

        public FileUploader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

      

        public async Task<IActionResult> OnPostAsync()
        {
            var file = Request.Form.Files["file"];
            FirstName = Request.Form["firstName"];
            Surname = Request.Form["surname"];
            SelectedPayor = Request.Form["payor"];
            
            
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file to upload.");
                return Page();
            }

            // Retrieve the connection string from appsettings.json
            //string connectionString = _configuration.GetConnectionString("AzureBlobStorage");
            string connectionString = string.Empty;
            connectionString = sBlobConnectionString;
            
            // Create a BlobServiceClient to interact with Blob service
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Replace 'your-container-name' with the name of your blob container
            string containerName = "earnings-data-tool";
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure the container exists
            await containerClient.CreateIfNotExistsAsync();

            // Generate a unique name for the file
            // string blobName = "OneTime/" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string blobName = $"_{General.RemovePunctuation(Surname)}-{General.RemovePunctuation(FirstName)}_{SelectedPayor}_{Path.GetFileName(file.FileName)}";
            
            // Get a reference to the blob
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            // Upload the file to the blob
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            UploadSuccess = true;
            return Page();
        }
        
       
        
        
    }
}