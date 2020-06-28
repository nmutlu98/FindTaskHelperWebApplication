using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public interface ICurrentSession
{
    void SetContextAccessor(IHttpContextAccessor httpContextAccessor);
    HttpContext GetHttpContext();
    bool SetUsername(string username);
    string GetUsername();
    bool ResetUsername();
}
public class CurrentSession : ICurrentSession
{
    private IHttpContextAccessor _httpContextAccessor;

    public void SetContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public HttpContext GetHttpContext()
    {
        return _httpContextAccessor.HttpContext;
    }
    public bool SetUsername(string username)
    {
        GetHttpContext().Session.SetString("username", username);
        return true;
    }
    public string GetUsername()
    {
        return GetHttpContext().Session.GetString("username");
    }
    public bool ResetUsername()
    {
        GetHttpContext().Session.SetString("username", "");
        return true;
    }
}