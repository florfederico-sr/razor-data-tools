using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace razor_data_tools.Pages
{
    public class TestPageModel : PageModel
    {
        [BindProperty, Required] public string Greeting { get; set; } // Required
        [BindProperty, Required] public string Country { get; set; } // <-- Add this
        [BindProperty, Required] public string State { get; set; } // <-- Add this

        public string Message { get; set; } // Not model bound

        public void OnGet() { }

        public void OnPost()
        {
            if (ModelState.IsValid)
            {
                Message = Message = $"{Greeting} from {State}, {Country}"; // <-- Updated
            }
        }
    }
}