using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MDAM.Models.Common;
using System;

namespace MDAM.Models.Boards
{
    public class Board : DbEntity<string>
    {
         public Board()
        {
            this.Id = Guid.NewGuid().ToString(); // Генерация ID
        }
        [Display(Name = "Создатель")]
        public string CreatorUserId { get; set; }

        [Display(Name = "Создатель")]
        public virtual ApplicationUser CreatorUser { get; set; }

        [Display(Name = "Текст")]
        public string Text { get; set; }

        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Display(Name = "Статус")]
        public string Status { get; set; }

        [Display(Name = "Дата подачи")]
        public DateTime Date { get; set; }
    }
}