using System;
using System.Collections.Generic;
using System.Text;


    [Serializable]

     /* Simplest realisation of vocabulary with sorted set.
     *  Used in this project to prove efficiency of compressed vocabulary.
     */
    class Vocabulary
    {

        private SortedSet<string> dict = new SortedSet<string>();

        public Vocabulary() { }

        //Amount of words in Vocabulary
        public int Size()
        {
            return dict.Count;
        }
        
        //Check if Vocabulary contains word
        public bool Contains(string word)
        {
            return dict.Contains(word);
        }

        //Add one word to Vocabulary
        public void Add(string word)
        {
            if (!dict.Contains(word)) dict.Add(word);
        }

        //Add multiple words to Vocabulary
        public void AddRange(string[] words)
        {
            foreach(string word in words) if (!dict.Contains(word)) dict.Add(word);
        }

    }

