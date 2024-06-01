using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    //This is a Singleton class
    public static NotificationManager Instance { get; private set; }

    private static List<INotificationObserver> AllObservers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;

            if (AllObservers == null)
            {
                AllObservers = new List<INotificationObserver>();
            }
        }
    }

    public static void AddObserver(INotificationObserver observerObject)
    {
        AllObservers.Add(observerObject);
    }

    public static void RemoveObserver(INotificationObserver observerObject)
    {
        AllObservers.Remove(observerObject);
    }

    public static void PostNotification(Dictionary<string, System.Object> userInfo)
    {
        foreach (INotificationObserver anObserver in AllObservers)
        {
            anObserver.BroadcastTriggered(userInfo);
        }
    }
}

public interface INotificationObserver
{
    public void BroadcastTriggered(Dictionary<string, System.Object> userInfo);
}
