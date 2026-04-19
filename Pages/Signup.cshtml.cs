using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace RestaurantProject.Pages
{
    // מחלקת צד השרת של דף ההרשמה.
    // תפקידה: לקרוא את הנתונים מהטופס, לבדוק אם המשתמש כבר קיים,
    // ואם לא - להוסיף אותו למסד הנתונים.
    public class SignupModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public string Message = "";

        public SignupModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            // קריאת הערכים מהטופס
            string fullName = Request.Form["fullName"];
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string email = Request.Form["email"];
            string phone = Request.Form["phone"];

string connectionString = @"Server=ITAY_FAYER\SQLEXPRESS;
Database=FinalPCProject;
Trusted_Connection=True;
TrustServerCertificate=True;";
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            // בדיקה האם שם המשתמש כבר קיים
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            using SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@Username", username);

            int count = (int)checkCmd.ExecuteScalar();

            if (count > 0)
            {
                Message = "Username already exists.";
                return;
            }

            // הוספת משתמש חדש
            string insertQuery = @"INSERT INTO Users
                                   (Username, Password, FullName, Email, Phone, Role)
                                   VALUES
                                   (@Username, @Password, @FullName, @Email, @Phone, 'User')";

            using SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.Parameters.AddWithValue("@FullName", fullName);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Phone", phone);

            cmd.ExecuteNonQuery();
            Message = "Registration completed successfully.";
        }
    }
}