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
    public InputField _roomCodeInputField;
    public SceneChange _sceneChange;


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
                // Error fetching room
                if (task.IsFaulted)
                {
                    Debug.LogError("Error reading room database! _roomCode:"+_roomCode);
                    return;
                }
                // Room fetch complete
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if( snapshot.Value == null )
                    {
                        roomCodeValid = true; // found valid room code, leave the while loop
                        Debug.Log("Room doesn't exist, using this code to make a new room. _roomCode:"+_roomCode);
                    }
                    else
                    {
                        // Room already exists, try a new name.
                        Debug.Log("Room already exists, trying a new room. _roomCode:"+_roomCode);
                    }
                }
            });

            if (attemptTime < Time.time)
            {
                Debug.LogError("Couldn't access the Database");
                yield break;
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
        string jsonMatch = JsonUtility.ToJson(PersistentLocal.persistentLocal.matchLocal);
        Debug.Log(jsonMatch);

        //System.Text.Encoding.UTF8.GetBytes(_roomCode)
        //string key = PersistentLocal.persistentLocal.databaseReference.Child("why").Push().Key;
        //Debug.Log("Key is " + key);

        //Dictionary<string, Object> childUpdates = new Dictionary<string, Object>();
        //childUpdates["/Rooms/" + key] = jsonMatch;

        //string room = "TestRoom";
        //room = "\"" + room + "\""; // Adding Quotes | Have tried it with and without this line
        Debug.Log(_roomCode); //Prints "TestRoom" 
        PersistentLocal.persistentLocal.databaseReference.Child(_roomCode).Child("Match").SetRawJsonValueAsync(jsonMatch).ContinueWith(task =>
        {
            // Room creation failed
            if( task.IsFaulted )
            {
                Debug.LogError("Error creating room! _roomCode:"+_roomCode);
                return;
            }
            // Room creation task completed
            else if( task.IsCompleted )
            {
                // Room was created successfully
                Debug.Log("Successfully created the Room and Match");
                // join host to room
                JoinLobby(_roomCode);
            }
        });


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
        //yield return new WaitForSeconds(1f);

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

        JoinLobby(_roomCodeInputField.text);
    }

    public void JoinLobby(string roomCode)
    {
        // TODO: make a Main scene that handles loading and unloading all the other scenes (LevelMaster style)
        Debug.Log("Joining Lobby for code:"+roomCode);
        _sceneChange.GoToScene("Lobby");

        // get current room
        // add self to userlist
        // save room/match/lobby

        // TODO Mitch: listen for room changes
    }
}
