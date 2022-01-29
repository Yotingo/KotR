using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match
{
    public float RoundTimer; // Used to calculate round countdown timer
    public int RoundCurrent;
    public bool IsGameOver;
    public string ListHolderUserName;

    public List<string> SecretNamesPicked = new List<string>();
    public List<string> AvatarsPicked = new List<string>();
    public string ActionAscend;
    public string ActionDescend;
}
