using System;
using Microsoft.SPOT;
using System.Collections;

namespace Verdant.Node.Core
{
    public class PropertyDictionary : IEnumerable
    {
        private Hashtable _propTable = new Hashtable();

        public void SetProperty(string name, object value)
        {
            if (_propTable.Contains(name))
                _propTable[name] = value;
            else
                _propTable.Add(name, value);
        }

        public void RemoveProperty(string name)
        {
            if (_propTable.Contains(name))
                _propTable.Remove(name);
        }

        public object this[string key]
        {
            get
            {
                if (_propTable.Contains(key))
                    return _propTable[key];
                else
                    return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _propTable.GetEnumerator();
        }
    }
}
