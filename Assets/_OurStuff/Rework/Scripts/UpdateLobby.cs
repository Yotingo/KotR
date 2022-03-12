using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class UpdateLobby : MonoBehaviour
{
   // DatabaseReference _reference;

    string _roomCode;

    // Scene References
    public Button _joinButton;
    public InputField _roomCodeInputField;


    private void Start()
    {
        PersistentLocal.persistentLocal.databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateButton()
    {
        StartCoroutine(Create());
    }

    private IEnumerator Create()
    {
        Debug.Log("Clicked CreateButton");

        //-------------------------------------Create valid room code-----------------------------------------
        bool roomCodeValid = false;
        float attemptTime = Time.time + 11f;
        while (roomCodeValid == false)
        {
            _roomCode = new RandomWordsGenerator().AdjectiveNoun(); // Get a random room code

            // Read from Database
            FirebaseDatabase.DefaultInstance.GetReference(_roomCode).GetValueAsync().ContinueWith(task =>
            {
                // Room doesn't exist, let's create it.
                if (task.IsFaulted)
                {
                    roomCodeValid = true; // found valid room code, leave the while loop
                    Debug.Log("Room doesn't exist");
                }
                // Room already exists, try a new name.
                else if (task.IsCompleted)
                {
                    // Keep trying
                    Debug.Log("Room already exists");
                }
            });

            if (attemptTime < Time.time)
            {
                Debug.Log("Couldn't access the Database");
                roomCodeValid = true;
            }

            //yield return new WaitUntil(() => roomCodeValid);
            yield return new WaitForSeconds(1.5f);
        }

        //-----------------------------Create room on Database -named room code---------------------------------
        //PersistentLocal.persistentLocal.room = new Room();
        // Update PersistentLocal references
        PersistentLocal.persistentLocal.matchLocal = new Match();
        PersistentLocal.persistentLocal.matchLocal.RoundCurrent = 1;

        //bool roomCreated = false;

        //System.Activator.CreateInstance(typeof.GetType("Test"));
        string jsonRoom = JsonUtility.ToJson(_roomCode); // Json Utility can't take Strings, only Objects (classes)
        Debug.Log(jsonRoom);
        string jsonMatch = JsonUtility.ToJson(PersistentLocal.persistentLocal.matchLocal);


        //System.Text.Encoding.UTF8.GetBytes(_roomCode)
        //string key = PersistentLocal.persistentLocal.databaseReference.Child("why").Push().Key;
        //Debug.Log("Key is " + key);

        //Dictionary<string, Object> childUpdates = new Dictionary<string, Object>();
        //childUpdates["/Rooms/" + key] = jsonMatch;

        //string room = "TestRoom";
        //room = "\"" + room + "\""; // Adding Quotes | Have tried it with and without this line
        Debug.Log(_roomCode); //Prints "TestRoom" 
        PersistentLocal.persistentLocal.databaseReference.Child(_roomCode).Child("Match").SetRawJsonValueAsync(jsonMatch);






        //PersistentLocal.persistentLocal.databaseReference.Child(jsonRoom).Child("Match").SetRawJsonValueAsync(jsonMatch).ContinueWith(task =>
        //{
        //    if (task.IsCompleted)
        //    {
        //        Debug.Log("Room Created");

        //        //// Push the match to the Database
        //        //string jsonMatch = JsonUtility.ToJson(PersistentLocal.persistentLocal.matchLocal);
        //        //FirebaseDatabase.DefaultInstance.GetReference(_roomCode).Child("Match").SetRawJsonValueAsync(jsonMatch).ContinueWith(task =>
        //        //{
        //        //    if (task.IsCompleted)
        //        //    {
        //        //        Debug.Log("Successfully created the Room and Match");
        //        //        //TO-DO: Auto-fill and run the "JoinButton" so the host's client can connect/create
        //        //        //JoinButton();
        //        //    }
        //        //    else
        //        //    {
        //        //        Debug.Log("Match creation failed");
        //        //    }
        //        //});
        //        roomCreated = true;
        //    }
        //    else
        //    {
        //        Debug.Log("Room creation failed");
        //        roomCreated = true;
        //    }
        //});

        //yield return new WaitUntil(() => roomCreated);
        yield return new WaitForSeconds(1f);

        // Push the match to the Database

        //PersistentLocal.persistentLocal.databaseReference.Child(_roomCode).Child("Match").SetRawJsonValueAsync(jsonMatch).ContinueWith(task =>
        //{
        //    if (task.IsCompleted)
        //    {
        //        Debug.Log("Successfully created the Room and Match");
        //        //TO-DO: Auto-fill and run the "JoinButton" so the host's client can connect/create
        //        //JoinButton();
        //    }
        //    else
        //    {
        //        Debug.Log("Match creation failed");
        //    }
        //});

        //// Save to Database
        //string matchJson = JsonUtility.ToJson(PersistentLocal.persistentLocal.matchLocal);
        //Debug.Log(matchJson);
        //PersistentLocal.persistentLocal.databaseReference.Child("Match").SetRawJsonValueAsync(matchJson).ContinueWith(task =>
        //{
        //    if (task.IsCompleted)
        //    {
        //        Debug.Log("Successfully added data to Firebase.");
        //    }
        //    else
        //    {
        //        Debug.Log("Adding data failed.");
        //    }
        //});

        //string userJson = "{}";
        //PersistentLocal.persistentLocal.databaseReference.Child("User").SetRawJsonValueAsync(userJson).ContinueWith(task =>
        //{
        //    if (task.IsCompleted)
        //    {
        //        Debug.Log("Successfully added data to Firebase.");
        //    }
        //    else
        //    {
        //        Debug.Log("Adding data failed.");
        //    }
        //});
    }

    public void JoinButton()
    {
        if (_roomCodeInputField.text == "")
        {
            //TO-DO: Add pop-up message
            Debug.Log("Please input a Room Code before pressing Join");
            return;
        }


    }
}
