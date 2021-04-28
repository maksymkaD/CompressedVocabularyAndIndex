using System;


    [Serializable] 
    /* Compressed vocabulary class.
     * To compress vocabulary, words are saved in array of strings.
     * Each of this string contains BlockLength words.
     * Before each word it`s length is stored in order to split and find words in string.
     * 
     * !!! For proper work, sorted array of words can be added only once.
     */
    class CompressedVocabulary
    {
        //Array where words are stored
        public string[] vocabulary = new string[0]; 
        public int BlockLength { get; } //Length of block
        //Constructor, with ability to  BlockLength parameter
        public CompressedVocabulary(int bl)
        {
            this.BlockLength = bl;
        }

        /* Method which adds range of words.
         */
        public void Add(string[] words)
        {
            Array.Sort(words); //Sort words (in case they weren`t sorted)
            vocabulary = new string[(int)Math.Ceiling((double)words.Length / BlockLength)]; //Initialize block array with proper length
            int blockCount = 0;
            for (int i = 0; i < words.Length; i++) //For each word
            {
                vocabulary[blockCount] += words[i].Length + words[i]; //Add lenth+word to block
                if ((i + 1) % BlockLength == 0) blockCount++; //Increase block counter to start filling next one
        }
        }


        /* Search result.
         * Returns true if vocabulary contains word, else false.
         */
        public bool Search(string word)
        {
            //Use binary search to find a block where 
            int firstIndex = 0;
            int lastIndex = vocabulary.Length - 1;

            while (firstIndex <= lastIndex)
            {
                int middleIndex = (firstIndex + lastIndex) / 2;
                
                string block = vocabulary[middleIndex]; // Get block in the middle
                string first = block.Substring(FindInt(block).ToString().Length, FindInt(block)); //Get it`s first word
                
                if (string.Compare(word, first) == -1) //If word we want to find is less then first word in block
                {
                    lastIndex = middleIndex - 1;
                }
                else // If word we want to find is bigger then first word in block
                {
                    if (first == word) return true; //Return true (which means that vocabulary contains word) if block`s first word equals word we`re looking for

                    block = block.Substring(FindInt(block).ToString().Length+first.Length); //Remove first word from block 
                    for (int i = 0; i < BlockLength - 1; i++) //Compare other BlockLength-1 words in block
                    {
                        
                        int length = FindInt(block); //Get length
                        string blockElement = block.Substring(FindInt(block).ToString().Length, length); //Get word
                        block = block.Substring(length.ToString().Length + blockElement.Length); //Remove this word from block 
                           
                        if (length == word.Length) if (blockElement == word) return true; //Return true if i word in block equals word we`re looking for

                    }
                    firstIndex = middleIndex + 1;

                }
            }
            return false; //Return false (no word in vocabulary) 
        }
        
        /* Method which finds int at beginning of the string.
         * Returns -1 if there is no int at beginning of the string.
         */
        static public int FindInt(string str)
        {
            int result = -1;
            for (int i = 0; i < str.Length; i++)
            {
                try
                {
                    result = Int32.Parse(str.Substring(0, i + 1));
                }
                catch { break; }
            }
            return result;

        }





    }

