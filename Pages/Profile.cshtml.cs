using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace finalProject.Pages;

public class ProfileModel : PageModel
{
    [BindProperty] public string FullName { get; set; }
    [BindProperty] public string Email { get; set; }
    [BindProperty] public string Phone { get; set; }
    [BindProperty] public string Gender { get; set; }
    [BindProperty] public string BirthDate { get; set; }
    [BindProperty] public string Password { get; set; }
    
    public string Message { get; set; }

    string connString = @"Server=ITAY_FAYER\SQLEXPRESS;Database=FinalPCProject;Trusted_Connection=True;TrustServerCertificate=True;";

    public IActionResult OnGet()
    {
        string user = HttpContext.Session.GetString("username");
        if (string.IsNullOrEmpty(user)) return RedirectToPage("/Login");

        using (SqlConnection conn = new SqlConnection(connString))
        {
            conn.Open();


            string userQuery = "SELECT FullName, Email, Phone, Gender, BirthDate, Password FROM Users WHERE username = @user";
            using (SqlCommand cmd = new SqlCommand(userQuery, conn))
            {
                cmd.Parameters.AddWithValue("@user", user);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        FullName = reader["FullName"]?.ToString();
                        Email = reader["Email"]?.ToString();
                        Phone = reader["Phone"]?.ToString();
                        Gender = reader["Gender"]?.ToString();
                        if (reader["BirthDate"] != DBNull.Value)
                        {
                            BirthDate = Convert.ToDateTime(reader["BirthDate"]).ToString("yyyy-MM-dd");
                        }
                        Password = reader["Password"]?.ToString();
                    }
                }
            }
        }
        return Page();
    }

    public IActionResult OnPost()
{
    string user = HttpContext.Session.GetString("username");
    if (string.IsNullOrEmpty(user))
        return RedirectToPage("/Login");

    try
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            conn.Open();

            string query = @"UPDATE Users 
                             SET FullName = @name, 
                                 Email = @email, 
                                 Phone = @phone, 
                                 Gender = @gender, 
                                 BirthDate = @birth, 
                                 Password = @pass 
                             WHERE username = @user";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", FullName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@email", Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@gender", (object)Gender ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@birth", (object)BirthDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@pass", Password ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@user", user);

                cmd.ExecuteNonQuery();
            }
        }

        TempData["Updated"] = true;
    }
    catch
    {
        TempData["Updated"] = false;
    }

    return RedirectToPage();
}
}