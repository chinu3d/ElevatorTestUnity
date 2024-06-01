using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OnFloorButtonState
{
    Fulfilled,
    Unfulfilled
}

public class OnFloorButtonScript : MonoBehaviour, INotificationObserver
{
    public int floorNumber;
    public OnFloorButtonState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = OnFloorButtonState.Fulfilled;
        NotificationManager.AddObserver(this);
    }

    public void ButtonClicked()
    {
        Dictionary<string, System.Object> payloadForNotification = new Dictionary<string, System.Object>();
        payloadForNotification["name"] = "OnFloorButtonPressed";
        payloadForNotification["onFloorButtonScript"] = this;

        NotificationManager.PostNotification(payloadForNotification);
    }

    public void BroadcastTriggered(Dictionary<string, object> userInfo)
    {
        if ((string)userInfo["name"] == "OnLiftReached")
        {
            if ((int)userInfo["floorReached"] == floorNumber)
            {
                currentState = OnFloorButtonState.Fulfilled;
                GetComponent<Image>().color = Color.white;
            }
        }
    }
}
