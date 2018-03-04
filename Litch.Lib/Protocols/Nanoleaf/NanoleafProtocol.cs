using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Litch.Lib.Protocols.Nanoleaf
{
    public class NanoleafProtocol
    {
        private const string EndpointFormat = "http://{0}:{1}/api/v1/{2}/{3}";

        public struct StateValue
        {
            public int value;
            public int max;
            public int min;
        }

        public struct StateValueInput_1
        {
            public int value;
            public int duration;
        }

        public struct StateValueInput_2
        {
            public int increment;
        }

        public IPEndPoint Endpoint { get; private set; }

        public NanoleafProtocol(IPEndPoint endpoint)
        {
            this.Endpoint = endpoint;
        }

        private string GetEndpoint(string user, string method) =>
            string.Format(EndpointFormat, this.Endpoint.Address.ToString(), this.Endpoint.Port, user, method);

        private Task<HttpWebResponse> IssueGetRequestAsync(string uri, string body = null) => this.IssueRequestAsync(uri, "GET", body);

        private Task<HttpWebResponse> IssuePutRequestAsync(string uri, string body = null) => this.IssueRequestAsync(uri, "PUT", body);

        private Task<HttpWebResponse> IssuePostRequestAsync(string uri, string body = null) => this.IssueRequestAsync(uri, "POST", body);

        private Task<HttpWebResponse> IssueDeleteRequestAsync(string uri, string body = null) => this.IssueRequestAsync(uri, "DELETE", body);

        private async Task<HttpWebResponse> IssueRequestAsync(string uri, string method, string body = null)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = method;

            if (!string.IsNullOrEmpty(body))
            {
                using (var sw = new StreamWriter(req.GetRequestStream()))
                {
                    sw.Write(body);
                }
            }

            return (HttpWebResponse)await req.GetResponseAsync();
        }

        public async Task<string> CreateUserAsync()
        {
            var res = await this.IssuePostRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/new");
            
            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Failure",
                    new Exception($"Expected '200' got '{res.StatusCode}'"),
                    WebExceptionStatus.ProtocolError,
                    res);
            }

            string jsonData;

            using (var sr = new StreamReader(res.GetResponseStream()))
            {
                jsonData = sr.ReadToEnd();
            }

            // parse the actual token
            var result = JObject.Parse(jsonData)["auth_token"].ToString();

            res.Close();

            return result;
        }

        public async Task<bool> DeleteUserAsync(string user)
        {
            var res = await this.IssueDeleteRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/${user}");

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<string> GetRawDataAsync(string user)
        {
            var res = await this.IssueGetRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}");

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Failure",
                    new Exception($"Expected '200' got '{res.StatusCode}'"),
                    WebExceptionStatus.ProtocolError,
                    res);
            }

            string rawData;

            using (var sr = new StreamReader(res.GetResponseStream()))
            {
                rawData = sr.ReadToEnd();
            }

            res.Close();

            return rawData;
        }

        public async Task<bool> GetOnAsync(string user)
        {
            var res = await this.IssueGetRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/on");

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Failure",
                    new Exception($"Expected '200' got '{res.StatusCode}'"),
                    WebExceptionStatus.ProtocolError,
                    res);
            }

            string jsonData;

            using (var sr = new StreamReader(res.GetResponseStream()))
            {
                jsonData = sr.ReadToEnd();
            }

            // parse the actual state value
            var result = JObject.Parse(jsonData)["value"].ToString() == "true";

            res.Close();

            return result;
        }

        public async Task<bool> SetOnAsync(string user, bool state)
        {
            var res = await this.IssuePutRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                JsonConvert.SerializeObject(new { on = new { value = state } }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<StateValue> GetBrightnessAsync(string user)
        {
            var res = await this.IssueGetRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/brightness");

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Failure",
                    new Exception($"Expected '200' got '{res.StatusCode}'"),
                    WebExceptionStatus.ProtocolError,
                    res);
            }

            string jsonData;

            using (var sr = new StreamReader(res.GetResponseStream()))
            {
                jsonData = sr.ReadToEnd();
            }

            // parse the actual state value
            var result = JsonConvert.DeserializeObject<StateValue>(jsonData);

            res.Close();

            return result;
        }

        public async Task<bool> SetBrightnessAsync(string user, StateValueInput_1 input)
        {
            var res = await this.IssuePutRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                JsonConvert.SerializeObject(new { brightness = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<bool> SetBrightnessAsync(string user, StateValueInput_2 input)
        {
            var res = await this.IssuePutRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                JsonConvert.SerializeObject(new { brightness = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<StateValue> GetHueAsync(string user)
        {
            var res = await this.IssueGetRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/hue");

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Failure",
                    new Exception($"Expected '200' got '{res.StatusCode}'"),
                    WebExceptionStatus.ProtocolError,
                    res);
            }

            string jsonData;

            using (var sr = new StreamReader(res.GetResponseStream()))
            {
                jsonData = sr.ReadToEnd();
            }

            // parse the actual state value
            var result = JsonConvert.DeserializeObject<StateValue>(jsonData);

            res.Close();

            return result;
        }

        public async Task<bool> SetHueAsync(string user, StateValueInput_1 input)
        {
            var res = await this.IssuePutRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                JsonConvert.SerializeObject(new { hue = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<bool> SetHueAsync(string user, StateValueInput_2 input)
        {
            var res = await this.IssuePutRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                JsonConvert.SerializeObject(new { hue = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<StateValue> GetSaturationAsync(string user)
        {
            var res = await this.IssueGetRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/saturation");

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Failure",
                    new Exception($"Expected '200' got '{res.StatusCode}'"),
                    WebExceptionStatus.ProtocolError,
                    res);
            }

            string jsonData;

            using (var sr = new StreamReader(res.GetResponseStream()))
            {
                jsonData = sr.ReadToEnd();
            }

            // parse the actual state value
            var result = JsonConvert.DeserializeObject<StateValue>(jsonData);

            res.Close();

            return result;
        }

        public async Task<bool> SetSaturationAsync(string user, StateValueInput_1 input)
        {
            var res = await this.IssuePutRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                JsonConvert.SerializeObject(new { saturation = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<bool> SetSaturationAsync(string user, StateValueInput_2 input)
        {
            var res = await this.IssuePutRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                JsonConvert.SerializeObject(new { saturation = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<StateValue> GetColorTemperatureAsync(string user)
        {
            var res = await this.IssueGetRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/ct");

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Failure",
                    new Exception($"Expected '200' got '{res.StatusCode}'"),
                    WebExceptionStatus.ProtocolError,
                    res);
            }

            string jsonData;

            using (var sr = new StreamReader(res.GetResponseStream()))
            {
                jsonData = sr.ReadToEnd();
            }

            // parse the actual state value
            var result = JsonConvert.DeserializeObject<StateValue>(jsonData);

            res.Close();

            return result;
        }

        public async Task<bool> SetColorTemperatureAsync(string user, StateValueInput_1 input)
        {
            var res = await this.IssuePutRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                JsonConvert.SerializeObject(new { ct = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<bool> SetColorTemperatureAsync(string user, StateValueInput_2 input)
        {
            var res = await this.IssuePutRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                JsonConvert.SerializeObject(new { ct = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<string> GetColorModeAsync(string user)
        {
            var res = await this.IssueGetRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/colorMode");

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Failure",
                    new Exception($"Expected '200' got '{res.StatusCode}'"),
                    WebExceptionStatus.ProtocolError,
                    res);
            }

            string rawData;

            using (var sr = new StreamReader(res.GetResponseStream()))
            {
                rawData = sr.ReadToEnd();
            }
            
            res.Close();

            return rawData;
        }

        public async Task<bool> WriteDisplayCommandAsync(string user, IDisplayCommand displayCommand)
        {
            var serializedCommand = JsonConvert.SerializeObject(new WriteDisplayCommand(displayCommand));

            using (var request = await this.IssuePutRequestAsync(this.GetEndpoint(user, "effects"), serializedCommand))
            {
                return request.StatusCode == HttpStatusCode.NoContent;
            };
        }

        public async Task<bool> IdentifyAsync(string user)
        {
            using (var request = await this.IssuePutRequestAsync(this.GetEndpoint(user, "identify")))
            {
                return request.StatusCode == HttpStatusCode.NoContent;
            };
        }
    }
}
