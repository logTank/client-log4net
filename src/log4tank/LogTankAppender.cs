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

        private Queue<LogTankEvent> _logs = new Queue<LogTankEvent>();
        private BackgroundWorker _worker = new BackgroundWorker();
        private JsonSerializerSettings _defaultSerializerSettings = new JsonSerializerSettings();

        private Uri _uri = null;
        private Encoding _defaultEncoding = new UTF8Encoding();

        private string _customerKey = string.Empty;
        private string _apiKey = string.Empty;
        private string _storeServerUrl = "https://store.logtank.com";
        private string _tags;

        public string CustomerKey
        {
            get
            {
                return _customerKey;
            }

            set
            {
                if (value != null)
                {
                    value = Environment.ExpandEnvironmentVariables(value);
                }
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
                if (value != null)
                {
                    value = Environment.ExpandEnvironmentVariables(value);
                }
                _apiKey = value;
                _uri = null;
            }
        }

        public string StoreServerUrl
        {
            get
            {
                return _storeServerUrl;
            }

            set
            {
                if (value != null)
                {
                    value = Environment.ExpandEnvironmentVariables(value);
                }
                _storeServerUrl = value;
                _uri = null;
            }
        }

        public string Tags
        {
            get
            {
                return _tags;
            }

            set
            {
                if (value != null)
                {
                    value = Environment.ExpandEnvironmentVariables(value);
                    value = value.Replace(',', '/');
                }
                _tags = value;
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
                    tmpUri = new Uri(new Uri(StoreServerUrl), CustomerKey + "/" + ApiKey + "/" + Tags);
                    _uri = tmpUri;
                }

                return tmpUri;
            }
        }

        public LogTankAppender()
        {
            // mono bug-fix: http://forums.xamarin.com/discussion/10405/the-authentication-or-decryption-has-failed-in-the-web-request
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
            System.Security.Cryptography.AesCryptoServiceProvider b = new System.Security.Cryptography.AesCryptoServiceProvider();

            _defaultSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            _defaultSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            _worker.DoWork += StoreLogs;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            lock (_lock)
            {
                _logs.Enqueue(new LogTankEvent(loggingEvent));
                StartWorkerIfNotRunning();
            }
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            lock (_lock)
            {
                foreach (var item in loggingEvents)
                {
                    _logs.Enqueue(new LogTankEvent(item));
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
                    if (_logs.Count < 2000)
                    {
                        json = JsonConvert.SerializeObject(_logs, _defaultSerializerSettings);
                        _logs.Clear();
                    }
                    else
                    {
                        LogTankEvent[] buffer = new LogTankEvent[1000];

                        for (int i = 0; i < buffer.Length; i++)
                        {
                            buffer[i] = _logs.Dequeue();
                        }

                        json = JsonConvert.SerializeObject(_logs, _defaultSerializerSettings);
                    }
                }

                SendJsonToLogTank(json);
            }
        }

        private void SendJsonToLogTank(string json)
        {
            var http = GetHttpClient();

            try
            {
                DumpJsonIntoStream(json, http.GetRequestStream());
                http.GetResponse().Close();
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Cannot send logs to logTank.", ex);
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