using InputKit.Shared.Validations;

namespace ControYaApp.Services.Validation
{
    public class IpAdressValidation : IValidation
    {
        public string Message { get; set; } = "Dirección IP no válida.";

        public bool Validate(object value)
        {
            const string pattern = @"^(([a-z0-9-]+\.)+[a-z]{2,6}|((25[0-5]|2[0-4][0-9]|[01]?[0-9]?[0-9])\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9]?[0-9]))(:(6553[0-5]|655[0-2]\\d|65[0-4]\\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3}))?(/[\w/.]*)?$";

            if (value is string ip)
            {
                var regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.Compiled);
                return regex.Match(ip).Success;
            }
            return false;
        }
    }
}
