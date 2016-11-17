using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HashTables
{
    class Program
    {
        static void LoadDataFromFile(A_Hashtable<Person, Person> ht)
        {
            StreamReader sr = new StreamReader(File.Open("People.txt", FileMode.Open));
            string sInput = "";

            try
            {
                //Read a line from the file
                while ((sInput = sr.ReadLine()) != null)
                {
                    try
                    {
                        char[] cArray = { ' ' };
                        string[] sArray = sInput.Split(cArray);
                        int iSSN = Int32.Parse(sArray[0]);
                        Person p = new Person(iSSN, sArray[2], sArray[1]);
                        ht.Add(p, p);
                    }
                    catch (ApplicationException ae)
                    {
                        //Console.WriteLine("Exception: " + ae.Message);
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            sr.Close();
        }

        static void TestAdd(A_Hashtable<int,string> ht)
        {
            ht.Add(2345, "Rob");
            //ht.Add(2345, "Rob");
            ht.Add(2453, "Ron");
            ht.Add(7648, "Rick");
            Console.WriteLine(ht.ToString());
            Console.WriteLine("Num of collisions: " + ht.NumCollisions);
        }

        static void TestHT(A_Hashtable<Person, Person> ht)
        {
            LoadDataFromFile(ht);
            Console.WriteLine(ht);
            Console.WriteLine("Hash table type " + ht.GetType().ToString());
            Console.WriteLine("# of People = " + ht.Count);
            Console.WriteLine("Number of collisions: " + ht.NumCollisions + "\n");
        }

        static void TestGet(A_Hashtable<int, string> ht)
        {
            try
            {
                Console.WriteLine(ht.Get(2345));
                Console.WriteLine(ht.Get(3756));
                Console.WriteLine(ht.Get(1000));
            }
            catch (ApplicationException ae)
            {
                Console.WriteLine(ae.Message);
            }
            
        }

        static void TestRemove(A_Hashtable<int, string> ht)
        {
            try
            {
                //Console.WriteLine(ht);
                ht.Remove(2453);    //Ron
                //ht.Remove(2345);    //Rob
                ht.Remove(7648);      //Rick
                Console.WriteLine(ht);
                //ht.Remove(100000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void TestEnumerator(A_Hashtable<int, string> ht)
        {
            foreach(string v in ht)
            {
                Console.WriteLine(v.ToString());
            }
        }

        static void Main(string[] args)
        {
            Chaining<int, string> chain = new Chaining<int, string>();
            TestAdd(chain);
            //TestEnumerator(chain);
            TestGet(chain);
            //TestRemove(chain);

            //Chaining<Person, Person> ht = new Chaining<Person, Person>();
            //TestHT(ht);
            /*Open Addressing Test Code*/
            //Linear<int, string> ht = new Linear<int, string>();
            //TestAdd(ht);
            //TestRemove(ht);
            //TestEnumerator(ht);
            //TestGet(ht);
            //Linear<Person, Person> htLinear = new Linear<Person, Person>();
            //TestHT(htLinear);

            //Quadratic<Person, Person> htQuad = new Quadratic<Person, Person>();
            //TestHT(htQuad);

            //DoubleHash<Person, Person> htDouble = new DoubleHash<Person, Person>();
            //TestHT(htDouble);

            //TestRemove(ht);
        }


    }
}
