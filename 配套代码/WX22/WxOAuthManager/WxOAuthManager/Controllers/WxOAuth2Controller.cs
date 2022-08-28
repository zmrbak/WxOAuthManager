using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly ILogger<WxOAuth2Controller> logger;
        private readonly IWebHostEnvironment environment;

        public WxOAuth2Controller(
            IConfiguration configuration,
            IMemoryCache memoryCache,
            ILogger<WxOAuth2Controller> logger,
            IWebHostEnvironment environment)
        {
            this.configuration = configuration;
            this.memoryCache = memoryCache;
            this.logger = logger;
            this.environment = environment;
        }

        [HttpGet]
        [Route("/connect/oauth2/qrconnect")]
        public IActionResult ProxyQrconnect(string redirect_uri, string scope, string state)
        {
            var stateChache = Guid.NewGuid().ToString().Replace("-", "");
            memoryCache.Set(stateChache, new ProxyData { RedirectUri = redirect_uri, State = state, IsQrCode = true }, TimeSpan.FromSeconds(60));

            var appid = configuration["WeChatScan:AppId"];
            var redirectUri = UrlWithoutPath() + "/connect/oauth2/callback";
            logger.LogInformation("redirectUri:\t" + redirectUri);

            var url = $"https://open.weixin.qq.com/connect/qrconnect?appid={appid}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}&response_type=code&scope={scope}&state={stateChache}#wechat_redirect";
            return Redirect(url);
        }

        [HttpGet]
        [Route("/connect/oauth2/authorize")]
        public IActionResult ProxyAuthorize(string redirect_uri, string scope, string state)
        {
            var stateChache = Guid.NewGuid().ToString().Replace("-", "");
            memoryCache.Set(stateChache, new ProxyData { RedirectUri = redirect_uri, State = state }, TimeSpan.FromSeconds(60));

            var appid = configuration["WeChatMp:AppId"];
            var redirectUri = UrlWithoutPath() + "/connect/oauth2/callback";
            logger.LogInformation("redirectUri:\t" + redirectUri);

            var url = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={appid}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}&response_type=code&scope={scope}&state={stateChache}#wechat_redirect";
            return Redirect(url);
        }

        [HttpGet]
        [Route("/connect/oauth2/callback")]
        public IActionResult ProxyCallback(string code, string state)
        {
            if (memoryCache.TryGetValue(state, out ProxyData proxyData))
            {
                memoryCache.Set(code, proxyData, TimeSpan.FromSeconds(60));
                return Redirect(proxyData.RedirectUri + $"?code={code}&state={proxyData.State}");
            }
            return NoContent();
        }

        [HttpGet]
        [Route("/connect/oauth2/access_token")]
        public string access_token(string code)
        {
            var appid = "";
            var secret = "";

            if (memoryCache.TryGetValue(code, out ProxyData proxyData))
            {
                if (proxyData.IsQrCode)
                {
                    appid = configuration["WeChatScan:AppId"];
                    secret = configuration["WeChatScan:AppSecret"];
                }
                else
                {
                    appid = configuration["WeChatMp:AppId"];
                    secret = configuration["WeChatMp:AppSecret"];
                }
            }

            var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={appid}&secret={secret}&code={code}&grant_type=authorization_code";
            var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync(url).Result;
            return result;
        }

        private string UrlWithoutPath()
        {
            var request = this.Request;
            var host = request.Host.Host;
            var port = request.Host.Port;
            var scheme = request.Scheme;
            if (environment.IsDevelopment())
            {
                if (scheme == "http")
                {
                    if (port == 80)
                    {
                        port = 0;
                    }
                }
                else
                {
                    if (port == 443)
                    {
                        port = 0;
                    }
                }
            }
            else
            {
                scheme = "https";
            }
            var redirect_uri = scheme + "://" + host;
            if (port != null && port != 0)
            {
                logger.LogInformation("port:\t" + port);
                redirect_uri += ":" + port;
            }


            return redirect_uri;
        }
    }
}
