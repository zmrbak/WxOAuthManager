using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Web;
using WxOAuthManager.Models;

namespace WxOAuthManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WxOAuth2Controller : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        public WxOAuth2Controller(IConfiguration configuration,IMemoryCache memoryCache)
        {
            this.configuration = configuration;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("/api/proxyauthorize")]
        public IActionResult ProxyAuthorize(string redirect_uri, string scope, string state)
        {
            var code = Guid.NewGuid().ToString().Replace("-", "");
            memoryCache.Set(code, new ProxyData { RedirectUri=redirect_uri, State= state }, TimeSpan.FromSeconds(61));

            var data = this.Url;
            var isHttps = data.ActionContext.HttpContext.Request.IsHttps;
            var host = data.ActionContext.HttpContext.Request.Host.Host;
            var port = data.ActionContext.HttpContext.Request.Host.Port;

            var hostString = "";
            if(isHttps==true)
            {
                if(port == 443)
                {
                    port = 0;
                }
                hostString = "https://";
            }
            else
            {
                if (port == 80)
                {
                    port = 0;
                }
                hostString = "http://";
            }

            hostString += host;
            if(port!=0)
            {
                hostString += ":"+port;
            }

            var Scope = scope;
            var State = code;
            var url = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={configuration["WeChatMp:AppId"]}&redirect_uri={HttpUtility.UrlEncode(hostString + configuration["WeChatMp:RedirectUri"])}&response_type=code&scope={Scope}&state={State}#wechat_redirect";
            return Redirect(url);
        }

        [HttpGet]
        public IActionResult Get(string code, string state)
        {
            if (memoryCache.TryGetValue(state, out ProxyData proxyData))
            {
                return Redirect(proxyData.RedirectUri + $"?code={code}&state={proxyData.State}");
            }
            return NoContent();
        }

    }


   

}
