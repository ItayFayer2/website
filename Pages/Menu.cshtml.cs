using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

[IgnoreAntiforgeryToken]
public class MenuModel : PageModel
{
    private readonly string connectionString = @"Server=ITAY_FAYER\SQLEXPRESS;Database=FinalPCProject;Trusted_Connection=True;TrustServerCertificate=True;";
    
    public bool IsLoggedIn { get; set; }

    public void OnGet()
    {

        string? user = HttpContext.Session.GetString("username");
        IsLoggedIn = user != null;
    }

    public IActionResult OnPostSubmitOrder([FromBody] OrderRequest data)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string query = "INSERT INTO Orders (Phone, Address, Items, TotalPrice) VALUES (@phone, @addr, @items, @total)";
            SqlCommand cmd = new SqlCommand(query, conn);
            

            cmd.Parameters.AddWithValue("@phone", data.Phone ?? "");
            cmd.Parameters.AddWithValue("@addr", data.Address ?? "");
            cmd.Parameters.AddWithValue("@items", data.Items ?? "");
            cmd.Parameters.AddWithValue("@total", data.Total);

            cmd.ExecuteNonQuery();
        }
        return new JsonResult(new { success = true });
    }

    public class OrderRequest
    {
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string Items { get; set; } = "";
        public int Total { get; set; }
    }
}