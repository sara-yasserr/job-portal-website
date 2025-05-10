using System.ComponentModel.DataAnnotations;

namespace Job_Portal_Project.Attributes
{
    public class MustBeTrueAttribute :ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is bool b && b;
        }
    }
}
