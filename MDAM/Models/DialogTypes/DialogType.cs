using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MDAM.Models.Common;
using MDAM.Models.Dialogs;
using System;

namespace MDAM.Models.DialogTypes
{
    public class DialogType : DbEntity<string>
    {
        public DialogType()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        [Display(Name = "Название")]
        [Required(ErrorMessage="Поле {0} должно быть определено")]
        public string Title { get; set; }

        public ICollection<Dialog> Dialogs { get; set; }
    }
}