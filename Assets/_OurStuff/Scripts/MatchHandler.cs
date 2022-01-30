using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity;
using Firebase.Database;
using UnityEngine.UI;

public class MatchHandler : MonoBehaviour
{
    // Match State
    public bool _matchStartedClient = true;
    public bool _matchStartedHost = false;
    //private bool _matchInitialized = false;
    public float _roundChangeTime; // When the change will happen

    public enum Rounds {Lobby, Initializing, Voting, Waiting, GameOver}
    public Rounds _roundCurrent = Rounds.Lobby;
    Rounds _roundLast = Rounds.Lobby;

    private float _upateLast;

    private bool initDone = false;

    // References
    DatabaseReference _reference;
    RealtimeDatabase _realtimeDatabase;
    [SerializeField] GameObject _characterListGO;
    [SerializeField] GameObject _countDownTimer;
    [SerializeField] GameObject _roundTitle;
    [HideInInspector] public User _userLocal = new User();
    [HideInInspector] public Match _matchLocal = new Match();
    public List<string> _secretnamesReference = new List<string>();
    public List<GameObject> _avatarList = new List<GameObject>();
    public List<string> _secretActionReference = new List<string>();

    [HideInInspector] public GameObject playerNameLabel;

    [HideInInspector] public List<User> _userList = new List<User>();
    [HideInInspector] public GameObject _voteButton;
    [HideInInspector] public GameObject _viewListButton;

    [HideInInspector] public GameObject _gameOverScreen;
    [HideInInspector] public GameObject _gameOverWinnerText;

    [HideInInspector] public GameObject gameOverPanel;
    string gameOverText;

    [HideInInspector] public GameObject playerSentPopup;
    [HideInInspector] public GameObject playerSentText;

    [HideInInspector] public GameObject newPlayerHasListPopup;

    [HideInInspector] public GameObject secretNameListPanel;
    [HideInInspector] public GameObject secretInfoPanel;
    [HideInInspector] public GameObject secretNameListText;

