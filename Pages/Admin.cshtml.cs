using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class AdminModel : PageModel
{
    public List<User> Users = new();
    public List<Booking> Bookings = new();
    public List<OrderEntry> MenuOrders = new(); 

    [BindProperty(SupportsGet = true)]
    public string Search { get; set; }

    string connectionString = @"Server=ITAY_FAYER\SQLEXPRESS;
Database=FinalPCProject;
Trusted_Connection=True;
TrustServerCertificate=True;";

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

    public class OrderEntry 
    {
        public int Id { get; set; } 
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Items { get; set; }
    }

    public IActionResult OnGet()
    {
        string username = HttpContext.Session.GetString("username");

        if (username == null)
            return RedirectToPage("/Login");

        using SqlConnection conn = new(connectionString);
        conn.Open();

        SqlCommand roleCmd = new(
            "SELECT Role FROM Users WHERE Username=@username", conn);

        roleCmd.Parameters.AddWithValue("@username", username);

        string role = roleCmd.ExecuteScalar()?.ToString();

        if (role != "Admin")
            return RedirectToPage("/Index");

        // Get users
        string query = "SELECT * FROM Users";

        if (!string.IsNullOrEmpty(Search))
            query += " WHERE Username LIKE @search OR FullName LIKE @search";

        SqlCommand cmd = new(query, conn);

        if (!string.IsNullOrEmpty(Search))
            cmd.Parameters.AddWithValue("@search", "%" + Search + "%");

        SqlDataReader reader = cmd.ExecuteReader();

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

        reader.Close();

        SqlCommand bookingCmd = new(
            "SELECT * FROM Bookings", conn);

        SqlDataReader bookingReader = bookingCmd.ExecuteReader();

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
        bookingReader.Close();

        SqlCommand orderCmd = new("SELECT Id, Phone, Address, Items FROM Orders", conn);
        SqlDataReader orderReader = orderCmd.ExecuteReader();

        while (orderReader.Read())
        {
            MenuOrders.Add(new OrderEntry
            {
                Id = (int)orderReader["Id"],
                Phone = orderReader["Phone"].ToString(),
                Address = orderReader["Address"].ToString(),
                Items = orderReader["Items"].ToString()
            });
        }
        orderReader.Close();

        return Page();
    }

    public IActionResult OnPostDelete()
    {
        string username = Request.Form["username"];

        using SqlConnection conn = new(connectionString);
        conn.Open();

        SqlCommand cmd = new(
            "DELETE FROM Users WHERE Username=@username", conn);

        cmd.Parameters.AddWithValue("@username", username);
        cmd.ExecuteNonQuery();

        return RedirectToPage();
    }

    public IActionResult OnPostDeleteBooking()
    {
        string id = Request.Form["id"];

        using SqlConnection conn = new(connectionString);
        conn.Open();

        SqlCommand cmd = new(
            "DELETE FROM Bookings WHERE Id=@id", conn);

        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();

        return RedirectToPage();
    }

    public IActionResult OnPostDeleteMenuOrder()
    {
        string id = Request.Form["id"];

        using SqlConnection conn = new(connectionString);
        conn.Open();

        SqlCommand cmd = new("DELETE FROM Orders WHERE Id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();

        return RedirectToPage();
    }
}