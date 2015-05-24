using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MDAM.Models.Common;
using MDAM.Models.DialogTypes;
using MDAM.Models.Messages;

namespace MDAM.Models.Dialogs
{
    public class Dialog : DbEntity<string>
    {
        [Display(Name = "Создатель")]
        public string CreatorUserId { get; set; }

        [Display(Name = "Создатель")]
        public virtual ApplicationUser CreatorUser { get; set; }

        [Display(Name = "Тип")]
        public string DialogTypeId { get; set; }

        [Display(Name = "Тип")]
        public virtual DialogType DialogType { get; set; }
        
        [Display(Name = "Название")]
        public string Title { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        [Display(Name = "Участники")]
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}