using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCursor : MonoBehaviour
{
    public GameObject _cursorChildObject;
    public GameObject _objectToPlace;
    public ARRaycastManager _raycastManager;

    public bool _useCursor = true;


    void Start()
    {
        _cursorChildObject.SetActive(_useCursor);
    }


    void Update()
    {
        if (_useCursor)
        {
            CursorUpdate();
        }

        // The user is touching the screen && it just began
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (_useCursor)
            {
                GameObject.Instantiate(_objectToPlace, transform.position, transform.rotation);
            }
            // Automatic cursor updates are turned off, we must collect raycast data.
            else
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                _raycastManager.Raycast(Input.GetTouch(0).position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
                if (hits.Count > 0)
                {
                    GameObject.Instantiate(_objectToPlace, hits[0].pose.position, hits[0].pose.rotation);
                }
            }
        }
    }

    private void CursorUpdate()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        _raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if (hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
        }
    }
}
