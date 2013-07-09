using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Text2Buxfer.Services
{
    public class Buxfer
    {
        string token;
        string baseLoginUrl = "https://www.buxfer.com/api/login.json?userid={0}&password={1}";
        string baseCmdUrl = "https://www.buxfer.com/api/{0}.json?token={1}";

        public async Task<bool> Login(string username, string password)
        {
            BuxferOutput output = await ExecuteRequest(string.Format(baseLoginUrl, username, password));
            if (output.ResponseOk())
            {
                token = output.response.token;
                return true;
            }
            else
                return false;
        }

        public async Task<bool> AddTransaction(string text)
        {
            string format = "sms";
            string url = string.Format(baseCmdUrl, "add_transaction", token) + string.Format("&format={0}&text={1}", format, text.Replace(" ", "%20"));
            BuxferOutput output = await ExecuteRequest(url);
            return output.ResponseOk();
        }

        async Task<BuxferOutput> ExecuteRequest(string uri)
        {
            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync(uri);
            return JsonConvert.DeserializeObject<BuxferOutput>(result);
        }
    }

    public class BuxferOutput
    {
        public Response response { get; set; }

        public bool ResponseOk()
        {
            return this.response.status == "OK";
        }
    }

    public class Response
    {
        public string status { get; set; }
        public string token { get; set; }
        public string parseStatus { get; set; }
    }
}