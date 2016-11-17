using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTables
{
    //K --> Generic value representing the data type of the key
    //V --> Generic value representing the data type of the value
    public interface I_Hashtable<K,V>: IEnumerable<V>
        where K: IComparable<K>
        where V: IComparable<V>
    {
        /// <summary>
        /// Return a value from the hashtable
        /// </summary>
        /// <param name="key">The key of the value to return</param>
        /// <returns>The value of the item returned</returns>
        V Get(K key);

        /// <summary>
        /// Add the key and value as a key-value pair to the hashtable
        /// </summary>
        /// <param name="key">Determines the location in the array</param>
        /// <param name="value">Value to store at that location</param>
        void Add(K key, V value);

        /// <summary>
        /// Remove the value form the hashtable based on the key
        /// </summary>
        /// <param name="key">Key of the item to remove. Determines location of the
        /// key-value pair</param>
        void Remove(K key);

        /// <summary>
        /// Remove all values from the hashtable and initialize to the default array size.
        /// </summary>
        void Clear();


    }
}
