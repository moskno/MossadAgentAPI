using Microsoft.AspNetCore.Mvc;

namespace MossadAgentAPI.Utils
{
    [ApiController]
    public static class HttpUtils
    {
        public static object Response(int status, object message)
        {
            bool success = status >= 200 && status < 300;
            return new
            {
                succes = success,
                message = message
            };
        }
    }
}
