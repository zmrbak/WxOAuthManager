using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace WxOAuthManager.Attributes
{
    public class CallBackHostActionFilterAttribute : ActionFilterAttribute
    {
        private readonly IConfiguration configuration;

        public CallBackHostActionFilterAttribute(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var hostList = configuration.GetSection("ProxyedHosts").GetChildren();
            var redirect_uri = context.ActionArguments["redirect_uri"];
            if (redirect_uri != null)
            {
                try
                {
                    var uri = new Uri(redirect_uri.ToString()!);
                    if (hostList.Any(s => s.Value.ToString().ToLower() == uri.Host.ToLower()))
                    {
                        return base.OnActionExecutionAsync(context, next);
                    }
                }
                catch {
                    goto REDIRECT_URI_ERROR;
                }
            }

            REDIRECT_URI_ERROR:
            context.Result = new UnauthorizedObjectResult(new
            {
                Success = false,
                Message = $"您的回调地址:“{redirect_uri}”不在允许的范围内，请修改配置文件中的“ProxyedHosts”属性。" +
                $"当前允许的地址为：" + String.Join(",", hostList.Select(x=>x.Value))

            });
            
            return Task.CompletedTask;
        }
    }
}
