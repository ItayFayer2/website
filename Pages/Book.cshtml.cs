using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class BookModel : PageModel
{
    public IActionResult OnPost()
    {
        string name = Request.Form["Name"];
        string date = Request.Form["Date"];
        string time = Request.Form["Time"];
        string people = Request.Form["People"];


        if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time))
        {
            TempData["Success"] = false;
            return RedirectToPage();
        }

        DateTime bookingDate;

        bool valid = DateTime.TryParse($"{date} {time}", out bookingDate);

        // Stop bad dates for SQL
        if (!valid || bookingDate.Year < 1753)
        {
            TempData["Success"] = false;
            return RedirectToPage();
        }

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
        cmd.Parameters.AddWithValue("@People", int.Parse(people));

        cmd.ExecuteNonQuery();



        TempData["Success"] = true;
        TempData["Date"] = bookingDate.ToString("dd/MM/yyyy");
        TempData["Time"] = bookingDate.ToString("HH:mm");
        TempData["People"] = people;

        return RedirectToPage();
    }
}