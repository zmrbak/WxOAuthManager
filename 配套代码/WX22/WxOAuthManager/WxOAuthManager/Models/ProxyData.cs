namespace WxOAuthManager.Models
{
    public class ProxyData
    {
        public string RedirectUri { get; set; } = default!;
        public string State { get; set; } = default!;
        public Boolean IsQrCode { set; get; }=false;
    }
}
