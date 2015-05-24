using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MDAM.Models.Common;
using System;

namespace MDAM.Models.Offices
{
    public class Office : DbEntity<string>
    {
        public Office()
        {
            this.Id = Guid.NewGuid().ToString(); // Генерация ID
        }
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Title { get; set; }

    }
}