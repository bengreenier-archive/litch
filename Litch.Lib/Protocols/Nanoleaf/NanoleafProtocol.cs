using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Litch.Lib.Protocols.Nanoleaf
{
    public class NanoleafProtocol
    {
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

        private async Task<HttpWebResponse> IssueRequestAsync(string uri, string method = "GET", string body = null)
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

            return (HttpWebResponse) await req.GetResponseAsync();
        }

        public async Task<string> CreateUserAsync()
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/new", "POST");
            
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
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/${user}", "DELETE");

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<string> GetRawDataAsync(string user)
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}", "GET");

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
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/on", "GET");

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
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                "PUT",
                JsonConvert.SerializeObject(new { on = new { value = state } }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<StateValue> GetBrightnessAsync(string user)
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/brightness", "GET");

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
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                "PUT",
                JsonConvert.SerializeObject(new { brightness = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<bool> SetBrightnessAsync(string user, StateValueInput_2 input)
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                "PUT",
                JsonConvert.SerializeObject(new { brightness = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<StateValue> GetHueAsync(string user)
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/hue", "GET");

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
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                "PUT",
                JsonConvert.SerializeObject(new { hue = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<bool> SetHueAsync(string user, StateValueInput_2 input)
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                "PUT",
                JsonConvert.SerializeObject(new { hue = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<StateValue> GetSaturationAsync(string user)
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/saturation", "GET");

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
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                "PUT",
                JsonConvert.SerializeObject(new { saturation = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<bool> SetSaturationAsync(string user, StateValueInput_2 input)
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                "PUT",
                JsonConvert.SerializeObject(new { saturation = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<StateValue> GetColorTemperatureAsync(string user)
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/ct", "GET");

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
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                "PUT",
                JsonConvert.SerializeObject(new { ct = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<bool> SetColorTemperatureAsync(string user, StateValueInput_2 input)
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state",
                "PUT",
                JsonConvert.SerializeObject(new { ct = input }));

            var result = res.StatusCode == HttpStatusCode.NoContent;

            res.Close();

            return result;
        }

        public async Task<string> GetColorModeAsync(string user)
        {
            var res = await IssueRequestAsync($"http://{Endpoint.Address.ToString()}:{Endpoint.Port}/api/v1/{user}/state/colorMode", "GET");

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

    }
}
