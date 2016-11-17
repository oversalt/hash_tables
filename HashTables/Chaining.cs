using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace HashTables
{
    public class Chaining<K, V> : A_Hashtable<K, V>
        where K : IComparable<K>
        where V : IComparable<V>
    {

        //Initial Size for oDataArray. Note that the size does not have to be a prime number as
        //we are not using a probe sequence to find an open spot. This means that we don't worry
        //about infinite loops.
        private const int iInitialSize = 4;
        //Track how many buckets contain arraylists.
        private int iBucketCount = 0;

        #region Constructors
        public Chaining()
        {
            this.oDataArray = new object[iInitialSize];
            //Use default loadfactor.
            this.dLoadFactor = 0.72;
        }

        public Chaining (int iInitialSize, double dLoadFactor)
        {
            //Set up the initial hash table array
            this.oDataArray = new object[iInitialSize];
            this.dLoadFactor = dLoadFactor;
        }
        #endregion

        public override void Add(K key, V value)
        {
            //Determine the initial position in the hash table
            int iInitialHash = HashFunction(key);
            //Create a key value pair object with the data passed in.
            KeyValue<K, V> kv = new KeyValue<K, V>(key, value);
            //A reference to the arraylist to add to.
            ArrayList alCurrent = null;

            //If the hashcode location does not contain an arraylist
            if(oDataArray[iInitialHash] == null)
            {
                //Create a new arraylist
                alCurrent = new ArrayList();
                //Put array list into the hashtable array
                oDataArray[iInitialHash] = alCurrent;
                //Increment the bucket count
                iBucketCount++;
            }
            else
            {
                //Get a reference to the existing arraylist
                alCurrent = (ArrayList) oDataArray[iInitialHash];
                //If the current arraylist already contains the value to add, throw an exception
                if(alCurrent.Contains(kv))
                {
                    throw new ApplicationException("Key already exists in hashtable.");
                }
                //increment the number of collisions
                iNumCollisions++;
            }

            //Add the item to the arraylist
            alCurrent.Add(kv);
            iCount++;

            //Check if the array is overloaded
            if (isOverloaded())
            {
                ExpandHashTable();
            }
        }

        private void ExpandHashTable()
        {
            object[] oOldArray = oDataArray;
            oDataArray = new object[HTSize * 2];
            //Reset the attributes
            iCount = 0;
            iNumCollisions = 0;
            iBucketCount = 0;

            //Loop thorugh the existing array and re-hash each key-value 
            for (int i = 0; i < oOldArray.Length; i++)
            {
                if (oOldArray[i] != null)
                {
                    foreach (KeyValue<K, V> kv in (ArrayList) oOldArray[i])
                    {
                        kv.ResetAccess();
                        this.Add(kv.Key, kv.Value);
                    }
                }
            }
        }

        private bool isOverloaded()
        {
            return iBucketCount / (double) HTSize > dLoadFactor;
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override V Get(K key)
        {
            /*
             * V vReturn = default(V);
             * int iInitialHash = HashFunction(key);
             * int iCurrentLocation = iInitialHash;
             * ArrayList alCurrent = (ArrayList) oDataArray[iInitialHash];
             * int iIndexOfValue = 0;
             * 
             * if(alCurrent != null)
             * {
             *      //Gets the item's location from the arraylist (returns -1 if not found)
             *      iIndexOfValue = alCurrent.IndexOf(new KeyValue<K,V>(key, default(V)));
             *      if(iIndexOfValue >= 0)
             *      {
             *          KeyValue<K,V> kv = (KeyValue<K,V>) alCurrent[iIndexOfValue];
             *          vReturn = kv.Value;
             *      }
             * }
             * 
             * if(alCurrent == null || iIndexOfValue == -1)
             * {
             *      throw new ApplicationException("Failed to Get: Value does not exist");
             * }
             * 
             * return vReturn;
             */

            
            V vReturn = default(V);
            int iInitialHash = HashFunction(key);
            int iCurrentLocation = iInitialHash;
            bool found = false;
            int iLargestLocation = -1;
            int iLargetAccess = -1;
            int iCurrentArrayListLocation = 0;
            KeyValue<K, V> temp;

            if (oDataArray[iCurrentLocation] != null)
            {
                foreach (KeyValue<K, V> kv in (ArrayList)oDataArray[iCurrentLocation])
                {
                    if (kv.Key.CompareTo(key) == 0)
                    {
                        vReturn = kv.Value;
                        found = true;
                        if (kv.Access > iLargetAccess && iLargetAccess >= 0)
                        {
                            //Put the previously most accessed one in a temp variable
                            temp = (KeyValue<K, V>)((ArrayList)oDataArray[iCurrentLocation])[iLargestLocation];
                            //Put the current KeyValue in the new location
                            ((ArrayList)oDataArray[iCurrentLocation])[iLargestLocation] = ((ArrayList)oDataArray[iCurrentLocation])[iCurrentArrayListLocation];
                            //Put the temp into the current location
                            ((ArrayList)oDataArray[iCurrentLocation])[iCurrentArrayListLocation] = temp;
                        }
                        break;
                    }
                    else
                    {
                        if (kv.Access > iLargetAccess)
                        {
                            iLargetAccess = kv.Access;
                            iLargestLocation = iCurrentArrayListLocation++;
                        }
                        else
                        {
                            iCurrentArrayListLocation++;
                        }
                    }
                }
            }

            if (!found)
            {
                throw new ApplicationException("Value does not exist");
            }

            return vReturn;
        }

        public override IEnumerator<V> GetEnumerator()
        {
            return new ChainingEnumerator(this);
        }

        public override void Remove(K key)
        {
            int iInitialHash = HashFunction(key);
            int iCurrentLocation = iInitialHash;
            ArrayList alCurrent = (ArrayList)oDataArray[iCurrentLocation];
            bool found = false;

            if (alCurrent != null)
            {
                KeyValue<K, V> kv = new KeyValue<K, V>(key, default(V));
                if(alCurrent.Contains(kv))
                {
                    alCurrent.Remove(kv);
                    found = true;
                    iCount--;
                    //Check the size of the arraylist
                    if(alCurrent.Count ==0)
                    {
                        oDataArray[iCurrentLocation] = null;
                        iBucketCount--;
                    }
                }
                
            }

            if (alCurrent == null || !found)
            {
                throw new ApplicationException("Key does not exist");
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //Loop through each bucket
            for (int i = 0; i < HTSize; i++)
            {
                //Add the bucket number to the string
                sb.Append("Bucket " + i.ToString() + ": ");
                //check if an arraylist exists at this location
                if (oDataArray[i] != null)
                {
                    ArrayList alCurrent = (ArrayList)oDataArray[i];
                    foreach (KeyValue<K, V> kv in alCurrent)
                    {
                        sb.Append(kv.Value.ToString() + "Access count: " + kv.Access + " --> ");
                    }
                    sb.Remove(sb.Length - 5, 5);
                }
                sb.Append("\n");

            }
            return sb.ToString();
        }

        private class ChainingEnumerator : IEnumerator<V>
        {
            private Chaining<K, V> parent = null;
            private ArrayList alCurrent = null;
            private KeyValue<K, V> kvCurrent = null;
            private int iBucketCurrent = 0;
            private int iArrayIndex = 0;

            public ChainingEnumerator(Chaining<K,V> parent)
            {
                this.parent = parent;
                Reset();
            }

            public V Current
            {
                get
                {
                    return kvCurrent.Value;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return kvCurrent.Value;
                }
            }

            public void Dispose()
            {
                parent = null;
                alCurrent = null;
                kvCurrent = null;
                iBucketCurrent = 0;
                iArrayIndex = 0;
            }

            public bool MoveNext()
            {
                bool bMoved = false;
                
                while(parent.oDataArray[iBucketCurrent] == null)
                {
                    iBucketCurrent++;
                }

                while(!bMoved && iBucketCurrent < parent.HTSize)
                {
                    if (parent.oDataArray[iBucketCurrent] != null)
                    {
                        alCurrent = (ArrayList)parent.oDataArray[iBucketCurrent];
                        if (iArrayIndex < alCurrent.Count)
                        {
                            if (alCurrent[iArrayIndex] != null)
                            {
                                bMoved = true;
                                kvCurrent = ((KeyValue<K, V>)alCurrent[iArrayIndex++]);
                            }

                        }
                        else
                        {
                            iBucketCurrent++;
                            iArrayIndex = 0;
                        }
                    }
                    else
                    {
                        iBucketCurrent++;
                        iArrayIndex = 0;
                    }
                }

                return bMoved;
            }

            public void Reset()
            {
                iBucketCurrent = 0;
                kvCurrent = null;
            }
        }
    }
}
