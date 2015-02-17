﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using log4net.Appender;
using log4net.Core;
using Newtonsoft.Json;

namespace log4tank
{
    public class LogTankAppender : AppenderSkeleton
    {
        private object _lock = new object();

        private List<LogTankEvent> _logs = new List<LogTankEvent>();
        private BackgroundWorker _worker = new BackgroundWorker();
        private JsonSerializerSettings _defaultSerializerSettings = new JsonSerializerSettings();

        private Uri _uri = null;
        private Encoding _defaultEncoding = new UTF8Encoding();

        private string _customerKey = string.Empty;
        private string _apiKey = string.Empty;

        public string CustomerKey
        {
            get
            {
                return _customerKey;
            }

            set
            {
                _customerKey = value;
                _uri = null;
            }
        }

        public string ApiKey
        {
            get
            {
                return _apiKey;
            }

            set
            {
                _apiKey = value;
                _uri = null;
            }
        }

        private Uri StoreUri
        {
            get
            {
                var tmpUri = _uri;

                if (tmpUri == null)
                {
                    tmpUri = new Uri(string.Format("http://store.logtank.com/{0}/{1}", CustomerKey, ApiKey));
                    _uri = tmpUri;
                }

                return tmpUri;
            }
        }

        public LogTankAppender()
        {
            _defaultSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            _defaultSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            _worker.DoWork += StoreLogs;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            lock (_lock)
            {
                _logs.Add(new LogTankEvent(loggingEvent));
                StartWorkerIfNotRunning();
            }
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            lock (_lock)
            {
                foreach (var item in loggingEvents)
                {
                    _logs.Add(new LogTankEvent(item));
                }
                StartWorkerIfNotRunning();
            }
        }

        private void StoreLogs(object sender, DoWorkEventArgs e)
        {
            while (_logs.Count > 0)
            {
                string json = "";

                lock (_lock)
                {
                    json = JsonConvert.SerializeObject(_logs, _defaultSerializerSettings);
                    _logs.Clear();
                }

                var http = GetHttpClient();

                try
                {
                    DumpJsonIntoStream(json, http.GetRequestStream());
                    //http.GetResponse().Close();
                    var response = http.GetResponse();
                    var sr = new StreamReader(response.GetResponseStream());

                    var strResponse = sr.ReadToEnd();
                    Console.WriteLine(strResponse);
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error("Cannot send logs to logTank.", ex);
                }
            }
        }

        private HttpWebRequest GetHttpClient()
        {
            var http = (HttpWebRequest)WebRequest.Create(StoreUri);

            http.ContentType = "application/json";
            http.Method = "POST";
            http.KeepAlive = false;

            return http;
        }

        private void DumpJsonIntoStream(string json, Stream stream)
        {
            var bytes = _defaultEncoding.GetBytes(json);
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
        }

        private void StartWorkerIfNotRunning()
        {
            if (!_worker.IsBusy)
            {
                _worker.RunWorkerAsync();
            }
        }
    }
}