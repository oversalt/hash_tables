using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    public class Quadratic<K, V> : A_OpenAddressing<K, V>
        where K : IComparable<K>
        where V : IComparable<V>
    {
        protected override int GetIncrement(int iAttempt, K key)
        {
            const double c1 = 0.5;
            const double c2 = 0.5;

            //Implement the formula for quadratic from the in-class notes
            //Recall that c2 cannot be 0. 
            return (int)(c1 * iAttempt + c2 * Math.Pow(iAttempt, 2));
        }

        public Quadratic()
        {
            //Set the load factor to 0.50 to ensure that we don't get an infinite loop
            //while incrementing. This is only necessary for the Quadratic implementation.
            this.dLoadFactor = 0.50;
        }
    }
}
