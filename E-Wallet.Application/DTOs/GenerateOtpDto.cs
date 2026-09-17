namespace E_Wallet.Application.DTOs
{
    public class GenerateOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
