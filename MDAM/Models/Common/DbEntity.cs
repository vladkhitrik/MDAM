using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace MDAM.Models.Common
{
    public class DbEntity<T>
    {
        [HiddenInput]        
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public T Id { get; set; }
    }
}
