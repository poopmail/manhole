using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace PoopmailGui.Api
{
    public class ApiResponse
    {
        public ApiResponse(HttpStatusCode statusCode, string response, bool hasResponse,
            Dictionary<string, string> responseHeaders, IList<RestResponseCookie> cookies, IList<string> httpCookies)
        {
            StatusCode = statusCode;
            Response = response;
            HasResponse = hasResponse;
            ResponseHeaders = responseHeaders;
            ResponseCookies = cookies;
            HttpCookies = httpCookies;
        }

        public HttpStatusCode StatusCode { get; }
        public string Response { get; set; }
        public bool HasResponse { get; }
        public Dictionary<string, string> ResponseHeaders { get; }
        public IList<RestResponseCookie> ResponseCookies { get; }
        public IList<string> HttpCookies { get; }

        public string ReadResponseMessage()
        {
            var reader = new JsonTextReader(new StringReader(Response));
            string msg = null;
            var next = false;
            while (reader.Read())
            {
                if (next)
                {
                    msg = (string) reader.Value;
                    break;
                }

                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("message")) next = true;
            }

            return msg;
        }
    }
}