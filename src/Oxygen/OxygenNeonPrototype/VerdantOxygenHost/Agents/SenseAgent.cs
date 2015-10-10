using System;
using Microsoft.SPOT;
using Verdant.Node.Common;
using Verdant.Node.Core;

namespace VerdantOxygenHost.Agents
{
    class SenseAgent : IAgent
    {
        private TimeSpan ProcessingInterval = new TimeSpan(0, 0, 6);

        private SyntheticSensor _insideTemp1Generator = new SyntheticSensor(23, 28, 0.2, 2);
        private SyntheticSensor _insideTemp2Generator = new SyntheticSensor(23, 28, 0.2, 2);
        private SyntheticSensor _outsideTempGenerator = new SyntheticSensor(0, 38, 0.2, 4);

        private PropertyDictionary _properties;
        private double _insideTemp1;
        private double _insideTemp2;
        private double _outsideTemp;

        public SenseAgent()
        {
            _properties = (PropertyDictionary)DiContainer.Instance.Resolve(typeof(PropertyDictionary));
        }

        public DateTime Process(DateTime now)
        {
            // Moving average over 10 readings, with each reading coming every 6 seconds
            // So, roughly, a one-minute moving average
            Debug.Print("Sense Agent Processing");
            _insideTemp1 = (_insideTemp1Generator.GetReading() + 10.0 * _insideTemp1) / 11;
            _insideTemp2 = (_insideTemp2Generator.GetReading() + 10.0 * _insideTemp2) / 11;
            _outsideTemp = (_outsideTempGenerator.GetReading() + 10.0 * _outsideTemp) / 11;

            _properties.SetProperty("Inside Temp 1", System.Math.Round(_insideTemp1 * 10.0) / 10.0);
            _properties.SetProperty("Inside Temp 2", System.Math.Round(_insideTemp2 * 10.0) / 10.0);
            _properties.SetProperty("Outside Temp ", System.Math.Round(_outsideTemp * 10.0) / 10.0);

            return now + ProcessingInterval;
        }

        public DateTime Start()
        {
            _insideTemp1 = _insideTemp1Generator.GetReading();
            _insideTemp2 = _insideTemp2Generator.GetReading();
            _outsideTemp = _outsideTempGenerator.GetReading();

            return DateTime.MinValue;
        }

        public void Stop()
        {
        }
    }
}
