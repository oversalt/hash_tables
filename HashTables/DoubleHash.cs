using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    public class DoubleHash<K, V> : A_OpenAddressing<K, V>
        where K : IComparable<K>
        where V : IComparable<V>
    {
        protected override int GetIncrement(int iAttempt, K key)
        {
            //Note that we multiply by the attempt number to get the TOTAL increment
            //from the initial hash location.
            return (1 + Math.Abs(key.GetHashCode()) % (HTSize - 1)) * iAttempt;
        }
    }
}
