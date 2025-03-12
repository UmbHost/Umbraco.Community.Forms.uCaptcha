using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Forms.uCaptcha.Extensions;
using Umbraco.Community.Forms.uCaptcha.Models;

namespace Umbraco.Community.Forms.uCaptcha.Composers
{
    public class uCaptchaRegisterComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<uCaptchaSettings>();
            builder.AdduCaptcha();
        }
    }
}
