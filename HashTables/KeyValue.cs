using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    /// <summary>
    /// Key-Value-Pair object used to lump the key and value together for storage purposes.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class KeyValue<K, V>
        where K : IComparable<K>
        where V : IComparable<V>
    {
        //Store the key
        K kKey;
        //Store the value
        V vValue;

        //Store the amount of times accessed
        int iAccess = 0;

        public KeyValue(K key, V value)
        {
            kKey = key;
            vValue = value;
        }

        public K Key
        {
            get
            {
                return kKey;
            }
        }

        public V Value
        {
            get
            {
                return vValue;
            }
        }

        public int Access
        {
            get
            {
                return iAccess;
            }
        }

        /// <summary>
        /// Called to reset the access count for the KeyValue pair. Should be used
        /// for expanding a hash table using the chaining method.
        /// </summary>
        public void ResetAccess()
        {
            iAccess = 0;
        }

        public void IncrementAccess()
        {
            iAccess++;
        }

        /// <summary>
        /// Must override Equals so that the KeyValue pair can be compared to another
        /// KeyValue pair. Especially important for the ArrayList. Contains method that
        /// calls the stored objects Equals method for comparisons.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            KeyValue<K, V> kv = (KeyValue<K, V>)obj;
            return this.Key.CompareTo(kv.Key) == 0;
        }
    }
}
