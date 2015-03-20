using System;
using System.Windows;
using System.Net;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Text;
using System.Collections.Specialized;

namespace Yandex.Metrica.Sample
{
    /// <summary>
    /// Report sample application events.
    /// </summary>
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            YandexMetrica.ReportEvent("MainPage Ctor");
            YandexMetrica.DispatchPeriod = TimeSpan.FromSeconds(5);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            YandexMetrica.ReportEvent("MainPage Unloaded");
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            YandexMetrica.ReportEvent("MainPage Loaded");
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            YandexMetrica.ReportEvent("Button click");
        }

        private void OnError(object sender, RoutedEventArgs e)
        {
            try
            {
                throw new ArgumentException("Throw exception and catch it'");
            }
            catch (Exception ex)
            {
                YandexMetrica.ReportError("Main button error", ex);
            }
        }

        private void OnCrash(object sender, RoutedEventArgs e)
        {
            throw new ArgumentException();
        }

        private async void OnUpdateAPIKey(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("http://workshop.bx23.net/key");
            uint newAPIKey = await GetAPIKeyAsync(uri);
            //if(newAPIKey > 0)
            //    YandexMetrica.ApiKey = newAPIKey;
        }

        private async Task<uint> GetAPIKeyAsync(Uri uri)
        {
//            HttpWebRequest httpWebRequest = await GetWebRequestAsync(uri);
//            httpWebRequest.Method = "GET";

            //using (Stream requestStream = await Task.Factory.FromAsync<Stream>(
            //    httpWebRequest.BeginGetRequestStream,
            //    httpWebRequest.EndGetRequestStream,
            //    null))
            //{
            //    WriteReportMessage(message, requestStream);
            //}

            //using (WebResponse response = await Task.Factory.FromAsync<WebResponse>(
            //    httpWebRequest.BeginGetResponse,
            //    httpWebRequest.EndGetResponse,
            //    null))
            //{
            //    var httpWebResponse = response as HttpWebResponse;
            //    Debug.Assert(httpWebResponse != null);

            //    return ParseAPIKeyResponce(StreamToString(httpWebResponse.GetResponseStream()));
            //}
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += onDownloaded;
            wc.DownloadStringAsync(uri);
            return 0;// ParseAPIKeyResponce(ars);
        }

        void onDownloaded(object sender, DownloadStringCompletedEventArgs e)
        {
            uint k = ParseAPIKeyResponse(e.Result);
            if( k > 0)
                YandexMetrica.ApiKey = k;
        }

        private async Task<HttpWebRequest> GetWebRequestAsync(Uri uri)
        {
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(uri);

//            string userAgent = await GetUserAgentStringAsync();

//#if WINDOWS_PHONE
//            httpWebRequest.Headers["User-Agent"] = userAgent;
//#else
//            httpWebRequest.UserAgent = userAgent;
//#endif

            return httpWebRequest;
        }

        private uint ParseAPIKeyResponse(string APIKeyResponce)
        {
            uint result = 0;
            Dictionary<string, string> ld = JsonConvert.DeserializeObject<Dictionary<string, string>>(APIKeyResponce);
            if (ld["status"] == "ok")
            {
                result = Convert.ToUInt32(ld["key"]);
            }
            return result;
        }

        private static string StreamToString(Stream stream)
        {
            //stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

    }
}
