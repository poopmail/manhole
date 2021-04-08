using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using PoopmailGui.Api.Model;
using RestSharp;

namespace PoopmailGui.Api
{
    public class ApiClient
    {
        private const string ApiServer = "https://api.poopmail.pm";
        private const string ApiServerDev = "https://api.s.poopmail.pm";
        private string ApiServerInUse;

        private readonly RestClient _restClient;

        public ApiClient(bool dev)
        {
            // Initialize RestClient
            _restClient = new RestClient(ApiServerInUse = dev ? ApiServerDev : ApiServer)
            {
                UserAgent = "Poopmail/Gui",
                Timeout = 6000,
                PreAuthenticate = true
            };
            _restClient.ConfigureWebRequest(rq => rq.KeepAlive = true);
            _restClient.CookieContainer = new CookieContainer();
        }

        public Result<Mailbox> CreateMailbox(string rft, string act, string mail)
        {
            var split = mail.Split("@");
            var key = split[0];
            var domain = split.Length > 1 ? split[1] : "";

            var response = MakeApiRequest("/v1/mailboxes",
                "{\"key\": \"" + key + "\", \"domain\": \"" + domain + "\", \"account\": \"@me\"}",
                new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}, {"Authorization", "Bearer " + act}
                }, new Dictionary<string, string>
                {
                    {"account", "@me"}
                },
                new Dictionary<string, string>(), "POST");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                act = GetAccessToken(rft).Obj.Token;
                response = MakeApiRequest("/v1/mailboxes", "{\"key\": \"" + key + "\", \"domain\": \"" + domain + "\"}",
                    new Dictionary<string, string>
                    {
                        {"Content-Type", "application/json"}, {"Authorization", "Bearer " + act}
                    }, new Dictionary<string, string>
                    {
                        {"account", "@me"}
                    },
                    new Dictionary<string, string>(), "POST");
            }

            if (!isGood(response.StatusCode))
                return new Result<Mailbox> {Obj = null, Response = response};

            var obj = JsonConvert.DeserializeObject<Mailbox>(response.Response);
            return new Result<Mailbox> {Obj = obj, Response = response};
        }

        public Result<MailboxResponse> GetMailboxes(string rft, string act)
        {
            var response = MakeApiRequest("/v1/mailboxes", "", new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}, {"Authorization", "Bearer " + act}
                }, new Dictionary<string, string>
                {
                    {"account", "@me"}
                },
                new Dictionary<string, string>(), "GET");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                act = GetAccessToken(rft).Obj.Token;
                response = MakeApiRequest("/v1/mailboxes", "", new Dictionary<string, string>
                    {
                        {"Content-Type", "application/json"}, {"Authorization", "Bearer " + act}
                    }, new Dictionary<string, string>
                    {
                        {"account", "@me"}
                    },
                    new Dictionary<string, string>(), "GET");
            }

            if (!isGood(response.StatusCode))
                return new Result<MailboxResponse> {Obj = null, Response = response};

            var obj = JsonConvert.DeserializeObject<MailboxResponse>(response.Response);
            return new Result<MailboxResponse> {Obj = obj, Response = response};
        }

        public Result<MessageResponse> GetMessages(string rft, string act, string mailbox)
        {
            var response = MakeApiRequest("/v1/messages", "", new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}, {"Authorization", "Bearer " + act}
                }, new Dictionary<string, string>
                {
                    {"mailbox", mailbox}
                },
                new Dictionary<string, string>(), "GET");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                act = GetAccessToken(rft).Obj.Token;
                response = MakeApiRequest("/v1/messages", "", new Dictionary<string, string>
                    {
                        {"Content-Type", "application/json"}, {"Authorization", "Bearer " + act}
                    }, new Dictionary<string, string>
                    {
                        {"mailbox", mailbox}
                    },
                    new Dictionary<string, string>(), "GET");
            }

            if (!isGood(response.StatusCode))
                return new Result<MessageResponse> {Obj = null, Response = response};

            var obj = JsonConvert.DeserializeObject<MessageResponse>(response.Response);
            return new Result<MessageResponse> {Obj = obj, Response = response};
        }

        public Result<string> GetRefreshToken(string user, string pass)
        {
            var resp = MakeApiRequest("/v1/auth/refresh_token",
                "{\"username\":\"" + user + "\",\"password\":\"" + pass + "\"}",
                new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }, new Dictionary<string, string>(),
                new Dictionary<string, string>(), "POST",
                new List<string> {"/v1/auth/access_token"});
            if (!isGood(resp.StatusCode))
                return new Result<string> {Obj = null, Response = resp};

            string cookie = null;
            foreach (var httpCookie in resp.HttpCookies)
                if (httpCookie.StartsWith("_refresh_token"))
                    cookie = httpCookie.Split("=")[1];
            return new Result<string> {Obj = cookie, Response = resp};
        }

        public Result<AccessToken> GetAccessToken(string refreshToken)
        {
            var resp = MakeApiRequest("/v1/auth/access_token", "",
                new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }, new Dictionary<string, string>(),
                new Dictionary<string, string>
                {
                    {"_refresh_token", refreshToken}
                }, "GET");
            if (!isGood(resp.StatusCode)) return new Result<AccessToken> {Obj = null, Response = resp};

            var strRes = resp.Response;
            var obj = JsonConvert.DeserializeObject<AccessToken>(strRes);
            return new Result<AccessToken> {Obj = obj, Response = resp};
        }

        private ApiResponse MakeApiRequest(string path, string body, Dictionary<string, string> header,
            Dictionary<string, string> query, Dictionary<string, string> cookies, string method)
        {
            return MakeApiRequest(path, body, header, query, cookies, method, new List<string>());
        }


        private ApiResponse MakeApiRequest(string path, string body, Dictionary<string, string> header,
            Dictionary<string, string> query, Dictionary<string, string> cookies, string method,
            List<string> cookiePaths)
        {
            // Initialize RestRequest
            var restRequest = new RestRequest(path) {Method = Enum.TryParse(method, out Method tmp) ? tmp : Method.GET};

            // Add query parameters and headers to the request
            query.ForEach(pair => restRequest.AddQueryParameter(pair.Key, pair.Value));
            header.Where(pair => !pair.Key.Equals("User-Agent", StringComparison.OrdinalIgnoreCase))
                .ForEach(pair => restRequest.AddHeader(pair.Key, pair.Value));

            // Write the body if the given string is not empty
            if (!body.Equals("")) restRequest.Body = new RequestBody("text/plain", "text/plain", body);

            // Set cookies
            foreach (var (key, value) in cookies) restRequest.AddCookie(key, value);

            // Execute the request and retrieve a response
            _restClient.CookieContainer = new CookieContainer();
            var restResponse = _restClient.Execute(restRequest);

            // Get http only cookies
            List<string> httpCookies = cookiePaths.Select(cookiePath =>
                _restClient.CookieContainer.GetCookieHeader(new Uri(ApiServerInUse + cookiePath))).ToList();

            // Return the response in form of a parsed ApiResponse object
            return new ApiResponse(restResponse.StatusCode, restResponse.Content, !restResponse.Content.Equals(""),
                restResponse.Headers.Where(parameter => parameter.Type == ParameterType.HttpHeader)
                    .ToDictionary(parameter => parameter.Name, parameter => parameter.Value.ToString()),
                restResponse.Cookies, httpCookies);
        }

        private bool isGood(HttpStatusCode responseStatusCode)
        {
            return (int)responseStatusCode >= 200 && (int)responseStatusCode < 300;
        }
        
    }
}