using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    public abstract class A_Hashtable<K,V> : I_Hashtable<K,V> 
        where K: IComparable<K>
        where V: IComparable<V>
    {
        //In the case of chaining, this will hold secondary data structure references
        //(we will use ArrayLists in class)
        //In the case of open-addressing (probing algorithms) the array will store
        //key-value pairs directly.
        protected object[] oDataArray;

        //Store the number of elements in the array
        protected int iCount;

        //Load factor - used to track the maximum percentage full that we will allow the array
        //to fill. The factor 0.72 is used by Microsoft for their hashtable
        protected double dLoadFactor = 0.72;

        //Collision count - mostly for statistical purposes
        protected int iNumCollisions = 0;

        #region Properties
        public int Count
        {
            get
            {
                return iCount;
            }
        }

        public int NumCollisions
        {
            get
            {
                return iNumCollisions;
            }
        }

        //Normally not exposed to the user, but makes testing easier
        public int HTSize
        {
            get
            {
                return oDataArray.Length;
            }
        }
        #endregion

        #region Hash Function
        protected int HashFunction(K key)
        {
            //All objects in the C# world have a GetHashCode method.
            //Note that the key object can override the GetHashCode method.
            return Math.Abs(key.GetHashCode() % HTSize);
        }

        #endregion

        #region I_Hashtable Implemenation
        public abstract V Get(K key);

        public abstract void Add(K key, V value);

        public abstract void Remove(K key);

        public abstract void Clear();
        #endregion

        public abstract IEnumerator<V> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
