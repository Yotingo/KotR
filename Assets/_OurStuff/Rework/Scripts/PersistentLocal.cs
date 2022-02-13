using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class PersistentLocal : MonoBehaviour
{
    public static PersistentLocal persistentLocal;

    public Room room;
    public DatabaseReference databaseReference;
    public int stage;
    public Match matchLocal;

    private void Awake()
    {
        if (persistentLocal != null)
        {
            GameObject.Destroy(persistentLocal);
        }
        else
        {
            persistentLocal = this;
        }

        DontDestroyOnLoad(this);
    }
}
