using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Web;
using WxOAuthManager.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WxOAuthManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WxMpProxyController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;

        public WxMpProxyController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("/connect/oauth2/authorize")]
        public IActionResult authorize(string redirect_uri, string scope, string state)
        {
            var code = Guid.NewGuid().ToString().Replace("-", "");
            memoryCache.Set(code, DateTime.Now, TimeSpan.FromSeconds(5));

            return Redirect(HttpUtility.UrlDecode(redirect_uri) + "?code=" + code + "&state=" + state);
        }

        [HttpGet]
        [Route("/sns/oauth2/access_token")]
        public WebAccessToken access_token(string code)
        {
            if (memoryCache.TryGetValue(code, out DateTime dateTime) == true)
            {
                return new WebAccessToken
                {
                    access_token = dateTime.ToString(),
                    expires_in = 7200,
                    openid = "open_id",
                    refresh_token = "refresh_token",
                    scope = "scope"
                };
            }

            return new WebAccessToken
            {
                access_token = "access_token",
                expires_in = 7200,
                openid = "open_id",
                refresh_token = "refresh_token",
                scope = "scope"
            };

        }
    }
}
