using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;
using WxOAuthManager.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // GET: api/<WxOAuth2Controller>
        [HttpGet]
        [Route("/api/[controller]/123.aspx")]
        public IActionResult Get()
        {
            var Scope = "snsapi_base";
            var State = "123";
            //var url = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={configuration["WeChatMp:AppId"]}&redirect_uri={HttpUtility.UrlEncode(configuration["WeChatMp:RedirectUri"])}&response_type=code&scope={Scope}&state={State}#wechat_redirect";
            var url = $"http://192.168.240.232:5188/connect/oauth2/authorize?redirect_uri={HttpUtility.UrlEncode(configuration["WeChatMp:RedirectUri"])}&scope={Scope}&state={State}";
            return Redirect(url);
        }

        // GET api/<WxOAuth2Controller>/5
        [HttpGet]
        public string Get(string code,string state)
        {
            var Code = code;
            var url = $"http://192.168.240.232:5188/sns/oauth2/access_token?code={Code}";
            var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync(url).Result;
            return result;
            //return JsonSerializer.Deserialize<WebAccessToken>(result)!.openid;
        }

        // POST api/<WxOAuth2Controller>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<WxOAuth2Controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WxOAuth2Controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }


   

}
