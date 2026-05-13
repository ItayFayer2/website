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

        public void OnGet()
        {
        }

        public void OnPost()
        {
            // קריאת הנתונים מהטופס
            string fullName = Request.Form["fullName"];
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string email = Request.Form["email"];
            string phone = Request.Form["phone"];
            string gender = Request.Form["gender"];
            string birthDate = Request.Form["birthDate"];

            // בדיקה שכל השדות מולאו
            if (string.IsNullOrWhiteSpace(fullName))
            {
                Message = "יש למלא שם מלא.";
                return;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                Message = "יש למלא שם משתמש.";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                Message = "יש למלא סיסמה.";
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                Message = "יש למלא כתובת אימייל.";
                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                Message = "יש למלא מספר טלפון.";
                return;
            }

            if (string.IsNullOrWhiteSpace(gender))
            {
                Message = "יש לבחור מגדר.";
                return;
            }

            if (string.IsNullOrWhiteSpace(birthDate))
            {
                Message = "יש לבחור תאריך לידה.";
                return;
            }

            string connectionString = @"Server=ITAY_FAYER\SQLEXPRESS;
Database=FinalPCProject;
Trusted_Connection=True;
TrustServerCertificate=True;";

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            // בדיקה אם שם המשתמש קיים
            string usernameQuery =
                "SELECT COUNT(*) FROM Users WHERE Username = @Username";

            using (SqlCommand cmd = new SqlCommand(usernameQuery, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);

                if ((int)cmd.ExecuteScalar() > 0)
                {
                    Message = "שם המשתמש כבר קיים.";
                    return;
                }
            }

            // בדיקה אם השם המלא קיים
            string fullNameQuery =
                "SELECT COUNT(*) FROM Users WHERE FullName = @FullName";

            using (SqlCommand cmd = new SqlCommand(fullNameQuery, conn))
            {
                cmd.Parameters.AddWithValue("@FullName", fullName);

                if ((int)cmd.ExecuteScalar() > 0)
                {
                    Message = "השם המלא כבר קיים.";
                    return;
                }
            }

            // בדיקה אם האימייל קיים
            string emailQuery =
                "SELECT COUNT(*) FROM Users WHERE Email = @Email";

            using (SqlCommand cmd = new SqlCommand(emailQuery, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);

                if ((int)cmd.ExecuteScalar() > 0)
                {
                    Message = "כתובת האימייל כבר קיימת.";
                    return;
                }
            }

            // בדיקה אם מספר הטלפון קיים
            string phoneQuery =
                "SELECT COUNT(*) FROM Users WHERE Phone = @Phone";

            using (SqlCommand cmd = new SqlCommand(phoneQuery, conn))
            {
                cmd.Parameters.AddWithValue("@Phone", phone);

                if ((int)cmd.ExecuteScalar() > 0)
                {
                    Message = "מספר הטלפון כבר קיים.";
                    return;
                }
            }

            // הוספת משתמש חדש
            string insertQuery = @"
                INSERT INTO Users
                (
                    Username,
                    Password,
                    FullName,
                    Email,
                    Phone,
                    Gender,
                    BirthDate,
                    Role
                )
                VALUES
                (
                    @Username,
                    @Password,
                    @FullName,
                    @Email,
                    @Phone,
                    @Gender,
                    @BirthDate,
                    'User'
                )";

            using SqlCommand insertCmd =
                new SqlCommand(insertQuery, conn);

            insertCmd.Parameters.AddWithValue("@Username", username);
            insertCmd.Parameters.AddWithValue("@Password", password);
            insertCmd.Parameters.AddWithValue("@FullName", fullName);
            insertCmd.Parameters.AddWithValue("@Email", email);
            insertCmd.Parameters.AddWithValue("@Phone", phone);
            insertCmd.Parameters.AddWithValue("@Gender", gender);
            insertCmd.Parameters.AddWithValue("@BirthDate", birthDate);

            insertCmd.ExecuteNonQuery();

            Message = "ההרשמה הושלמה בהצלחה.";
        }
    }
}