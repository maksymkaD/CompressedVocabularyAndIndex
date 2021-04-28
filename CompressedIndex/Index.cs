using System;
using System.Collections.Generic;
using System.Text;


    [Serializable]
    /* Simplest realisation of index with sorted dictionary.
     * Used in this project to prove efficiency of compressed index.
     */
    class Index
    {
        //Sorted dictionary which contains words and list of document ids where this words can be found
        private SortedDictionary<string, List<int>> index = new SortedDictionary<string, List<int>>();
        public Index() {}

        //Method of adding word to index
        public void Add(string word, int docId)
        {
            if (!index.ContainsKey(word))
            {
                List<int> set = new List<int>(); //Initialise new list
                set.Add(docId); //Add docId to that list
                index.Add(word, set); //Add new word with list containing one document to dictionary
            }
            else
            {
                //Add new docId to list of word`s documents if it wasnt previously added
                if (!index[word].Contains(docId)) index[word].Add(docId); 
            }
        }

        /* Returns array of docIds as a search results.
         * If there is no such word in index, returns empty array.
         */
        public int[] Search(string word)
        {
            if (!index.ContainsKey(word)) return new int[0]; 
            else
            {
                int[] result = index[word].ToArray();
                Array.Sort(result);
                return result;
            }
        }
    }

