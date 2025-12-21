namespace Kutuphane.WebUI.Services.EmailService.Dto
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; } // Linkten gelecek gizli anahtar
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
