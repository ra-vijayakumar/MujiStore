using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text.RegularExpressions;
namespace MujiStore.BLL
{
    public class RegularExp
    {
    }
    public class RegularExpressionWithOptionsAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public RegularExpressionWithOptionsAttribute(string pattern) : base(pattern) { }

        public RegexOptions RegexOptions { get; set; }

        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty(value as string))
                return true;

            return Regex.IsMatch(value as string, "^" + Pattern + "$", RegexOptions);
        }

        public IEnumerable<System.Web.Mvc.ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "regexwithoptions"
            };

            rule.ValidationParameters["pattern"] = Pattern;

            string flags = "";
            if ((RegexOptions & RegexOptions.Multiline) == RegexOptions.Multiline)
                flags += "m";
            if ((RegexOptions & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase)
                flags += "i";
            rule.ValidationParameters["flags"] = flags;

            yield return rule;
        }
    }
    public class IgnorecaseRegularExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        public IgnorecaseRegularExpressionAttribute(string pattern) : base("(?i)" + pattern)
        { }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "icregex",
                ErrorMessage = ErrorMessage
            };
            // Remove the (?i) that we added in the pattern as this
            // is not necessary for the client validation
            rule.ValidationParameters.Add("pattern", Pattern.Substring(4));
            yield return rule;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class CustomExtn : ValidationAttribute
{
    private readonly string _DefaultErrorMessage = MujiStore.Resources.Resource.CustomExtnDefaultErrorMessage;
    private IEnumerable<string> _ValidTypes { get; set; }
    public string AllowedExtn { get; set; }

 

        public override bool IsValid(object value)
        {
            if(value == null)
            {
                return true;
            }
            else
            {
                HttpPostedFileBase uploadFile = value as HttpPostedFileBase;
                string extn = System.IO.Path.GetExtension(uploadFile.FileName); //.txt
                extn = extn.TrimStart('.').ToLower();
                return AllowedExtn.Contains(extn);
            }
           
            //return base.IsValid(value);
        }
  
}
}