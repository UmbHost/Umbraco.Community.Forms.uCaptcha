using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Umbraco.Community.Forms.uCaptcha.Enums;
using Umbraco.Community.Forms.uCaptcha.Models;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;

namespace Umbraco.Community.Forms.uCaptcha.FieldTypes
{
    public sealed class uCaptchaFieldType : FieldType
    {
        private readonly uCaptchaSettings _config;
        private readonly ILogger<uCaptchaFieldType> _logger;
        private bool ishCaptcha;
        private bool isreCaptcha;
        private bool isTurnstile;

        public uCaptchaFieldType(IOptionsMonitor<uCaptchaSettings> config, ILogger<uCaptchaFieldType> logger)
        {
            _config = config.CurrentValue;
            _logger = logger;
            Id = new Guid("76fc6a38-4517-4fea-b928-9ff20c626adb");
            Name = "uCaptcha";
            Description = "hCaptcha or Google reCaptcha bot protection";
            Icon = "icon-eye";
            DataType = FieldDataType.Bit;
            SortOrder = 10;
            SupportsRegex = false;
            ishCaptcha = _config.Provider.Equals(Provider.Name.hCaptcha.ToString(), StringComparison.InvariantCultureIgnoreCase);
            isreCaptcha = _config.Provider.Equals(Provider.Name.reCaptcha.ToString(), StringComparison.InvariantCultureIgnoreCase);
            isTurnstile = _config.Provider.Equals(Provider.Name.Turnstile.ToString(), StringComparison.InvariantCultureIgnoreCase);

            FieldTypeViewName = "FieldType.uCaptcha.cshtml";
            PreviewView = "uCaptcha.Field.Preview";
            MandatoryByDefault = true;
            HideLabel = true;
        }

        [Setting("Theme", Description = "uCaptcha theme", PreValues = "dark,light",
            View = "Umb.PropertyEditorUi.Dropdown")]
        public string Theme { get; set; }

        [Setting("Size", Description = "uCaptcha size", PreValues = "normal,compact,invisible",
            View = "Umb.PropertyEditorUi.Dropdown")]
        public string Size { get; set; }

        [Setting("reCaptcha Badge Position", Description = "Reposition the reCAPTCHA badge",
            PreValues = "bottomright,bottomleft,inline",
            View = "Umb.PropertyEditorUi.Dropdown")]
        public string reCaptchaBadgePosition { get; set; }

        [Setting("Error Message",
            Description =
                "The error message to display when the user does not pass the uCaptcha check, the default message is: \"You must check the \"I am human\" checkbox to continue\"",
            View = "Umb.PropertyEditorUi.TextBox")]
        public string ErrorMessage { get; set; }

        public override IEnumerable<string> RequiredJavascriptFiles(Field field)
        {
            var javascriptFiles = base.RequiredJavascriptFiles(field).ToList();
            if (isTurnstile)
            {
                javascriptFiles.Add(Constants.Turnstile.JsResource);
                javascriptFiles.Add($"{Constants.AssetsPath}{Constants.Turnstile.LocalJsResource}");
            }
            else
            {
                if (field.Settings.TryGetValue("Size", out var size) && size.Equals("invisible"))
                {
                    if (ishCaptcha)
                    {
                        javascriptFiles.Add(Constants.hCaptcha.JsResource);
                        javascriptFiles.Add(
                            $"{Constants.AssetsPath}{Constants.hCaptcha.LocalInvisibleJsResource}");
                    }
                    else if (isreCaptcha)
                    {
                        javascriptFiles.Add(Constants.reCaptcha.JsResource);
                        javascriptFiles.Add(
                            $"{Constants.AssetsPath}{Constants.reCaptcha.LocalInvisibleJsResource}");
                    }
                }
                else
                {
                    if (ishCaptcha)
                    {
                        javascriptFiles.Add(Constants.hCaptcha.JsResource);
                        javascriptFiles.Add($"{Constants.AssetsPath}{Constants.hCaptcha.LocalJsResource}");
                    }
                    else if (isreCaptcha)
                    {
                        javascriptFiles.Add(Constants.reCaptcha.JsResource);
                        javascriptFiles.Add($"{Constants.AssetsPath}{Constants.reCaptcha.LocalJsResource}");
                    }
                }
            }

            return javascriptFiles;
        }

