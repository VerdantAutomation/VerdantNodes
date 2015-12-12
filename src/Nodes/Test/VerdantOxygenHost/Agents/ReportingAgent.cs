using System;
using Microsoft.SPOT;
using Verdant.Node.Common;
using Verdant.Node.Core;
using System.Text;
using System.Collections;
using PervasiveDigital.Net;
using PervasiveDigital;

namespace VerdantOxygenHost.Agents
{
    //TODO: Create a backlog list of recordings that were taken but not sent yet. Make sure that collection is bounded

    class ReportingAgent : IAgent
    {
        public const int MaximumBacklog = 5;

        private TimeSpan ProcessingInterval = new TimeSpan(0, 1, 0);
        private PropertyDictionary _properties;
        private ArrayList _backlog = new ArrayList();
        private int _droppedRecords = 0;

        // this array is structured as : propName, jsonTag
        private string[] _propList = new string[]
        {
            PropertyNames.NodeIdPropName, "nodeId",
            PropertyNames.InsideTemp1PropName, "t1",
            PropertyNames.InsideTemp2PropName, "t2",
            PropertyNames.OutsideTempPropName, "t3"
        };

        public ReportingAgent()
        {
            _properties = (PropertyDictionary)DiContainer.Instance.Resolve(typeof(PropertyDictionary));
        }

        public DateTime Process(DateTime now)
        {
            // Once per minute, send data
            _backlog.Add(CreatePayload());
            while (_backlog.Count > MaximumBacklog)
            {
                _backlog.RemoveAt(0);
                ++_droppedRecords;
            }
            SendBacklog();

            return now + ProcessingInterval;
        }

        public DateTime Start()
        {
            return DateTime.Now + new TimeSpan(TimeSpan.TicksPerMinute);
        }

        public void Stop()
        {
        }

        private void SendBacklog()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            bool fFirst = true;
            foreach (var item in _backlog)
            {
                if (!fFirst)
                    sb.Append(",");
                fFirst = false;
                sb.Append(item);
            }
            sb.Append("]");

            //TODO: With large backlogs and large property sets, this will exceed the ESP8266 buffer. To fix that, send each backlog item separately

            // Send the data
            var adapter = (INetworkAdapter)DiContainer.Instance.Resolve(typeof(INetworkAdapter));
            var httpClient = new HttpClient(adapter);
            var request = new HttpRequest(HttpMethod.Post, new Uri("http://192.168.1.236/"));
            request.Body = Encoding.UTF8.GetBytes(sb.ToString());
            //request.ResponseReceived += HttpResponseReceived;
            httpClient.SendAsync(request);

            //TODO: We're always assuming the tranmission worked, and it doesn't. We're not building up the backlog of failed transmissions
            _backlog.Clear();
        }

        private string CreatePayload()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            sb.Append("\"timestamp: \"" + DateTime.UtcNow.ToString("r") + "\"");

            for (int i = 0; i<_propList.Length; i += 2)
            {
                string name = _propList[i + 1];
                var value = _properties[_propList[i]];

                if (value!=null)
                {
                    sb.Append(",");
                    if (value is double)
                    {
                        sb.Append("\"" + name + "\":" + ((double)value).ToString("N"));
                    }
                    else
                    {
                        sb.Append("\"" + name + "\": \"" + value.ToString() + "\"");
                    }
                }
            }
            sb.Append("}");

            return sb.ToString();
        }
    }
}
