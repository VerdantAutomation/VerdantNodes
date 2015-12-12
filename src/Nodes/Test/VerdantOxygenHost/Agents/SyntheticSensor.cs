using System;
using Microsoft.SPOT;

namespace VerdantOxygenHost.Agents
{
    class SyntheticSensor
    {
        private readonly double _min;
        private readonly double _max;
        private readonly double _resolution;
        private readonly int _maxDelta;

        private Random _rng = new Random();
        private double _lastReading;

        public SyntheticSensor(double min, double max, double resolution, int maxDelta)
        {
            _min = min;
            _max = max;
            _resolution = resolution;
            _maxDelta = maxDelta;

            _lastReading = (_max + _min) / 2.0;
            _lastReading = GetReading();
        }

        public double GetReading()
        {
            do
            {
                int deltaSteps = _maxDelta - _rng.Next(_maxDelta * 2);
                var newValue = _lastReading + (_resolution * deltaSteps);
                if (newValue >= _min && newValue <= _max)
                {
                    _lastReading = newValue;
                    return _lastReading;
                }
            } while (true);
        }
    }
}
