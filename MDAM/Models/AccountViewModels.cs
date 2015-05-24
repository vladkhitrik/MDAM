using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MDAM.Models
{
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
    public class UserViewModel
    {
        [Display(Name = "Email адрес")]
        public string Email { get; set; }

        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Фотография")]
        public string Image { get; set; }
    }
}