        public override IEnumerable<string> ValidateField(Form form, Field field, IEnumerable<object> postedValues, HttpContext context,
            IPlaceholderParsingService placeholderParsingService, IFieldTypeStorage fieldTypeStorage, List<string> errors)
        {
            if (field.Values.Contains("false"))
            {
                string errorMessage;
                if (field.Settings.ContainsKey("ErrorMessage") && !string.IsNullOrEmpty(field.Settings["ErrorMessage"]))
                {
                    errorMessage = field.Settings["ErrorMessage"];
                }
                else
                {
                    errorMessage = "You must check the \"I am human\" checkbox to continue";
                }

                return new List<string> { errorMessage };
            }

            errors.AddRange(ValidateFieldWithCaptcha(field, context, errors));

            if (!errors.Any())
            {
                return base.ValidateField(form, field, postedValues, context, placeholderParsingService, fieldTypeStorage, errors);
            }

            return errors;
        }

        private List<string> ValidateFieldWithCaptcha(Field field, HttpContext context, List<string> errors)
        {
            if (!errors.Any())
            {
                if (string.IsNullOrWhiteSpace(_config.SecretKey))
                {
                    string message =
                        "ERROR: uCaptcha is missing the Secret Key.  Please update the configuration to include a value at: " +
                        Constants.uCaptcha + ":SecretKey'";
                    _logger.LogWarning(message);
                    errors.Add(message);
                }

                string verifyUrl = null;
                string verifyPostParameter = null;
                if (ishCaptcha)
                {
                    verifyUrl = Constants.hCaptcha.VerifyUrl;
                    verifyPostParameter = Constants.hCaptcha.VerifyPostParameter;
                }
                else if (isreCaptcha)
                {
                    verifyUrl = Constants.reCaptcha.VerifyUrl;
                    verifyPostParameter = Constants.reCaptcha.VerifyPostParameter;
                }
                else if (isTurnstile)
                {
                    verifyUrl = Constants.Turnstile.VerifyUrl;
                    verifyPostParameter = Constants.Turnstile.VerifyPostParameter;
                }

                if (verifyUrl == null)
                {
                    throw new Exception("\"uCaptchaProvider\" is missing or incorrect in AppSettings.");
                }

                string errorMessage;
                if (field.Settings.ContainsKey("ErrorMessage") && !string.IsNullOrEmpty(field.Settings["ErrorMessage"]))
                {
                    errorMessage = field.Settings["ErrorMessage"];
                }
                else
                {
                    errorMessage = "You must check the \"I am human\" checkbox to continue";
                }

                var secretKey = _config.SecretKey;

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.Add("Accept", "*/*");

                var parameters = new List<KeyValuePair<string, string>>
            {
                new("response", context.Request.Form[verifyPostParameter]),
                new("secret", secretKey)
            };

                var remoteIpAddress = context.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
                if (remoteIpAddress != null)
                {
                    parameters.Add(new KeyValuePair<string, string>("remoteip", remoteIpAddress));
                }

                var request = new HttpRequestMessage(HttpMethod.Post, verifyUrl)
                {
                    Content = new FormUrlEncodedContent(parameters)
                };

                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync();
                    jsonString.Wait();

                    var result = JsonConvert.DeserializeObject<uCaptchaVerifyResponse>(jsonString.Result);
                    if (result is { Success: false })
                    {
                        errors.Add(errorMessage);
                    }
                }

                if (!response.IsSuccessStatusCode)
                {
                    errors.Add(errorMessage);
                }
            }

            return errors;
        }
    }
}
