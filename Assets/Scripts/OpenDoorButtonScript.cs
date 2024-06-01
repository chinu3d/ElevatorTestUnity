using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ButtonPressed()
    {
        Dictionary<string, System.Object> payloadForNotification = new Dictionary<string, System.Object>();
        payloadForNotification["name"] = "OpenDoorButtonPressed";

        NotificationManager.PostNotification(payloadForNotification);

    }
}
