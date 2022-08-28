using Microsoft.AspNetCore.Mvc;
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

        public WxOAuth2Controller(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("/login.aspx")]
        public IActionResult Get()
        {
            var Scope = "snsapi_base";
            var State = "123";
            var url = $"{configuration["WeChatMp:OAuth2Protocal"]}://{configuration["WeChatMp:OAuth2Host"]}:{configuration["WeChatMp:OAuth2Port"]}/connect/oauth2/authorize?redirect_uri={HttpUtility.UrlEncode(configuration["WeChatMp:RedirectUri"])}&scope={Scope}&state={State}";
            return Redirect(url);
        }

        [HttpGet]
        public string Get(string code,string state)
        {
            var Code = code;
            var url = $"{configuration["WeChatMp:OAuth2Protocal"]}://{configuration["WeChatMp:OAuth2Host"]}:{configuration["WeChatMp:OAuth2Port"]}/sns/oauth2/access_token?code={Code}";
            var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync(url).Result;
            return "APP:<br>"+result;
        }
    }
}
