using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace razor_data_tools.Pages.Tools
{
	public class PayorSelectorModel : PageModel
    {
        [BindProperty]
        public string SelectedOption { get; set; }

        [BindProperty]
        public string SelectedSubOption { get; set; }

        [BindProperty]
        public string TextBox1Value { get; set; }

        [BindProperty]
        public string TextBox2Value { get; set; }

        // Property to hold dropdown options
        public List<SelectListItem> Options { get; set; }

        public void OnGet()
        {
            // Simulated query results
            var queryResults = new List<string> { "BMI", "ASCAP", "Tunecore" };

            // Populate dropdown options
            Options = new List<SelectListItem>();
            foreach (var result in queryResults)
            {
                Options.Add(new SelectListItem { Text = result, Value = result });
            }
        }


        public IActionResult OnPost()
        {
            // Here you can access the values of the dropdown box and text boxes
            // and perform whatever action you need with them.
            // For this example, let's just return to the same page.
            return Page();
        }



        public IActionResult OnGetGetSubOptions(string selectedOption)
        {
            // Simulated query results based on selectedOption
            var subOptions = new List<string> { selectedOption + " SubOption 1", selectedOption + " SubOption 2", selectedOption + " SubOption 3" };

            var result = new List<SelectListItem>();
            foreach (var subOption in subOptions)
            {
                result.Add(new SelectListItem { Text = subOption, Value = subOption });
            }

            return new JsonResult(result);
        }



    }
}
