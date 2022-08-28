using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace WxOAuthManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WxOAuth2Controller : ControllerBase
    {
        private readonly IConfiguration configuration;

        public WxOAuth2Controller(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("/WeChatLogin.aspx")]
        public IActionResult WeChatLogin()
        {
            string redirect_uri = UrlWithoutPath()+ "/LoginCallback.aspx";

            var scope = configuration["WeChatMpProxy:Scope"];
            var state = configuration["WeChatMpProxy:State"];
            var proxy = configuration["WeChatMpProxy:Proxy"];
            var url = proxy + $"/connect/oauth2/authorize?redirect_uri={HttpUtility.UrlEncode(redirect_uri)}&scope={scope}&state={state}";
            return Redirect(url);
        }

        private string UrlWithoutPath()
        {
            var request = this.Request;
            var host = request.Host.Host;
            var port = request.Host.Port;
            var scheme = request.Scheme;
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
            var redirect_uri = scheme + "://" + host;
            if (port != 0)
            {
                redirect_uri += ":" + port;
            }

            return redirect_uri;
        }

        [HttpGet]
        [Route("/LoginCallback.aspx")]
        public string LoginCallback(string code,string state)
        {
            var proxy = configuration["WeChatMpProxy:Proxy"];
            var url = proxy + $"/connect/oauth2/access_token?code={code}";
            var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync(url).Result;
            return "APP:<br>"+result;
        }
    }
}
