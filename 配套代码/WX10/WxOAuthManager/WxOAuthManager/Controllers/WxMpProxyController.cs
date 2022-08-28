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
        private readonly IConfiguration configuration;

        public WxMpProxyController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("/connect/oauth2/authorize")]
        public IActionResult authorize(string redirect_uri, string scope, string state)
        {
            return RedirectToAction("ProxyAuthorize", "WxOAuth2", new { redirect_uri, scope, state });
        }

        [HttpGet]
        [Route("/sns/oauth2/access_token")]
        public string access_token(string code)
        {
            var Code = code;
            var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={configuration["WeChatMp:AppId"]}&secret={configuration["WeChatMp:AppSecret"]}&code={Code}&grant_type=authorization_code";
            var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync(url).Result;
            return result;
        }
    }
}
