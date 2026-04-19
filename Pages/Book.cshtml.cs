using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class BookModel : PageModel
{
    public string Message = "";

    public void OnPost()
{
    string name = Request.Form["Name"];
    string date = Request.Form["Date"];
    string time = Request.Form["Time"];
    string people = Request.Form["People"];

    string bookingDate = date + " " + time; // combine safely

    string connectionString = @"Server=ITAY_FAYER\SQLEXPRESS;
Database=FinalPCProject;
Trusted_Connection=True;
TrustServerCertificate=True;";

    using SqlConnection conn = new SqlConnection(connectionString);
    conn.Open();

    string query = @"INSERT INTO Bookings (Name, BookingDate, People)
                     VALUES (@Name, @Date, @People)";

    using SqlCommand cmd = new SqlCommand(query, conn);

    cmd.Parameters.AddWithValue("@Name", name);
    cmd.Parameters.AddWithValue("@Date", bookingDate);
    cmd.Parameters.AddWithValue("@People", people);

    cmd.ExecuteNonQuery();
}
}