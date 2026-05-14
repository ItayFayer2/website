using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kk.Pages;

public class ContactModel : PageModel
{
    public bool ShowAlert { get; set; }

    public void OnGet()
    {

    }

    public void OnPost()
    {
        ShowAlert = true;
    }
}