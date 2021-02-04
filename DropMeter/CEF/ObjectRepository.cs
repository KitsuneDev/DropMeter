using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropMeter.CEF
{
    class ObjectRepository<T, T2> where T2: new()
    {
        public Dictionary<T, T2> keyValuePairs = new Dictionary<T, T2>();
        public T2 this[T key]
        {
            get
            {
                T2 value;
                if(keyValuePairs.TryGetValue(key, out value))
                {
                    return value;
                } else
                {
                    keyValuePairs[key] = new T2();
                    return keyValuePairs[key];
                }

                
            }
            set => keyValuePairs[key] = value;
        }
    }
    class OptionalRepository<T, T2> where T2: struct
    {
        public Dictionary<T, T2> keyValuePairs = new Dictionary<T, T2>();
        public T2? this[T key]
        {
            get
            {
                T2 value;
                if (keyValuePairs.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                   
                    return null;
                }


            }
            set
            {
                if (value != null)
                {
                    keyValuePairs[key] = value.Value;
                }
            }
        }
    }
}
