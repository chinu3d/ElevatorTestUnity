using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OnLiftButtonState
{
    Fulfilled,
    Unfulfilled
}

public class OnLiftButtonScript : MonoBehaviour, INotificationObserver
{
    public int floorNumber;
    public OnLiftButtonState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = OnLiftButtonState.Fulfilled;
        NotificationManager.AddObserver(this);
    }

    public void ButtonClicked()
    {
        Dictionary<string, System.Object> payloadForNotification = new Dictionary<string, System.Object>();
        payloadForNotification["name"] = "OnLiftButtonPressed";
        payloadForNotification["onLiftButtonScript"] = this;

        NotificationManager.PostNotification(payloadForNotification);
    }

    public void BroadcastTriggered(Dictionary<string, object> userInfo)
    {
        if ((string)userInfo["name"] == "OnLiftReached")
        {
            if ((int)userInfo["floorReached"] == floorNumber)
            {
                currentState = OnLiftButtonState.Fulfilled;
                GetComponent<Image>().color = Color.white;
            }
        }
    }
}
