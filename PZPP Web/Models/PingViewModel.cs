using PZPP_Web.Data_Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PZPP_Web.Models
{
    public class PingViewModel
    {
        public PingViewModel()
        {
            DropDown = new List<SelectListItem>();
            Selected = new List<string>();
        }

        [Required]
        [IP(ErrorMessage = "błędne ip")]
        [Display(Name = "IP")]
        public string IP { get; set; }

        
        [Display(Name = "Descriptions")]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> DropDown { get; set; }
        public IEnumerable<string> Selected { get; set; }
    }
}