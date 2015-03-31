using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MDAM.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Код")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Запомнить этот браузер?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [Display(Name = "Пользователь")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [Display(Name = "Пользователь")]
        [System.Web.Mvc.Remote("CheckUserName", "Account", ErrorMessage = "Пользователь с таким именем уже существует.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [EmailAddress(ErrorMessage = "Поле {0} содержит не действующий адрес электронной почты.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [StringLength(100, ErrorMessage = "{0} должен быть длиной не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [EmailAddress(ErrorMessage = "Поле {0} содержит не действующий адрес электронной почты.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [StringLength(100, ErrorMessage = "{0} должен быть длиной не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [EmailAddress(ErrorMessage = "Поле {0} содержит не действующий адрес электронной почты.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
