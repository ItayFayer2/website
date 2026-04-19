using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class AdminModel : PageModel
{
    private readonly IConfiguration _configuration;

    public List<User> Users = new List<User>();
    public List<Booking> Bookings = new List<Booking>();

    [BindProperty(SupportsGet = true)]
    public string Search { get; set; }

    public AdminModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public class User
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }

    public class Booking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BookingDate { get; set; }
        public int People { get; set; }
    }

    public IActionResult OnGet()
    {
        string username = HttpContext.Session.GetString("username");

        if (username == null)
            return RedirectToPage("/Login");

string connectionString = @"Server=ITAY_FAYER\SQLEXPRESS;
Database=FinalPCProject;
Trusted_Connection=True;
TrustServerCertificate=True;";

        using SqlConnection conn = new SqlConnection(connectionString);
        conn.Open();

        // 🔒 check admin
        string roleQuery = "SELECT Role FROM Users WHERE username = @username";
        using SqlCommand roleCmd = new SqlCommand(roleQuery, conn);
        roleCmd.Parameters.AddWithValue("@username", username);

        string role = roleCmd.ExecuteScalar()?.ToString();

        if (role != "Admin")
            return RedirectToPage("/Index");

        // ---------------- USERS ----------------
        string query = "SELECT Username, FullName, Email, Phone, Role FROM Users";

        if (!string.IsNullOrEmpty(Search))
        {
            query += " WHERE Username LIKE @search OR FullName LIKE @search";
        }

        using SqlCommand cmd = new SqlCommand(query, conn);

        if (!string.IsNullOrEmpty(Search))
        {
            cmd.Parameters.AddWithValue("@search", "%" + Search + "%");
        }

        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Users.Add(new User
            {
                Username = reader["Username"].ToString(),
                FullName = reader["FullName"].ToString(),
                Email = reader["Email"].ToString(),
                Phone = reader["Phone"].ToString(),
                Role = reader["Role"].ToString()
            });
        }

        reader.Close(); // IMPORTANT before next query

        // ---------------- BOOKINGS ----------------
        string bookingQuery = "SELECT * FROM Bookings";

        using SqlCommand bookingCmd = new SqlCommand(bookingQuery, conn);
        using SqlDataReader bookingReader = bookingCmd.ExecuteReader();

        while (bookingReader.Read())
        {
            Bookings.Add(new Booking
            {
                Id = (int)bookingReader["Id"],
                Name = bookingReader["Name"].ToString(),
                BookingDate = bookingReader["BookingDate"].ToString(),
                People = (int)bookingReader["People"]
            });
        }

        return Page();
    }

    // ❌ DELETE USER
    public IActionResult OnPostDelete()
    {
        string username = Request.Form["username"];

string connectionString = @"Server=ITAY_FAYER\SQLEXPRESS;
Database=FinalPCProject;
Trusted_Connection=True;
TrustServerCertificate=True;";

        using SqlConnection conn = new SqlConnection(connectionString);
        conn.Open();

        string query = "DELETE FROM Users WHERE Username = @username";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@username", username);

        cmd.ExecuteNonQuery();

        return RedirectToPage();
    }
    public IActionResult OnPostDeleteBooking()
{
    string id = Request.Form["id"];

    string connectionString = @"Server=ITAY_FAYER\SQLEXPRESS;
Database=FinalPCProject;
Trusted_Connection=True;
TrustServerCertificate=True;";

    using SqlConnection conn = new SqlConnection(connectionString);
    conn.Open();

    string query = "DELETE FROM Bookings WHERE Id = @id";

    using SqlCommand cmd = new SqlCommand(query, conn);
    cmd.Parameters.AddWithValue("@id", id);

    cmd.ExecuteNonQuery();

    return RedirectToPage();
}
}