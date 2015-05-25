using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MDAM.Models
{
    public class NewViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Поле {0} должно быть определено.")]
        [Display(Name = "Текст")]
        public string Text { get; set; }
    }
}