using System;
using System.Collections.Generic;
using System.Linq;



    [Serializable]
    /* Compressed index class.
     * To compress index, variable byte encoding is used.
     * It allows to save numbers in more memory-efficient way than using int.
     * Also, instead of saving docId, this index saves interval between docIds.
     * Exaple: if we add 2 words with docIds 50 and 150, encoded 50 and 100 (150-50=100) will be saved.
     * 
     * !!! Works properly only if words with smaller docIds added first.
     */
    class CompressedReverseIndex
    {
        //Sorted dictionary, where words and encoded intervals are stored.
        SortedDictionary<string, HashSet<byte>> cri = new SortedDictionary<string, HashSet<byte>>();
        public CompressedReverseIndex() {}

        //Method of adding word.
        public void Add(string word, int docId)
        {
            if(!cri.ContainsKey(word)) 
            {
                cri.Add(word, new HashSet<byte>() { } ); //Add word and empry hashSet of encoded intervals
                foreach (byte b in VBEncode(docId)) //Write docId in proper format
                cri[word].Add(b);
                
            }
            if(cri.ContainsKey(word))
            {
                int interval = 0;
                List<byte> oneNum = new List<byte>(); //Create list, where encoded number will be stored
                foreach (byte b in cri[word])
                {
                    oneNum.Add((byte)(b & 0b_0111_1111)); //Add byte to List
                    if ((b & 0b_1000_0000) == 0b_1000_0000) //If last byte of number
                    {
                        interval += VBDecode(oneNum.ToArray()); //Add to interval to find last added docId
                        oneNum.Clear(); //Clear number list
                    }
                }
                if (docId - interval != 0) //If this word`s docId is not in index
                {
                    foreach (byte b in VBEncode(docId - interval))
                        cri[word].Add(b); //Add encoded number
                }
            }

        }

        //Encode integer (won`t work if num>2^28)
        public byte[] VBEncode(int num)
        {
            List<byte> list = new List<byte>(); //Create byte list 

            for(int i=0; i<4; i++) //Get last 7 bytes of num 4 times 
            {
                    byte last_7_numbers = (byte) (num & 0b_0111_1111); //Get last 7 bits
                    num = num >> 7; //Bitwise shift
                    if (i == 0) last_7_numbers = (byte) ((int)last_7_numbers | 0b_1000_0000); //Make 1st bit=1 if we encoded last 7 bits of integer 
                    if (last_7_numbers != 0) list.Add(last_7_numbers); //Add non-zero bytes
                
            }
            //Save reversed list
            list.Reverse();
            return list.ToArray();
        }

        //Decode integer
        public int VBDecode(byte[] vb)
        {
            int result = 0;
            int counter = 0;
            foreach (byte b in vb.Reverse()) //Reverse number
            {
                byte b_tmp = b;//Copy of one byte of array
                for(int i=7; i>=0; i--) //
                {
                /* First bit of last encoded byte from byte array used to indicate if this is last, so we don`t add
                 * it in decoded result by checking if if (i != 0)
                 * ***
                 * If bit==1, add proper power of 2 to result.
                 */
                if (i != 0) { result += (b_tmp & 0b_0000_0001) * (int)Math.Pow(2, counter); counter++;} 
                b_tmp >>= 1; //Right shift
                }
            }
            return result;
        }

        /* Returns SortedSet with docIds which contain particular word.
         * If there is no word in index, returns empty SortedSet
         */
        public SortedSet<int> Search(string query) 
        {
           if(!cri.ContainsKey(query)) return new SortedSet<int>(); //Return empty set if there is no word we need
           else
            {
                SortedSet<int> setOfDocIds = new SortedSet<int>(); //Init set which will be returned
                int docId = 0; //create variable of current docId
                List<byte> oneNum = new List<byte>(); //One encoded number
                foreach (byte b in cri[query])
                {
                    oneNum.Add((byte)(b)); //Add byte
                    if (((int)b & 0b_1000_0000) == 0b_1000_0000) //If bit is last
                    {
                        docId += VBDecode(oneNum.ToArray()); //Add interval to docId
                        setOfDocIds.Add(docId); //Push result to setOfDocIds
                       oneNum.Clear(); //Clear encoded number list
                    }   
                }

                return setOfDocIds;
            }

        }

        //Return search result in pretty and easily readable string.
        public string StringifySearchResult(string query)
        {
            //Display search word
            string result = "Word: " + query + "\nDocIds : [ ";
            foreach (int docId in Search(query))
            {
                result += docId + " "; //Add all docIds from Search(...) method
            }
            return result + "]"; 
        }

    }



