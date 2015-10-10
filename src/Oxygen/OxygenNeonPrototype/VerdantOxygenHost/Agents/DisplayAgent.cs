using System;
using Microsoft.SPOT;
using Verdant.Node.Common;
using Verdant.Node.Core;

namespace VerdantOxygenHost.Agents
{
    class DisplayAgent : IAgent
    {
        private TimeSpan ProcessingInterval = new TimeSpan(0, 0, 3);
        private PropertyDictionary _properties;
        private ILcd2x16Driver _lcd;

        private string[] _propList = new string[]
        {
            PropertyNames.CurrentTimePropName,
            PropertyNames.IPAddressPropName,
            PropertyNames.InsideTemp1PropName,
            PropertyNames.InsideTemp2PropName,
            PropertyNames.OutsideTempPropName
        };
        private int _currentItem = 0;

        public DisplayAgent()
        {
            _lcd = (ILcd2x16Driver)DiContainer.Instance.Resolve(typeof(ILcd2x16Driver));
            _properties = (PropertyDictionary)DiContainer.Instance.Resolve(typeof(PropertyDictionary));
        }

        public DateTime Process(DateTime now)
        {
            // Every five seconds, update the display if it is not being used for UI
            Debug.Print("Display Agent Processing");

            string propName;
            object value;
            var iRetries = _propList.Length;
            do
            {
                propName = _propList[_currentItem++];
                if (_currentItem >= _propList.Length)
                    _currentItem = 0;

                if (propName == PropertyNames.CurrentTimePropName)
                    value = DateTime.Now.ToString();
                else
                    value = _properties[propName];
                if (--iRetries==0) // don't probe more times than we have values. This is just to avoid an infinite loop here if the proptable is empty
                    break;
            } while (value == null);

            if (value != null)
            {
                _lcd.Clear();
                _lcd.SetCursor(0, 0);
                _lcd.Print(propName);
                _lcd.SetCursor(0, 1);
                if (value is double)
                {
                    _lcd.Print(((double)value).ToString("N"));
                }
                else
                {
                    _lcd.Print(value.ToString());
                }
            }

            return now + ProcessingInterval;
        }

        public DateTime Start()
        {
            return DateTime.MinValue;
        }

        public void Stop()
        {
        }
    }
}
