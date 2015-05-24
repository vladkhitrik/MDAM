using System;
using System.ComponentModel.DataAnnotations;
using MDAM.Models.Attachments;
using MDAM.Models.Common;
using MDAM.Models.Dialogs;

namespace MDAM.Models.Messages
{
    public class Message : DbEntity<string>
    {
        [Display(Name = "Диалог")]
        public string DialogId { get; set; }

        [Display(Name = "Диалог")]
        public Dialog Dialog { get; set; }

        [Display(Name = "Отправитель")]
        public string SenderId { get; set; }

        [Display(Name = "Отправитель")]
        public ApplicationUser Sender { get; set; }

        [Display(Name = "Текст сообщения")]
        public string Text { get; set; }
        
        [Display(Name = "Файл")]
        public Attachment Attachment { get; set; }

        [Display(Name = "Дата отправки")]
        public DateTime DateTime { get; set; }
    }
}