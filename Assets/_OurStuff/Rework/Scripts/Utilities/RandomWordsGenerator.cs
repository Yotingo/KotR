using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWordsGenerator
{
    public string AdjectiveNoun ()
    {
        string chosenWords;

        // Load adjectives as a List
        TextAsset txtAdjectives = (TextAsset)Resources.Load("English Adjectives");
        List<string> adjectivesList = new List<string>(txtAdjectives.text.Split("\n"[0])); 

        // Remove empty words
        for (int i = 0; i < adjectivesList.Count; i++) 
        {
            if (adjectivesList[i] == "")
            {
                adjectivesList.RemoveAt(i);
                i--;
            }
        }

        // Load adjectives as a List
        TextAsset txtNouns = (TextAsset)Resources.Load("English Nouns");
        List<string> nounsList = new List<string>(txtNouns.text.Split("\n"[0]));

        // Remove empty words
        for (int i = 0; i < nounsList.Count; i++)
        {
            if (nounsList[i] == "")
            {
                nounsList.RemoveAt(i);
                i--;
            }
        }

        // Choose random words from the lists
        string chosenAdjective = adjectivesList[Random.Range(0, adjectivesList.Count - 1)];
        string chosenNoun = nounsList[Random.Range(0, nounsList.Count - 1)];

        chosenWords = chosenAdjective + chosenNoun;

        return chosenWords;
    }
}
