using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match
{
    public float RoundTimer = 0f; // Used to calculate round countdown timer
    public int RoundCurrent = 0;
    public bool IsGameOver = false;
    public string ListHolderUserName = "";
    public string LastSentUserName = "";

    public List<User> UserList = new List<User>();
    public List<string> SecretNamesPicked = new List<string>();
    public List<string> AvatarsPicked = new List<string>();
    public string ActionAscend = "";
    public string ActionDescend = "";
}
