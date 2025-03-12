using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Forms.uCaptcha.FieldTypes;
using Umbraco.Community.Forms.uCaptcha.Models;
using Umbraco.Forms.Core.Providers;

namespace Umbraco.Community.Forms.uCaptcha.Extensions
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AdduCaptcha(this IUmbracoBuilder builder)
        {
            builder.WithCollectionBuilder<FieldCollectionBuilder>().Add<uCaptchaFieldType>();
            builder.Services.Configure<uCaptchaSettings>((IConfiguration)builder.Config.GetSection(Constants.uCaptcha));
            return builder;
        }
    }
}
