namespace EMPLOYEE.Setting
{
    public class EmailSettings
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 465;
        public bool EnableSSL { get; set; } = true;
    }
}
