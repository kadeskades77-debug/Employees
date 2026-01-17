namespace EMPLOYEE.Models
{
    public class PasswordResetOtp
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }

}