    void Start()
    {
        _reference = FirebaseDatabase.DefaultInstance.RootReference;
        _realtimeDatabase = gameObject.GetComponent<RealtimeDatabase>();
        _countDownTimer = GameObject.Find("Round Timer");
        _roundTitle = GameObject.Find("Round Title");
        _characterListGO = GameObject.Find("Characters");
        DontDestroyOnLoad(_characterListGO);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _upateLast + 0.2f)
        {
            UpdateSlow(); // Use less data
        }
    }

    private void UpdateSlow()
    {
        // In "Lobby" - waiting for players to ready up
        //if (_matchInitialized == false && _matchStarted == false)
        //{
        //    ReadyCheck();
        //}

        //if (_matchInitialized == false && _matchStarted == true)
        //{
        //    _roundCurrent = Rounds.Initializing;
        //    //TO-DO: Get round change time from database
        //}


        if (_matchStartedHost == true)
        {
            MatchUpdateHost();
            if (_countDownTimer != null) _countDownTimer.GetComponent<Text>().text = (_matchLocal.RoundTimer).ToString();
        }
        if (_matchStartedClient == true)
        {
            MatchUpdateClient();
            if (_countDownTimer != null) _countDownTimer.GetComponent<Text>().text = (_matchLocal.RoundTimer).ToString();
        }
    }

    private void ReadyCheck()
    {

    }

    public void MatchStart(bool host)
    {
        Debug.Log(System.Environment.StackTrace);
        //if (_matchLocal.RoundCurrent != 0)
        //{
        //    return;
        //}

        Debug.Log("Voting Round has started");
        _roundLast = Rounds.GameOver;
        StartCoroutine(LateInit());

        if (host)
        {
            _roundCurrent = Rounds.Voting;
            _matchLocal.RoundCurrent = 1;
            _roundChangeTime = Time.time + 10f;
            _matchStartedHost = true;
            _matchStartedClient = false;
        }
        else
        {
            //_roundCurrent = Rounds.Initializing;
            //_matchLocal.RoundCurrent = 1;
            _matchStartedHost = false;
            _matchStartedClient = true;
        }
    }

    private IEnumerator LateInit () 
    {
        yield return new WaitForSeconds(0.2f);
        _characterListGO.transform.position = GameObject.Find("Character Anchor").transform.position; //Move characters to center of screen
        GameObject.Find("Left Button").GetComponent<Button>().onClick.AddListener(delegate { GetComponent<RealtimeDatabase>().AvatarCycle(false); });
        GameObject.Find("Right Button").GetComponent<Button>().onClick.AddListener(delegate { GetComponent<RealtimeDatabase>().AvatarCycle(true); });
        GameObject.Find("Vote Button").GetComponent<Button>().onClick.AddListener(delegate { GetComponent<RealtimeDatabase>().AvatarVote(); });
        GameObject.Find("Send Button").GetComponent<Button>().onClick.AddListener(delegate { GetComponent<RealtimeDatabase>().AvatarSend(); });

        yield return new WaitForSeconds(2.3f);
        _countDownTimer = GameObject.Find("Round Timer");
        _roundTitle = GameObject.Find("Round Title");
        _voteButton = GameObject.Find("Vote Button");
        _viewListButton = GameObject.Find("Scroll List True Names Button");
        _viewListButton.SetActive(false);

        playerNameLabel = GameObject.Find("PlayerNameLabel");

        //_gameOverScreen = GameObject.Find("Game Over Up Panel");
        //_gameOverWinnerText = GameObject.Find("Winner Text");
        //_gameOverScreen.SetActive(false);

        playerSentPopup = GameObject.Find("Player Sent Announcement Panel");
        playerSentText = GameObject.Find("Player Sent Announcement Text");
        playerSentPopup.SetActive(false);

        newPlayerHasListPopup = GameObject.Find("New List Owner Popup");
        newPlayerHasListPopup.SetActive(false);

        secretNameListPanel = GameObject.Find("Scroll List True Names Panel");
        secretNameListText = GameObject.Find("Scroll List True Names Text");
        secretNameListPanel.SetActive(false);

        secretInfoPanel = GameObject.Find("Secret Info Panel");
        secretInfoPanel.gameObject.transform.GetChild(0).GetComponent<Text>().text = _userLocal.Team;
        secretInfoPanel.gameObject.transform.GetChild(1).GetComponent<Text>().text = _userLocal.SecretAction;

        gameOverPanel = GameObject.Find("Game Over Panel");
        gameOverPanel.SetActive(false);

        GameObject clickBlocker = GameObject.Find("Click Blocker");
        clickBlocker.SetActive(false);

        //this will probably need to be called after all users have joined
        string listText = "";
       foreach(User user in _userList)
       {
            listText += user.UserName + " - " + user.SecretName + "\n";
       }
        secretNameListText.GetComponent<Text>().text = listText;

        _realtimeDatabase.AvatarCycle(true);

        initDone = true;
    }

    private void MatchUpdateHost()
    {
        UsersLocalUpdate(); // Update the local user info from Database

        //_matchLocal.RoundTimer = _roundChangeTime;
        //Debug.Log(_matchLocal.RoundTimer);
        Debug.Log(_roundCurrent);
        if (Time.time > _roundChangeTime)
        {
            switch (_roundCurrent)
            {
                case Rounds.Initializing:
                    // The initializing round is used in place of a lobby. We can't jump staight into a voting round without all the players being connected.
                    Debug.Log("Voting Round has started");
                    _roundTitle.GetComponent<Text>().text = "Voting starts in:";
                    _roundChangeTime = Time.time + 10f;
                    _matchLocal.RoundCurrent = 2; //Move to Voting Round
                    _roundCurrent = Rounds.Voting;
                    break;

                case Rounds.Voting:           
                    // total the votes
                    Dictionary<string, int> voteCountByUsername = new Dictionary<string, int>();
                    foreach(User user in _userList)
                    {
                        int newTotal = 0;
                        voteCountByUsername.TryGetValue(user.CurrentVote, out newTotal);
                        newTotal++;
                        voteCountByUsername[user.CurrentVote] = newTotal;
                    }
                    // get winner
                    KeyValuePair<string, int> highestPair;
                    foreach (KeyValuePair<string, int> entry in voteCountByUsername)
                    {
                        if (entry.Key == "")
                            continue;

                        if(!highestPair.Equals(default(KeyValuePair<string, int>)))
                        {
                            if( highestPair.Value > entry.Value )
                            {
                                highestPair = entry;
                            }
                        }
                        else
                        {
                            highestPair = entry;
                        }
                    }

                    
                    string winningUsername;
                    if (highestPair.Equals(default(KeyValuePair<string, int>))) // if the highestPair equals zero (no votes were made)
                    {
                        int winner = Random.Range(0, _userList.Count - 1);
                        
                        // Make sure the winner isn't already sent
                        while (_userList[winner].sentUp || _userList[winner].sentDown)
                        {
                            winner += 1;

                            if (winner >= _userList.Count)
                            {
                                winner = 0;
                            }
                        }
                        
                        winningUsername = _userList[winner].UserName;
                    }
                    else
                    {
                        winningUsername = highestPair.Key;
                    }

                    Debug.Log("The new list holder is " + winningUsername);
                    _matchLocal.ListHolderUserName = winningUsername;
                    newPlayerHasListPopup.SetActive(true);

                    // Activate ListDisplayButton for the Host
                    Debug.Log("ListHolderName =  " + _matchLocal.ListHolderUserName);
                    Debug.Log("UserName =  " + _userLocal.UserName);
                    if (_matchLocal.ListHolderUserName == _userLocal.UserName && !_userLocal.sentUp && !_userLocal.sentDown)
                    {
                        _viewListButton.SetActive(true);
                    }
                    else
                    {
                        _viewListButton.SetActive(false);
                    }

                    Debug.Log("Waiting Round has started");
                    _roundTitle.GetComponent<Text>().text = "Voting starts in:";
                    _roundChangeTime = Time.time + 10f;
                    _matchLocal.RoundCurrent = 3; //Move to Waiting Round
                    _roundCurrent = Rounds.Waiting;
                    break;

                case Rounds.Waiting:
                    Debug.Log("Voting Round has started");
                    _roundTitle.GetComponent<Text>().text = "Voting ends in:";
                    //TO-DO: Check for game over
                    _roundChangeTime = Time.time + 60f;
                    _matchLocal.RoundCurrent = 2; //Move to Voting Round
                    _roundCurrent = Rounds.Voting;
                    break;

                case Rounds.GameOver:
                    Debug.Log("Game Over Round has started");
                    _roundChangeTime = Time.time + 10f;
                    _matchLocal.RoundCurrent = 1; //Move to Initializing Round
                    gameOverPanel.SetActive(true);
                    gameOverPanel.gameObject.transform.GetChild(2).GetComponent<Text>().text = gameOverText;
                    //TO-DO: Return to main menu
                    break;
            }
        }

        _matchLocal.RoundTimer = _roundChangeTime - Time.time;

        // Save to Database
        string json = JsonUtility.ToJson(_matchLocal);
        //Debug.Log(json);

        _reference.Child("Match").SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                //Debug.Log("Successfully added data to Firebase.");
            }
            else
            {
                //Debug.Log("Adding data failed.");
            }
        });
    }

    public void MatchUpdateClient ()
    {
        // Read from Database
        _reference.Child("Match").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                //Debug.Log(snapshot.Child("ActionAscend").Value.ToString());
                Match match = JsonUtility.FromJson<Match>(snapshot.GetRawJsonValue());
                _matchLocal = match; // Overwrite local Match class
                //Debug.Log(_matchLocal.ActionAscend);
                // enable list viewing if we got voted and aren't sent

                Debug.Log("ListHolderName =  " + _matchLocal.ListHolderUserName);
                Debug.Log("UserName =  " + _userLocal.UserName);
            }
        });

        if (initDone == true)
        {
            if (_matchLocal.ListHolderUserName == _userLocal.UserName && !_userLocal.sentUp && !_userLocal.sentDown)
            {
                _viewListButton.SetActive(true);
            }
            else
            {
                _viewListButton.SetActive(false);
            }
        }

        UsersLocalUpdate(); // Update the local user info from Database

        _roundCurrent = (Rounds)_matchLocal.RoundCurrent; // Update the last Round Enum
        _roundChangeTime = _matchLocal.RoundTimer;

        switch (_roundCurrent)
        {
            case Rounds.Lobby:
                if (_roundCurrent != _roundLast) // Just started the round
                {
                    Debug.Log("Lobby Round has started");
                    //_matchLocal.RoundTimer = TimeUtils.GetUnixTime() + 90f;
                    //TO-DO: Update round time on database
                }
                break;

            case Rounds.Initializing:
                //if (TimeUtils.GetUnixTime() > _matchLocal.RoundTimer)
                if (_roundCurrent != _roundLast) // Just started the round
                {
                    Debug.Log("Initializing Round has started");
                    _roundTitle.GetComponent<Text>().text = "Voting starts in:";
                    //_matchLocal.RoundTimer = TimeUtils.GetUnixTime() + 90f;
                }
                break;

            case Rounds.Voting:
                if (_roundCurrent != _roundLast) // Just started the round
                {
                    // check new list owner?

                    Debug.Log("Voting Round has started");
                    _roundTitle.GetComponent<Text>().text = "Voting ends in:";
                }
                break;

            case Rounds.Waiting:
                if (_roundCurrent != _roundLast) // Just started the round
                {
                    Debug.Log("Waiting Round has started");
                    _roundTitle.GetComponent<Text>().text = "Voting starts in:";

                    _userLocal.CurrentVote = "";

                    //-------------------Add User to database--------------------
                    string json = JsonUtility.ToJson(_userLocal); //Comvert class to json file
                    _reference.Child("User").Child(_userLocal.UserName).SetRawJsonValueAsync(json).ContinueWith(task =>
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
                }
                break;

            case Rounds.GameOver:
                if (_roundCurrent != _roundLast) // Just started the round
                {
                    Debug.Log("Game over Round has started");
                    gameOverPanel.SetActive(true);
                    gameOverPanel.gameObject.transform.GetChild(2).GetComponent<Text>().text = gameOverText;
                }
                break;
        }

        _roundLast = (Rounds)_matchLocal.RoundCurrent; // Update the last Round Enum
    }

    public void AvatarSendCheck ()
    {
        if( _userLocal.sentUp || _userLocal.sentDown )
        {
            Debug.Log("Cannot send when you are already sent!");
            return;
        }    

        User userToSend = null;
        string typedName = GameObject.Find("Player True Name InputField").GetComponent<InputField>().text;

        foreach (User user in _userList)
        {
            if (user.SecretName == typedName)
            {
                userToSend = user;
                break;
            }
        }

        if (userToSend == null)
        {
            Debug.Log("Invalid Name");
            //TO-DO: add invalid name message
            return;
        }

        if (_userLocal.Team == "Ascend Team")
        {
            userToSend.sentUp = true;
        }
        else
        {
            userToSend.sentDown = true;
        }

        // Save to Database
        _matchLocal.LastSentUserName = userToSend.UserName;
        string json = JsonUtility.ToJson(_matchLocal);
        //Debug.Log(json);

        _reference.Child("Match").SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                //Debug.Log("Successfully added data to Firebase.");
            }
            else
            {
                //Debug.Log("Adding data failed.");
            }
        });

        //-------------------Add User to database--------------------
        string Userjson = JsonUtility.ToJson(userToSend); //Comvert class to json file
        _reference.Child("User").Child(userToSend.UserName).SetRawJsonValueAsync(Userjson).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Successfully added \"send\" to Firebase.");
                GameOverCheck();
            }
            else
            {
                Debug.Log("Adding data failed.");
            }
        });
    }

    private void UsersLocalUpdate ()
    {
        _reference.Child("User").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                //Debug.Log(snapshot.Child("UserName").Value.ToString

                _userList.Clear();
                //if (_matchStartedHost == true) _matchLocal.AvatarsPicked.Clear();

                int count = 0;
                foreach (var child in snapshot.Children)
                {
                    count++;

                    //string name = child.Child("UserName").Value.ToString();
                    //Debug.Log(name.ToString());

                    User user = JsonUtility.FromJson<User>(child.GetRawJsonValue());
                    //Debug.Log(user.UserName);

                    // Build _userList
                    _userList.Add(user);

                    //// Send Check
                    //foreach (User userCurrent in _userList)
                    //{
                    //    if (userCurrent.UserName == user.UserName)
                    //    {
                    //        if (userCurrent.sentUp != user.sentUp || userCurrent.sentDown != user.sentDown)
                    //        {
                    //            PlayerSentPop(user);
                    //            if (user.UserName == _userLocal.UserName)
                    //            {
                    //                _userLocal.sentDown = user.sentDown;
                    //                _userLocal.sentUp = user.sentUp;
                    //            }
                    //        }
                    //    }
                    //    _userList.Remove(userCurrent);
                    //    _userList.Add(user);
                    //}


                    // Update Match info
                    if (_matchStartedHost == true)
                    {
                        if (!_matchLocal.AvatarsPicked.Contains(user.Avatar))
                        {
                            _matchLocal.AvatarsPicked.Add(user.Avatar);
                        }
                    }
                }
                //Debug.Log("number of users " + count);
            }
        });

        //foreach (User user in _userList)
        //{
        //    Debug.Log(user.UserName);
        //}
    }

    public void PlayerSentPop (User userSent)
    {
        // Activate pop up
        playerSentPopup.SetActive(true);
        playerSentText.GetComponent<Text>().text = userSent.UserName + " was sent "+(userSent.sentUp?"up":"down")+"!";

        gameOverPanel.SetActive(true);
    }

    private void GameOverCheck ()
    {
        int totalSentUp = 0;
        int totalSentDown = 0;
        int target = (_userList.Count / 2);
        foreach (User user in _userList)
        {
            if (user.sentUp == true)
            {
                totalSentUp++;
            }
            if (user.sentDown == true)
            {
                totalSentDown++;
            }

            if (totalSentUp >= target)
            {
                WinSreen(true);
                break;
            }
            if (totalSentDown >= target)
            {
                WinSreen(false);
                break;
            }
        }
    }


    private void WinSreen(bool upWon)
    {
        Debug.Log("Game Over " + upWon);
        //_gameOverScreen.SetActive(true);
        Debug.Log("1");
        //_gameOverWinnerText.GetComponent<Text>().text = upWon ? "Ascend Team won!" : "Descend Team won!";

        //gameOverPanel.SetActive(true);
        //gameOverPanel.gameObject.transform.GetChild(1).GetComponent<Text>().text = upWon ? "Ascend Team won!" : "Descend Team won!";
        gameOverText = upWon ? "Ascend Team won!" : "Descend Team won!";

        Debug.Log("2");
        // Update match to new phase
        _roundCurrent = Rounds.GameOver;
        Debug.Log("3");
        _matchLocal.IsGameOver = true;
        Debug.Log("4");
        _matchLocal.RoundCurrent = (int)Rounds.GameOver; 
        Debug.Log("The current round should be GameOver " + _roundCurrent);

        //_matchLocal.RoundTimer = _roundChangeTime - Time.time;

        // Save to Database
        string json = JsonUtility.ToJson(_matchLocal);
        //Debug.Log(json);

        _reference.Child("Match").SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                //Debug.Log("Successfully added data to Firebase.");
            }
            else
            {
                //Debug.Log("Adding data failed.");
            }
        });
    }

    public void PrepareForMatch () //called by Clients
    {
        MatchUpdateClient();

        //Match match = new Match();
        //_matchLocal = match;
        //_matchLocal.IsGameOver = false;
        //_matchStartedClient = false;
        //_matchStartedHost = false;
        //_matchLocal.RoundCurrent = 0;
        ////_matchInitialized = false;
        //_matchLocal.AvatarsPicked.Clear(); //Clear avatar pick list
        //_matchLocal.SecretNamesPicked.Clear();
        //_matchLocal.SecretNamesPicked.Add("Init");
    }

    public void ResetMatch() //called by Host
    {
        Match match = new Match();
        _matchLocal = match;
        match.IsGameOver = false;
        _matchStartedClient = false;
        _matchStartedHost = false;
        match.RoundCurrent = 0;
        //_matchInitialized = false;
        match.AvatarsPicked.Clear(); //Clear avatar pick list
        match.SecretNamesPicked.Clear();
        match.SecretNamesPicked.Add("Init");

        // Team Actions
        int actionAscendInt = Random.Range(0, _secretActionReference.Count - 1);
        int actionDescendInt = Random.Range(actionAscendInt + 1, _secretActionReference.Count - 2);
        match.ActionAscend = _secretActionReference[actionAscendInt];
        match.ActionDescend = _secretActionReference[actionDescendInt];

        // Save to Database
        string matchJson = JsonUtility.ToJson(match);
        Debug.Log(matchJson);
        _reference.Child("Match").SetRawJsonValueAsync(matchJson).ContinueWith(task =>
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

        string userJson = "{}";
        _reference.Child("User").SetRawJsonValueAsync(userJson).ContinueWith(task =>
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

    }
}
