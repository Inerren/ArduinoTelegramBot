using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoTelegramBot.Core
{
    public class Telegram
    {
        private string _apiURL;

        public Telegram(string token)
        {
            _apiURL = "https://api.telegram.org/bot" + token;
        }

        public async Task<string> GetData(string method, string parameters = "")
        {
            // Start the request
            HttpWebRequest HttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(_apiURL + "/" + method);
            HttpWebRequest.Method = "POST";
            HttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            Stream RequestStream = await HttpWebRequest.GetRequestStreamAsync();
            StreamWriter StreamWriter = new StreamWriter(RequestStream);
            StreamWriter.Write(parameters);
            StreamWriter.Flush();
            StreamWriter.Dispose();
            // Get the response
            HttpWebResponse HttpWebResponse = (HttpWebResponse)await HttpWebRequest.GetResponseAsync();
            StreamReader StreamReader = new StreamReader(HttpWebResponse.GetResponseStream());
            string result = StreamReader.ReadToEnd();

            return result;
        }
    }
}
