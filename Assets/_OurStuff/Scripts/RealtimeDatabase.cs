using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RealtimeDatabase : MonoBehaviour
{
    DatabaseReference _reference;
    [SerializeField] InputField _username;
    //[SerializeField] InputField _usernameToRead;
    [SerializeField] Text _avatarNumber;

    [SerializeField] MatchHandler _matchHandler;
    [SerializeField] GameObject _directionalLight;

    //private bool _mainMenuScene = true;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        _reference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    //void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    //{
    //    //Debug.Log("Level Loaded");
    //    //Debug.Log(scene.name);
    //    //Debug.Log(mode);

    //    if (scene.name == "MainGame") _mainMenuScene = false;
    //}


    void Update()
    {

    }

    /*
    // Save User data to Firebase
    public void UserDataSave()
    {
        User user = new User(); //Create new User class
        _matchHandler._userLocal = user;
        user.UserName = _username.text; //Save UserName inside User class as the input field text

        //Add to database
        string json = JsonUtility.ToJson(user); //Comvert class to json file
        _reference.Child("User").Child(user.UserName).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log("Successfully added data to Firebase.");
            }
            else
            {
                Debug.Log("Adding data failed.");
            }
        });
    }
    */

    /*
    // Load User data from Firebase
    public void UserDataRead()
    {
        _reference.Child("User").Child(_usernameToRead.text).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Successfully retreived data from Firebase.");
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot.Child("UserName").Value.ToString());
            }
        });
    }
    */

    // Join Game button
    public void JoinGame()
    {
        if (_username.text == "Yotingo")
        {
            //MatchCreate();
            //JoinGameProcess();

            if (_matchHandler._matchLocal == null)
            {
                Debug.Log("Don't forget to start a match!");
            }
            else
            {
                JoinGameProcess();
                _matchHandler.MatchStart(true);
                //_matchHandler._matchStarted = true;
            }

            //TO-DO: React to input and toggle button
        }
        else
        {
            if (_username.text == "")
            {
                Debug.Log("Please enter a name");
            }
            else
            {
                _matchHandler.PrepareForMatch(); // Setup local match files
                JoinGameProcess();
                _matchHandler.MatchStart(false);

                //if (_matchHandler._matchLocal == null)
                //{
                //    Debug.Log("Please wait for the host to create the match.");
                //}
                //else
                //{
                //    JoinGameProcess();
                //    _matchHandler.MatchStart(false);
                //}

                //TO-DO: React to input and toggle button
            }
        }
    }

    // Logic for joining
    private void JoinGameProcess ()
    {
        //-------------------------------Create User-------------------------
        User user = new User(); //Create new User class
        user.UserName = _username.text; //Save UserName inside User class as the input field text
        _matchHandler._userLocal = user;

        // Update the match info locally
        _matchHandler.MatchUpdateClient();


        //--------------------------------Avatar----------------------------
        if (AvatarSelect() == true)
        {

            //--------------------------Secret Name-------------------------
            string selectedName = "UnAssigned";
            bool isNamePicked = false;
            while (isNamePicked == false)
            {
                int randomNumber = Random.Range(0, _matchHandler._secretnamesReference.Count - 1);
                selectedName = _matchHandler._secretnamesReference[randomNumber];
                if (_matchHandler._matchLocal.SecretNamesPicked == null)
                {
                    isNamePicked = true;
                }
                else
                {
                    if (_matchHandler._matchLocal.SecretNamesPicked.Contains(selectedName))
                    {
                        // The name has been picked by another player. Do nothing and repeat the while loop.
                    }
                    else
                    {
                        isNamePicked = true;
                    }
                }
            }
            user.SecretName = selectedName; //Assign name to local User class
            _matchHandler._matchLocal.SecretNamesPicked.Add(selectedName); //Add picked name to local Match class

            //------------------------Team & Action-------------------------
            // Even player : something divided by two without remainder is even
            if ((_matchHandler._matchLocal.SecretNamesPicked.Count - 1) % 2 == 0)
            {
                user.Team = "Ascend Team";
                user.SecretAction = _matchHandler._matchLocal.ActionAscend;
            }
            // Odd
            else
            {
                user.Team = "Descend Team";
                user.SecretAction = _matchHandler._matchLocal.ActionDescend;
            }

            //-----------------List & Reincarnated & Vote--------------------
            user.sentUp = false;
            user.sentDown = false;
            user.CurrentVote = "";


            //-------------------Add User to database--------------------
            string json = JsonUtility.ToJson(user); //Comvert class to json file
            _reference.Child("User").Child(user.UserName).SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Successfully added data to Firebase.");
                }
                else
                {
                    Debug.Log("Adding data failed.");
                }
            });


            //-------------------Update Match info-----------------------
            string jsonMatch = JsonUtility.ToJson(_matchHandler._matchLocal);
            Debug.Log(json);
            _reference.Child("Match").SetRawJsonValueAsync(jsonMatch).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Successfully added data to Firebase.");
                }
                else
                {
                    Debug.Log("Adding data failed.");
                }
            });

            SceneManager.LoadScene("MainGame");
        }
        else
        {
            //TO-DO: Pop-up notification (avatar is taken)
        }
    }

    // Create new match button
    public void MatchCreate ()
    {
        if (_username.text == "Yotingo")
        {
            _matchHandler.ResetMatch(); // Setup and reset database match files
            _matchHandler._matchStartedHost = true;
        }
        //else
        //{
        //    _matchHandler.PrepareForMatch(); // Setup local match files
        //}
    }

    public void AvatarCycle(bool directionForward)
    {
        //-------------------------------Disable Current Avatar--------------------------
        // Find 'Avatar Number' and set reference
        if (_avatarNumber == null)
        {
            _avatarNumber = GameObject.Find("Avatar Number").GetComponent<Text>();
            _avatarNumber.text = 0.ToString();
        }

        // Parse the 'avatarNumberCur' from the '_avatarNumber' text gameOject
        int avatarNumberCur = int.Parse(_avatarNumber.text);
        _matchHandler._avatarList[avatarNumberCur].SetActive(false);


        //---------------------------------On Menu Screen-----------------------------
        if (_matchHandler._roundCurrent == 0) 
        {
            bool nextAvailableAvatarFound = false;
            while (nextAvailableAvatarFound == false)
            {
                if (directionForward == true)
                {
                    avatarNumberCur += 1;

                    if (avatarNumberCur > _matchHandler._avatarList.Count - 1)
                    {
                        avatarNumberCur = 0;
                    }
                }
                else
                {
                    avatarNumberCur -= 1;

                    if (avatarNumberCur < 0)
                    {
                        avatarNumberCur = _matchHandler._avatarList.Count - 1;
                    }
                }

                Debug.Log(_matchHandler._matchLocal.AvatarsPicked);
                Debug.Log(_matchHandler._matchLocal.AvatarsPicked.Count);

                // Null check
                if (_matchHandler._matchLocal.AvatarsPicked.Count <= 0)
                {
                    break;
                }

                // This avatar is available
                if (_matchHandler._matchLocal.AvatarsPicked.Contains(_matchHandler._avatarList[avatarNumberCur].name))
                {
                    //nextAvailableAvatarFound = false; // Continue
                    // Don't do nothin'
                    Debug.Log("Next button on menu screen");
                }
                else
                {
                    nextAvailableAvatarFound = true; // break
                }
            }
        }
        //------------------------------On gameplay screen---------------------------
        else
        {
            bool nextAvailableAvatarFound = false;
            while (nextAvailableAvatarFound == false)
            {
                if (directionForward == true)
                {
                    avatarNumberCur += 1;

                    if (avatarNumberCur > _matchHandler._avatarList.Count - 1)
                    {
                        avatarNumberCur = 0;
                    }
                }
                else
                {
                    avatarNumberCur -= 1;

                    if (avatarNumberCur < 0)
                    {
                        avatarNumberCur = _matchHandler._avatarList.Count - 1;
                    }
                }

                Debug.Log(_matchHandler._matchLocal.AvatarsPicked);
                Debug.Log(_matchHandler._matchLocal.AvatarsPicked.Count);

                // Null check
                if (_matchHandler._matchLocal.AvatarsPicked.Count <= 0)
                {
                    break;
                }

                // This avatar has been sent -- don't appear on vote list
                bool hasBeenSent = false;
                string avatarNameCur = _matchHandler._avatarList[avatarNumberCur].name;
                foreach (User user in _matchHandler._userList)
                {
                    if (user.Avatar == avatarNameCur)
                    {
                        if (user.sentUp == true) hasBeenSent = true;
                        if (user.sentDown == true) hasBeenSent = true;

                        _matchHandler.playerNameLabel.GetComponent<Text>().text = user.UserName;
                        _matchHandler._voteButton.GetComponentInChildren<Text>().text = ("Vote for " + user.UserName); 
                        Debug.Log("User's name is " + user.UserName);

                        break;
                    }
                }

                // Grey out avatars that have been sent
                if (hasBeenSent == false && _matchHandler._userLocal.Avatar != avatarNameCur)
                {
                    GameObject.Find("Directional Light").GetComponent<Light>().intensity = 1.88f;
                    _matchHandler._voteButton.SetActive(true);
                    Debug.Log("Light On");
                }
                else
                {
                    GameObject.Find("Directional Light").GetComponent<Light>().intensity = 0.25f;
                    _matchHandler._voteButton.SetActive(false);
                    Debug.Log("Light Off");
                }

                // This avatar belongs to a player
                if (_matchHandler._matchLocal.AvatarsPicked.Contains(avatarNameCur)) //&& !hasBeenSent)
                {
                    nextAvailableAvatarFound = true; // break
                }
                else
                {
                    Debug.Log("Next button on menu screen");
                }
            }

            //Debug.Log("Avatar Next");

            //string avatarNameCur = _matchHandler._avatarList[avatarNumberCur].name;
            //bool hasBeenSent = false;

            //foreach (User user in _matchHandler._userList)
            //{
            //    Debug.Log(user.UserName);

            //    if (user.Avatar == avatarNameCur)
            //    {
            //        if (user.sentUp == true) hasBeenSent = true;
            //        if (user.sentDown == true) hasBeenSent = true;

            //        _matchHandler.playerNameLabel.GetComponent<Text>().text = user.UserName; // Not working
            //        Debug.Log("User's name is " + user.UserName);
            //    }
            //}

            //if (_matchHandler._matchLocal.AvatarsPicked.Contains(avatarNameCur) == true || hasBeenSent == false || _matchHandler._userLocal.Avatar != avatarNameCur)
            //{
            //    GameObject.Find("Directional Light").GetComponent<Light>().intensity = 1.88f;
            //    _matchHandler._voteButton.SetActive(true);
            //}
            //else
            //{
            //    GameObject.Find("Directional Light").GetComponent<Light>().intensity = 0.25f;
            //    _matchHandler._voteButton.SetActive(false);
            //}
        }

        // Enable selected avatar
        _avatarNumber.text = avatarNumberCur.ToString();
        _matchHandler._avatarList[avatarNumberCur].SetActive(true);
    }
    
    //public void AvatarLast ()
    //{
    //    if (_avatarNumber == null)
    //    {
    //        _avatarNumber = GameObject.Find("Avatar Number").GetComponent<Text>();
    //        _avatarNumber.text = 0.ToString();
    //    }

    //    int avatarNumberCur = int.Parse(_avatarNumber.text);
    //    _matchHandler._avatarList[avatarNumberCur].SetActive(false);
    //    avatarNumberCur -= 1;

    //    if (avatarNumberCur < 0)
    //    {
    //        avatarNumberCur = _matchHandler._avatarList.Count - 1;
    //    }

    //    _avatarNumber.text = avatarNumberCur.ToString();
    //    _matchHandler._avatarList[avatarNumberCur].SetActive(true);

    //    if (_matchHandler._matchStartedHost == true) // On gameplay screen. Do voting.
    //    {
    //        string avatarNameCur = _matchHandler._avatarList[avatarNumberCur].name;
    //        bool hasBeenSent = false;
    //        foreach (User user in _matchHandler._userList)
    //        {
    //            if (user.Avatar == avatarNameCur)
    //            {
    //                if (user.sentUp == true) hasBeenSent = true;
    //                if (user.sentDown == true) hasBeenSent = true;

    //                _matchHandler.playerNameLabel.GetComponent<Text>().text = user.UserName;
    //            }
    //        }

    //        if (_matchHandler._matchLocal.AvatarsPicked.Contains(avatarNameCur) == true || hasBeenSent == false || _matchHandler._userLocal.Avatar != avatarNameCur)
    //        {
    //            GameObject.Find("Directional Light").GetComponent<Light>().intensity = 1.88f;
    //            _matchHandler._voteButton.SetActive(true);
    //        }
    //        else
    //        {
    //            GameObject.Find("Directional Light").GetComponent<Light>().intensity = 0.25f;
    //            _matchHandler._voteButton.SetActive(false);
    //        }    
    //    }
    //}

    // Called when join match is selected
    public bool AvatarSelect ()
    {
        if (_avatarNumber == null)
        {
            _avatarNumber = GameObject.Find("Avatar Number").GetComponent<Text>();
            _avatarNumber.text = 0.ToString();
        }

        int avatarNumberCur = int.Parse(_avatarNumber.text);
        if (_matchHandler._matchLocal.AvatarsPicked == null)
        {
            _matchHandler._matchLocal.AvatarsPicked[avatarNumberCur] = _matchHandler._avatarList[avatarNumberCur].name; // Add the current avatar to the pick list
            _matchHandler._userLocal.Avatar = _matchHandler._avatarList[avatarNumberCur].name; // Save selected avatar to user
            return true;
        }
        else
        {
            if (_matchHandler._matchLocal.AvatarsPicked.Contains(_matchHandler._avatarList[avatarNumberCur].name))
            {
                // The name has been picked by another player. 
                Debug.Log("This avatar is taken!");
                return false;
            }
            else
            {
                _matchHandler._matchLocal.AvatarsPicked.Add(_matchHandler._avatarList[avatarNumberCur].name); // Pick avatar
                _matchHandler._userLocal.Avatar = _matchHandler._avatarList[avatarNumberCur].name; // Save selected avatar to user
                return true;
            }
        }
    }

    public void AvatarVote ()
    {
        if (_avatarNumber == null)
        {
            _avatarNumber = GameObject.Find("Avatar Number").GetComponent<Text>();
            _avatarNumber.text = 0.ToString();
        }

        int avatarNumberCur = int.Parse(_avatarNumber.text);
        string avatarNameCur = _matchHandler._avatarList[avatarNumberCur].name;

        foreach( User user in _matchHandler._userList )
        {
            if( user.Avatar == avatarNameCur )
            {
                _matchHandler._userLocal.CurrentVote = user.UserName;
                break;
            }
        }

        //-------------------Add User to database--------------------
        string json = JsonUtility.ToJson(_matchHandler._userLocal); //Comvert class to json file
        _reference.Child("User").Child(_matchHandler._userLocal.UserName).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Successfully added vote to Firebase.");
            }
            else
            {
                Debug.Log("Adding vote failed.");
            }
        });

        Debug.Log("I voted!");

        GameObject.Find("Directional Light").GetComponent<Light>().intensity = 0.25f;
    }

    public void AvatarSend ()
    {
        _matchHandler.AvatarSendCheck();

        //if (sentUp == true)
        //{
        //    user.sentUp = true;
        //}
        //else
        //{
        //    user.sentDown = true;
        //}
    }
}
