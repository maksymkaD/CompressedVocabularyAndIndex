using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;


    class Program
    {
        static void Main(string[] args)
        {
           

            //File path array
            string[] pathArray = new[] {
            //Enter path to file(s) from which you want to compose a vocabulary/index
                    "D:\\avidreaders.ru__animal-farm-a-fairy-story.fb2",
                    "D:\\meta.fb2",
            };
            
           
            CompressedReverseIndex cri = new CompressedReverseIndex(); //Init CompressedReverseIndex


            Console.WriteLine("TEST VIRTUAL BYTE ENCODING AND DECODING WITH RANDOM NUMBERS");
            var rand = new Random();  
            for (int i=0; i<10; i++)
            {
            int before = rand.Next(0, 1000); //Get random in 0-1000 range
            int after = cri.VBDecode(cri.VBEncode(before)); //Encode this random number and decode result
            Console.WriteLine("BEFORE ENCODING: " + before + " | AFTER ENCODING: " + after);
            }

            Index idx = new Index(); //Init Index
            HashSet<string> set = new HashSet<string>();

            //Read every document from paths
            for (int i = 0; i < pathArray.Length; i++)
            {
            string file_content = System.IO.File.ReadAllText(pathArray[i]);
            string[] contentStrs = Regex.Split(file_content, "\\s+<*>\\s+|[^a-zA-Z]+"); //Regex which tokenizes text, removes XML-tags and numbers
                foreach (string str in contentStrs)
                {
                if (!String.IsNullOrWhiteSpace(str)) //Check if token is not empty string or contains only whitespaces
                {
                    /* Add word token to HastSet, this is needed to add words in
                     * compressed vocabulary later (because it doesn`t support adding one word)
                     */
                    if (!set.Contains(str))
                    {
                        set.Add(str);
                    }
                    cri.Add(str, i); //Add word to CompressedReverseIndex
                    idx.Add(str, i); //Add word to Index
                }
                }
            }

            Vocabulary vcb = new Vocabulary(); //Init vocabulary
            CompressedVocabulary cv = new CompressedVocabulary(4); //Init CompressedVocabulary
            string[] arr = new string[set.Count]; 
            set.CopyTo(arr); // Move values from HashSet of lexemes to array
            cv.Add(arr); 
            vcb.AddRange(arr);  //Add array to compressed and non-compressed vocabulary

            //Write all vocabularies and indexes somewhere (in this case, folder on disc D) to make sure that compression actually works
            WriteFile<CompressedVocabulary>("D:\\CompressedVsUsualIndex\\COMPRESSED_VOCAB.txt", cv);
            WriteFile<Vocabulary>("D:\\CompressedVsUsualIndex\\NON_COMPRESSED_VOCAB.txt",vcb);
            WriteFile<CompressedReverseIndex>("D:\\CompressedVsUsualIndex\\COMPRESSED_INDEX.txt",cri);
            WriteFile<Index>("D:\\CompressedVsUsualIndex\\NON_COMPRESSED_INDEX.txt",idx);
     


            //Search in compressed index/vocabulary
            while (true)
            {
                Console.Write("Enter word: ");
                string query = Console.ReadLine();
                Console.WriteLine("VOCABULARY SEARCH: "+cv.Search(query));
                Console.WriteLine("INDEX SEARCH\n"+cri.StringifySearchResult(query));
            }
          

        }

        //Method which serializes objects using binary formatter
        static private void WriteFile<T>(string path, T t)
        {
        IFormatter form = new BinaryFormatter();//Create formatter to serialize indexes and vocabularies 
        Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write); //create file stream
        form.Serialize(stream, t); //create file stream
        stream.Close();
        }
               

    }