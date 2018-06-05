using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PZPP_Web.Data_Validations
{
    public class IPAttribute :ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            string ip = Convert.ToString(value);
            string Pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            Regex check = new Regex(Pattern);
            if (string.IsNullOrEmpty(ip))
                return  false;
            else
                return check.IsMatch(ip);
           
        }
    }
}