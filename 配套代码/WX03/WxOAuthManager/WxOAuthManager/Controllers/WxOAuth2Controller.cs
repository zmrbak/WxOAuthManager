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
        private string AppId = "wx6dbd50066c719ab7";
        private string AppSecret = "5231ed39553611b4f44edf1a94880ec3";

        // GET: api/<WxOAuth2Controller>
        [HttpGet]
        public string Get()
        {
            var RedirectUri = "http://127.0.0.1/abc.aspx";
            var Scope = "snsapi_base";
            var State = "123";
            var url = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={AppId}&redirect_uri={HttpUtility.UrlEncode(RedirectUri)}&response_type=code&scope={Scope}&state={State}#wechat_redirect";
            return url;
        }

        // GET api/<WxOAuth2Controller>/5
        [HttpGet("{code}")]
        public string Get(string code)
        {
            var Code = code;
            var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={AppId}&secret={AppSecret}&code={Code}&grant_type=authorization_code";
            var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync(url).Result;

            return JsonSerializer.Deserialize<WebAccessToken>(result)!.openid;
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
