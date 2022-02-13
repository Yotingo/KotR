using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageHost : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*
    public void StageUpdate()
    {
        //// Read from Database
        //_reference.Child("Match").GetValueAsync().ContinueWith(task =>
        //{
        //    if (task.IsCompleted)
        //    {
        //        DataSnapshot snapshot = task.Result;
        //        //Debug.Log(snapshot.Child("ActionAscend").Value.ToString());
        //        Match match = JsonUtility.FromJson<Match>(snapshot.GetRawJsonValue());
        //        _matchLocal.SecretNamesPicked = match.SecretNamesPicked; // Overwrite local Match class
        //        //Debug.Log(_matchLocal.ActionAscend);
        //        // enable list viewing if we got voted and aren't sent
        //    }
        //});

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
                    _roundChangeTime = Time.time + 3f;
                    gameOverPanel.SetActive(false);
                    _matchLocal.RoundCurrent = 2; //Move to Voting Round
                    _roundCurrent = Rounds.Voting;
                    break;

                case Rounds.Voting:
                    // total the votes
                    Dictionary<string, int> voteCountByUsername = new Dictionary<string, int>();
                    foreach (User user in _userList)
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

                        if (!highestPair.Equals(default(KeyValuePair<string, int>)))
                        {
                            if (highestPair.Value > entry.Value)
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
                    if (ascendWon == true) gameOverPanel.gameObject.transform.GetChild(2).GetComponent<Text>().text = "Ascend Team won!";
                    else gameOverPanel.gameObject.transform.GetChild(2).GetComponent<Text>().text = "Descend Team won!";
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
    */
}
