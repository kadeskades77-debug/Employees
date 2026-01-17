namespace EMPLOYEE.Application.Dtos
{
    public class ResetPasswordOtpDto
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public string NewPassword { get; set; }
    }
}
