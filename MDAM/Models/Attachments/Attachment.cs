using System.ComponentModel.DataAnnotations;
using MDAM.Models.Messages;

namespace MDAM.Models.Attachments
{
    public class Attachment
    {
        [Display(Name = "Файл")]
        public string AttachmentId { get; set; }

        [Display(Name = "Путь")]
        public string Path { get; set; }

        [Display(Name = "Размер")]
        public string Size { get; set; }

        [Display(Name = "Имя файла")]
        public string FileName { get; set; }

        public virtual Message Message { get; set; }
    }
}