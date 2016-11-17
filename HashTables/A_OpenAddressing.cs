using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    public abstract class A_OpenAddressing<K,V>: A_Hashtable<K,V>
        where K : IComparable<K>
        where V : IComparable<V>
    {
        //Abstract method to get the increment value
        protected abstract int GetIncrement(int iAttempt, K key);
        private PrimeNumber pn = new PrimeNumber();
        public A_OpenAddressing()
        {
            oDataArray = new object[pn.GetNextPrime()];
        }

        public override V Get(K key)
        {
            int iInitialHash = HashFunction(key);
            V returnVal = default(V);
            int iCurrentLocation = iInitialHash;
            int iAttempt = 1;
            bool found = false;

            //While the value is not found and the current location is not null
            while(!found && oDataArray[iCurrentLocation] != null)
            {
                //If it is a key value pair
                if(oDataArray[iCurrentLocation].GetType() == typeof(KeyValue<K, V>))
                {
                    KeyValue<K, V> kv = (KeyValue<K, V>)oDataArray[iCurrentLocation];
                    //Is this the item we are looking for?
                    if (kv.Key.CompareTo(key) == 0)
                    {
                        //Get a reference to the value ot return
                        returnVal = kv.Value;
                        //set found to true
                        found = true;
                    }
                }
                //Increment to the next location
                iCurrentLocation = iInitialHash + GetIncrement(iAttempt++, key);
                //If at the end of the table, wrap back up to the top
                iCurrentLocation %= HTSize;
            }
            if(!found)
            {
                throw new ApplicationException("Value does not exist");
            }
            return returnVal;
        }

        public override void Add(K key, V value)
        {
            //How many attempts were made to increment
            int iAttempt = 1;
            //Get the initial hash (location) of the key
            int iInitialHash = HashFunction(key);
            //The current location we are trying to add.
            int iCurrentLocation = iInitialHash;
            //Position to add
            int iPositionToAdd = -1;

            //While the current location is already full
            while(oDataArray[iCurrentLocation] != null)
            {
                //if the current value is a key-value pair.
                if(oDataArray[iCurrentLocation].GetType() == typeof(KeyValue<K,V>))
                {
                    //Check to see if current locations value is the same as the value we're adding
                    KeyValue<K, V> kv = (KeyValue<K,V>) oDataArray[iCurrentLocation];
                    if(kv.Key.CompareTo(key) == 0)
                    {
                        //Throw an exception
                        throw new ApplicationException("Item already exists");
                    }
                }
                else //It is a tombstone
                {
                    //Mark location as a potential location to add
                    if(iPositionToAdd == -1)
                    {
                        iPositionToAdd = iCurrentLocation;
                    }
                }
                //Increment to the next location
                iCurrentLocation = iInitialHash + GetIncrement(iAttempt++, key);
                //If at the end of the table, wrap back up to the top
                iCurrentLocation %= HTSize;
                iNumCollisions++;
            }

            //We have found a position to add, a null or tombstone
            if(iPositionToAdd == -1)
            {
                //No tombstone found so add at a null
                iPositionToAdd = iCurrentLocation;
            }

            //Add the key value pair at position to add
            KeyValue<K, V> kvNew = new KeyValue<K, V>(key, value);
            oDataArray[iPositionToAdd] = kvNew;
            iCount++;

            if(isOverloaded())
            {
                ExpandHashTable();
            }
        }

        private void ExpandHashTable()
        {
            object[] oOldArray = oDataArray;
            //Create a new array, with the next prime number size
            oDataArray = new object[pn.GetNextPrime()];
            //Reset the attributes
            iCount = 0;
            iNumCollisions = 0;

            //Loop thorugh the existing array and re-hash each key-value 
            for (int i = 0; i < oOldArray.Length; i++)
            {
                if(oOldArray[i] != null)
                {
                    //it is tombstone or kv pair
                    if (oOldArray[i].GetType() == typeof(KeyValue<K,V>))
                    {
                        //Get a reference to the current key-value pair
                        KeyValue<K, V> kv = (KeyValue<K, V>)oOldArray[i];
                        //Add the key value pair to the new array
                        this.Add(kv.Key, kv.Value);
                    }
                }
            }
            
        }

        private bool isOverloaded()
        {
            return iCount / (double) HTSize > dLoadFactor;
        }

        public override void Remove(K key)
        {
            int iInitialHash = HashFunction(key);
            int iCurrentLocation = iInitialHash;
            int iAttempt = 1;
            bool found = false;

            //While the value is not found and the current location is not null
            while (!found && oDataArray[iCurrentLocation] != null)
            {
                //If it is a key value pair
                if (oDataArray[iCurrentLocation].GetType() == typeof(KeyValue<K, V>))
                {
                    KeyValue<K, V> kv = (KeyValue<K, V>)oDataArray[iCurrentLocation];
                    //Is this the item we are looking for?
                    if (kv.Key.CompareTo(key) == 0)
                    {
                        //Set current location to tombstone
                        oDataArray[iCurrentLocation] = new Tombstone();
                        //set found to true
                        found = true;
                        //Decrement the count
                        iCount--;
                    }
                }
                //Increment to the next location
                iCurrentLocation = iInitialHash + GetIncrement(iAttempt++, key);
                //If at the end of the table, wrap back up to the top
                iCurrentLocation %= HTSize;
            }

            if(!found)
            {
                throw new KeyNotFoundException("Key " + key.ToString() + " not found.");
            }
        }


        public override void Clear()
        {

        }

        public override IEnumerator<V> GetEnumerator()
        {
            return new OpenAddressingEnumerator(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < oDataArray.Length; i++)
            {
                sb.Append("Bucket " + i + ": ");
                if (oDataArray[i] != null)
                {
                    if (oDataArray[i].GetType() == typeof(Tombstone))
                    {
                        sb.Append("Tombstone");
                    }
                    else
                    {
                        KeyValue<K, V> kv = (KeyValue<K, V>)oDataArray[i];
                        sb.Append(kv.Value.ToString());
                    }

                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
        private class OpenAddressingEnumerator : IEnumerator<V>
        {
            A_OpenAddressing<K, V> ht;
            KeyValue<K, V> kv = null;
            int iCurrentBucket = -1;

            public OpenAddressingEnumerator(A_OpenAddressing<K,V> ht)
            {
                this.ht = ht;
            }

            public V Current
            {
                get
                {
                    return kv.Value;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return kv.Value;
                }
            }

            public void Dispose()
            {
                ht = null;
                iCurrentBucket = 0;
                kv = null;
            }



            public bool MoveNext()
            {
                bool found = false;
                iCurrentBucket++;
                while(!found && iCurrentBucket < ht.oDataArray.Length)
                {
                    if(ht.oDataArray[iCurrentBucket] == null || ht.oDataArray[iCurrentBucket].GetType() == typeof(Tombstone))
                    {
                        iCurrentBucket++;
                    }
                    else
                    {
                        found = true;
                        kv = (KeyValue<K, V>)ht.oDataArray[iCurrentBucket];
                    }
                }
                return found;
            }

            public void Reset()
            {
                iCurrentBucket = 0;
                kv = null;
            }
        }
    }

    
}
