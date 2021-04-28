# CompressedVocabularyAndIndex
Data structures, which allow to save Compressed Vocabulary and Reverse Index, which lead to higher memory effiency.

To compress vocabulary:
     * Words are saved in array of strings.
     * Each of this string called "block" and contains amount of words which must be set by user.
     * Before each word it's length is stored in order to split and find words in string.
     !!! For proper work, sorted array of words can be added only once.
     
To compress index:
     * Variable byte encoding is used.
     * Also, instead of saving docId, this index saves interval between docIds.
     * Exaple: if we add 2 words with docIds 50 and 150, encoded 50 and 100 (150-50=100) will be saved.
     !!! Works properly only if words with smaller docIds added first.
     
Also non-compressed Index and Vocabulary are added in order to compare compressed and non-compressed structures and prove efficiency of compression.
