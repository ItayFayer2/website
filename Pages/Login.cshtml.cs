// ספריה שמאפשרת להשתמש בפונקציות של MVC (לא חובה במקרה הזה אבל נפוצה בפרויקטים)
using Microsoft.AspNetCore.Mvc;

// ספריה שמאפשרת לעבוד עם Razor Pages (הדפים של האתר)
using Microsoft.AspNetCore.Mvc.RazorPages;

// ספריה שמאפשרת להתחבר למסד נתונים מסוג SQL Server
using Microsoft.Data.SqlClient;


// namespace הוא שם האזור של הקוד בפרויקט
// בדרך כלל זה שם הפרויקט + התיקייה
namespace finalProject.Pages;


// המחלקה שמכילה את הלוגיקה של דף Login
// השם חייב להיות שם הדף + Model
// כלומר עבור Login.cshtml המחלקה תהיה LoginModel
public class LoginModel : PageModel
{

    // משתנה שישמור את שם המשתמש שהוזן בטופס
    public string username;

    // משתנה שישמור את הסיסמה שהוזנה בטופס
    public string password;

    // משתנה שישמור הודעה למשתמש במקרה שההתחברות נכשלה
    public string message;


    // הפונקציה OnPost מופעלת כאשר המשתמש לוחץ על כפתור submit בטופס
    // כלומר כאשר הנתונים נשלחים מהדף לשרת
    public void OnPost()
    {

        // קבלת הנתונים מהטופס
        // חשוב ששמות ה-input ב-HTML יהיו זהים לשמות כאן

        // לדוגמה בדף Login.cshtml:
        // <input name="username">
        // <input name="password">

        username = Request.Form["username"];
        password = Request.Form["password"];


        // ----------------------------------------------------
        // חיבור למסד הנתונים
        // ----------------------------------------------------

        // connectionString מכיל את פרטי החיבור למסד הנתונים

        // Server = כתובת שרת ה-SQL
        // 1433 = הפורט של SQL Server (ברירת מחדל)

        // Database = שם מסד הנתונים
        // User Id = המשתמש של SQL Server
        // Password = הסיסמה של SQL Server
        // TrustServerCertificate=True מאפשר להתחבר בלי בדיקת תעודת SSL

string connectionString = @"Server=ITAY_FAYER\SQLEXPRESS;
Database=FinalPCProject;
Trusted_Connection=True;
TrustServerCertificate=True;";
        // עבור משתמשי ווינדוס
        // string connectionString = "Server=localhost\\SQLEXPRESS;Database=UserDB;Trusted_Connection=True;TrustServerCertificate=True;";


        // ----------------------------------------------------
        // יצירת חיבור למסד הנתונים
        // ----------------------------------------------------

        // SqlConnection הוא אובייקט שמייצג חיבור ל-SQL Server
        using SqlConnection conn = new SqlConnection(connectionString);

        // פתיחת החיבור למסד הנתונים
        conn.Open();


        // ----------------------------------------------------
        // כתיבת שאילתת SQL
        // ----------------------------------------------------

        // השאילתה מחפשת משתמש בטבלה Users
        // אם שם המשתמש והסיסמה קיימים בטבלה

        // אם נמצא משתמש – היא תחזיר את uName

        string query = "SELECT username FROM Users WHERE username = @username AND password = @password";


        // יצירת פקודת SQL
        using SqlCommand cmd = new SqlCommand(query, conn);


        // ----------------------------------------------------
        // הוספת פרמטרים לשאילתה
        // ----------------------------------------------------

        // שימוש בפרמטרים מגן מפני SQL Injection
        // ולכן לא מכניסים את המשתנים ישירות לתוך השאילתה

        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);


        // ----------------------------------------------------
        // ביצוע השאילתה
        // ----------------------------------------------------

        // ExecuteScalar מחזיר ערך אחד בלבד
        // במקרה הזה הוא יחזיר את uName אם המשתמש קיים

        string result = (string) cmd.ExecuteScalar();


        // ----------------------------------------------------
        // בדיקה אם נמצא משתמש
        // ----------------------------------------------------

        if (!string.IsNullOrEmpty(result))
        {

            // אם נמצא משתמש במסד הנתונים
            // סימן שההתחברות הצליחה

            Response.Redirect("/Index");

        }
        else
        {

     

            message = "שם המשתמש או הסיסמא אינם תקינים.";

        }
        
    }
}