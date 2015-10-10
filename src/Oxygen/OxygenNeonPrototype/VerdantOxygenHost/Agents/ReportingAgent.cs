using System;
using Microsoft.SPOT;
using Verdant.Node.Common;
using Verdant.Node.Core;
using System.Text;
using System.Collections;

namespace VerdantOxygenHost.Agents
{
    //TODO: Create a backlog list of recordings that were taken but not sent yet. Make sure that collection is bounded

    class ReportingAgent : IAgent
    {
        public const int MaximumBacklog = 10;

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
            Debug.Print("Reporting Agent Processing");

            _backlog.Add(CreateJsonPayload());
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

            //TODO: send

            _backlog.Clear();
        }

        private string CreateJsonPayload()
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
                    if (i != 0)
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
