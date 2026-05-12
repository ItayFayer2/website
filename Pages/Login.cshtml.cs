using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace finalProject.Pages;

public class LoginModel : PageModel
{
    public string username;
    public string password;
    public string message;

    public void OnPost()
    {
        username = Request.Form["username"];
        password = Request.Form["password"];

        string connectionString = @"Server=ITAY_FAYER\SQLEXPRESS;
Database=FinalPCProject;
Trusted_Connection=True;
TrustServerCertificate=True;";

        using SqlConnection conn = new SqlConnection(connectionString);
        conn.Open();

        string query = "SELECT username, Role FROM Users WHERE username = @username AND password = @password";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);

        using SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            string user = reader["username"].ToString();
            string role = reader["Role"]?.ToString() ?? "User";

            HttpContext.Session.SetString("username", user);
            HttpContext.Session.SetString("role", role);

            Response.Redirect("/Index");
        }
        else
        {
            message = "שם המשתמש או הסיסמא אינם תקינים.";
        }
    }
}