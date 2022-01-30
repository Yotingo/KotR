using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarHider : MonoBehaviour
{
    RealtimeDatabase realtimeDatabase;

    private void OnEnable()
    {
        if (realtimeDatabase == null) realtimeDatabase = GameObject.FindObjectOfType<RealtimeDatabase>();

        realtimeDatabase.AvatarHide(false);
    }

    private void OnDisable()
    {
        if (realtimeDatabase == null) realtimeDatabase = GameObject.FindObjectOfType<RealtimeDatabase>();

        realtimeDatabase.AvatarHide(true);
    }
}
