using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace RestaurantProject.Pages
{
    public class SignupModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string Message = "";

        public SignupModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet() { }

        public void OnPost()
        {
            // 1. קריאת הערכים מהטופס (כולל השדות החדשים)
            string fullName = Request.Form["fullName"];
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string email = Request.Form["email"];
            string phone = Request.Form["phone"];
            string gender = Request.Form["gender"]; // שדה חדש
            string birthDate = Request.Form["birthDate"]; // שדה חדש

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

            // 2. עדכון שאילתת ההוספה - הוספנו Gender ו-BirthDate
            string insertQuery = @"INSERT INTO Users 
                                   (Username, Password, FullName, Email, Phone, Gender, BirthDate, Role) 
                                   VALUES 
                                   (@Username, @Password, @FullName, @Email, @Phone, @Gender, @BirthDate, 'User')";

            using SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.Parameters.AddWithValue("@FullName", fullName);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Phone", phone);
            
            // שימוש ב-DBNull אם המשתמש לא מילא את השדה (למניעת שגיאות)
            cmd.Parameters.AddWithValue("@Gender", (object)gender ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BirthDate", (object)birthDate ?? DBNull.Value);

            cmd.ExecuteNonQuery();
            Message = "Registration completed successfully.";
        }
    }
}