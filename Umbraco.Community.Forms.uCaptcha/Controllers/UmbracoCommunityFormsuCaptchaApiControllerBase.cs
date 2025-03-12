using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace Umbraco.Community.Forms.uCaptcha.Controllers
{
    [ApiController]
    [BackOfficeRoute("umbracocommunityformsucaptcha/api/v{version:apiVersion}")]
    [Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
    [MapToApi(Constants.ApiName)]
    public class UmbracoCommunityFormsuCaptchaApiControllerBase : ControllerBase
    {
    }
}
