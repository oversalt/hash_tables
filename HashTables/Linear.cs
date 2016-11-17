using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    class Linear<K, V> : A_OpenAddressing<K, V>
        where K : IComparable<K>
        where V : IComparable<V>
    {

        protected override int GetIncrement(int iAttempt, K key)
        {
            int iIncrement = 1;
            return iAttempt * iIncrement;
        }

        public override void Remove(K key)
        {
            int iInitialHash = HashFunction(key);
            int iCurrentLocation = iInitialHash;
            int iAttempt = 1;
            bool found = false;
            bool moreInCollision = false;
            int[] oPastLocations = new int[HTSize];
            int i = 0;

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
                        //Question 1
                        int iCheckAttempt = 1;
                        int iCheck = iCurrentLocation;
                        iCheck = iCheck + GetIncrement(iCheckAttempt++, key);
                        iCheck %= HTSize;
                        while(!moreInCollision && iCheckAttempt < HTSize)
                        {
                            try
                            {
                                if (HashFunction(((KeyValue<K,V>) oDataArray[iCheck]).Key) == HashFunction(key))
                                {
                                    moreInCollision = true;
                                }
                                else
                                {
                                    break;  //Hacky but don't care the moment, DO NOT TOUCH, IGNORE THIS, IT'S FINE
                                }
                            }
                            catch (Exception e)
                            {
                                //This will only happen if the next position in oDataArray is null
                                //stops the loop because we must be at the end of the chain
                                //And yes, it is hacky. But it works.
                                break;
                            }
                        }

                        if(moreInCollision)
                        {
                            //Set location to a tombstone if there is still more in the collision chain
                            oDataArray[iCurrentLocation] = new Tombstone();
                        }
                        else
                        {
                            //Set current location to null if there is no more items in the collision sequence
                            oDataArray[iCurrentLocation] = null;
                            foreach(int past in oPastLocations)
                            {
                                //If there are tombstones before the current position in the chain,
                                //make them null
                                if(oDataArray[past].GetType() == typeof(Tombstone))
                                {
                                    oDataArray[past] = null;
                                }
                            }
                        }
                        //set found to true
                        found = true;
                        //Decrement the count
                        iCount--;
                    }
                }
                //Recording the locations in the collision chain
                oPastLocations[i] = iCurrentLocation;
                //Increment to the next location
                iCurrentLocation = iInitialHash + GetIncrement(iAttempt++, key);
                //If at the end of the table, wrap back up to the top
                iCurrentLocation %= HTSize;
                
            }

            if (!found)
            {
                throw new KeyNotFoundException("Key " + key.ToString() + " not found.");
            }
        }
    }
}
