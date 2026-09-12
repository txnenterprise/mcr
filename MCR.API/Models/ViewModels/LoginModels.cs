using System.ComponentModel.DataAnnotations;

namespace MCR.API.Models.ViewModels
{
    public class AuthenticationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Mensagem { get; set; }
        public string Adaptador { get; set; }
        public string ReturnUrl { get; set; }
        public bool Sucesso { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string Email { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }
}
