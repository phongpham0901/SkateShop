using System.ComponentModel.DataAnnotations;

namespace SkateShop.CustomValidation
{
    public class NotAdminAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string str && str.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                ErrorMessage = "The Last Name field cannot be 'Admin'";
                return false;
            }
            return true;
        }
    }
}
