using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorControllerScript : MonoBehaviour, INotificationObserver
{
    [SerializeField]
    GameObject BottomMostPoint;

    [SerializeField]
    GameObject TopMostPoint;

    [SerializeField]
    GameObject ElevatorObject;

    [SerializeField]
    int TotalNumberOfFloors;

    [SerializeField]
    GameObject FloorIndicatorPrefab;

    [SerializeField]
    GameObject OnLiftFloorButtonsPanel;

    [SerializeField]
    GameObject OnLiftFloorButtonPrefab;

    [SerializeField]
    GameObject OnFloorButtonsPanel;

    [SerializeField]
    GameObject OnFloorButtonPrefab;

    [SerializeField]
    GameObject ElevatorParent;

    [SerializeField]
    GameObject OpenDoorButton;

    [SerializeField]
    Image LiftDirectionIndicatorImage;

    [SerializeField]
    Sprite SpriteForUpIndication;

    [SerializeField]
    Sprite SpriteForDownIndication;

    [SerializeField]
    Sprite SpriteForInactiveIndication;

    [SerializeField]
    TMPro.TMP_Text CurrentFloorText;

    [SerializeField]
    TMPro.TMP_Text DoorIsOpenIndicatorText;


    private int maxFloors = 10;
    private int minFloors = 2;

    private List<Vector3> arrayOfAllFloorIndicators;
    private List<int> floorsToBeFulfilled;

    private float elevatorSpeed = 0.05f;
    private float secondsToWaitAtAFloor = 5.0f;

    private float distanceBetweenFloors = 0.0f;

    private float timePassedSinceLiftLastStopped = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        NotificationManager.AddObserver(this);
        arrayOfAllFloorIndicators = new List<Vector3>();
        floorsToBeFulfilled = new List<int>();

        LiftDirectionIndicatorImage.sprite = SpriteForInactiveIndication;
    }

    void ResetFloorsToBeFulfilled()
    {
        floorsToBeFulfilled.Clear();
    }

    public void ConfigureElevator(int numFloors)
    {
        if (numFloors > maxFloors)
        {
            TotalNumberOfFloors = maxFloors;
        }
        else if (numFloors < minFloors)
        {
            TotalNumberOfFloors = minFloors;
        }
        else
        {
            TotalNumberOfFloors = numFloors;
        }


        //Compute and place the floor indicators
        distanceBetweenFloors = (TopMostPoint.transform.position.y - BottomMostPoint.transform.position.y) / (TotalNumberOfFloors + 1);
        for (int i = 0; i < TotalNumberOfFloors; i++)
        {
            GameObject newFloorIndicatorPrefab = GameObject.Instantiate(FloorIndicatorPrefab);
            newFloorIndicatorPrefab.transform.parent = ElevatorParent.transform;
            newFloorIndicatorPrefab.transform.position = new Vector3(
                newFloorIndicatorPrefab.transform.position.x,
                BottomMostPoint.transform.position.y + ((i + 1) * distanceBetweenFloors),
                newFloorIndicatorPrefab.transform.position.z
                );

            arrayOfAllFloorIndicators.Add(newFloorIndicatorPrefab.transform.position);

            GameObject newOnLiftFloorButtonPrefab = GameObject.Instantiate(OnLiftFloorButtonPrefab);
            newOnLiftFloorButtonPrefab.GetComponent<RectTransform>().parent = OnLiftFloorButtonsPanel.GetComponent<RectTransform>();
            newOnLiftFloorButtonPrefab.GetComponent<RectTransform>().position = OnLiftFloorButtonsPanel.GetComponent<RectTransform>().position;
            newOnLiftFloorButtonPrefab.GetComponent<OnLiftButtonScript>().floorNumber = i;
            newOnLiftFloorButtonPrefab.GetComponentInChildren<TMPro.TMP_Text>().text = $"{i}";

            GameObject newOnFloorButtonPrefab = GameObject.Instantiate(OnFloorButtonPrefab);
            newOnFloorButtonPrefab.GetComponent<RectTransform>().parent = OnFloorButtonsPanel.GetComponent<RectTransform>();
            newOnFloorButtonPrefab.GetComponent<RectTransform>().position = OnFloorButtonsPanel.GetComponent<RectTransform>().position;
            newOnFloorButtonPrefab.GetComponent<OnFloorButtonScript>().floorNumber = numFloors - i - 1;

            if (i == 0)
            {
                ElevatorObject.transform.position = new Vector3(0.0f, newFloorIndicatorPrefab.transform.position.y, -3.0f);
            }
        }

    }

    private void FixedUpdate()
    {
        //Allow the door to be opened only when the elevator is stopped
        if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentState == ElevatorState.Stopped || ElevatorObject.GetComponent<ElevatorObjectScript>().currentState == ElevatorState.DoorOpen)
        {
            OpenDoorButton.GetComponent<Button>().enabled = true;
            OpenDoorButton.GetComponent<Image>().color = Color.white;
            timePassedSinceLiftLastStopped += Time.deltaTime;
        }
        else
        {
            OpenDoorButton.GetComponent<Button>().enabled = false;
            OpenDoorButton.GetComponent<Image>().color = Color.gray;
            timePassedSinceLiftLastStopped = 0.0f;
        }

        if ((ElevatorObject.GetComponent<ElevatorObjectScript>().currentState == ElevatorState.DoorOpen) &&
            (timePassedSinceLiftLastStopped >= secondsToWaitAtAFloor))
        {
            ElevatorObject.GetComponent<ElevatorObjectScript>().currentState = ElevatorState.Stopped;
        }

        if ((timePassedSinceLiftLastStopped >= secondsToWaitAtAFloor) && 
            (floorsToBeFulfilled.Count > 0) &&
            (ElevatorObject.GetComponent<ElevatorObjectScript>().currentState == ElevatorState.Stopped))
        {
            timePassedSinceLiftLastStopped = 0.0f;

            if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentFloor < floorsToBeFulfilled[0])
            {
                ElevatorObject.GetComponent<ElevatorObjectScript>().currentState = ElevatorState.MovingUp;
            }
            else
            {
                ElevatorObject.GetComponent<ElevatorObjectScript>().currentState = ElevatorState.MovingDown;
            }
        }

        //Check if the elevator has reached a certain floor
        if ((ElevatorObject.GetComponent<ElevatorObjectScript>().currentState != ElevatorState.Stopped) &&
            (ElevatorObject.GetComponent<ElevatorObjectScript>().currentState != ElevatorState.DoorOpen))
        {
            foreach (Vector3 aFloorIndicatorPosition in arrayOfAllFloorIndicators)
            {
                if (Mathf.Abs(aFloorIndicatorPosition.y - ElevatorObject.transform.position.y) <= 0.1f) {

                    //Compute the floor number
                    int floorNumber = ((int)Mathf.Floor((aFloorIndicatorPosition.y - BottomMostPoint.transform.position.y) / distanceBetweenFloors)) - 1;
                    ElevatorObject.GetComponent<ElevatorObjectScript>().currentFloor = floorNumber;
                    Debug.Log("REACHED: " + floorNumber);

                    foreach (int aTargetFloorNumber in floorsToBeFulfilled)
                    {
                        if (floorNumber == aTargetFloorNumber)
                        {
                            ElevatorObject.GetComponent<ElevatorObjectScript>().currentState = ElevatorState.DoorOpen;
                            ElevatorObject.transform.position = aFloorIndicatorPosition;
                            floorsToBeFulfilled.Remove(floorNumber);

                            //Trigger a notification
                            Dictionary<string, System.Object> payloadForNotification = new Dictionary<string, System.Object>();
                            payloadForNotification["name"] = "OnLiftReached";
                            payloadForNotification["floorReached"] = floorNumber;
                            NotificationManager.PostNotification(payloadForNotification);

                            break;
                        }
                    }

                    break;
                }
            }

        }

        if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentState == ElevatorState.MovingDown)
        {
            ElevatorObject.transform.position = new Vector3(
                ElevatorObject.transform.position.x, 
                ElevatorObject.transform.position.y - elevatorSpeed,
                ElevatorObject.transform.position.z);

            LiftDirectionIndicatorImage.sprite = SpriteForDownIndication;
        }
        else if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentState == ElevatorState.MovingUp)
        {
            ElevatorObject.transform.position = new Vector3(
                ElevatorObject.transform.position.x,
                ElevatorObject.transform.position.y + elevatorSpeed,
                ElevatorObject.transform.position.z);

            LiftDirectionIndicatorImage.sprite = SpriteForUpIndication;
        }

        if (floorsToBeFulfilled.Count == 0)
        {
            LiftDirectionIndicatorImage.sprite = SpriteForInactiveIndication;
        }

        if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentState == ElevatorState.DoorOpen)
        {
            DoorIsOpenIndicatorText.text = "Door is open";
        }
        else
        {
            DoorIsOpenIndicatorText.text = "";
        }

        CurrentFloorText.text = $"{ElevatorObject.GetComponent<ElevatorObjectScript>().currentFloor}";
    }

    public void BroadcastTriggered(Dictionary<string, object> userInfo)
    {
        if ((string)userInfo["name"] == "OnLiftButtonPressed")
        {
            OnLiftButtonScript onLiftButtonScript = (OnLiftButtonScript)userInfo["onLiftButtonScript"];

            if (onLiftButtonScript.currentState == OnLiftButtonState.Fulfilled)
            {
                onLiftButtonScript.gameObject.GetComponent<Image>().color = Color.yellow;
                onLiftButtonScript.currentState = OnLiftButtonState.Unfulfilled;

                if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentState == ElevatorState.Stopped)
                {
                    //Don't do anything if the elevator is at the same floor
                    if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentFloor == onLiftButtonScript.floorNumber)
                    {
                        onLiftButtonScript.gameObject.GetComponent<Image>().color = Color.white;
                        onLiftButtonScript.currentState = OnLiftButtonState.Fulfilled;
                    }
                    else if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentFloor < onLiftButtonScript.floorNumber)
                    {
                        ElevatorObject.GetComponent<ElevatorObjectScript>().currentState = ElevatorState.MovingUp;

                        if (floorsToBeFulfilled.Contains(onLiftButtonScript.floorNumber) == false)
                            floorsToBeFulfilled.Add(onLiftButtonScript.floorNumber);
                    }
                    else if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentFloor > onLiftButtonScript.floorNumber)
                    {
                        ElevatorObject.GetComponent<ElevatorObjectScript>().currentState = ElevatorState.MovingDown;

                        if (floorsToBeFulfilled.Contains(onLiftButtonScript.floorNumber) == false)
                            floorsToBeFulfilled.Add(onLiftButtonScript.floorNumber);
                    }

                }
                else
                {
                    if (floorsToBeFulfilled.Contains(onLiftButtonScript.floorNumber) == false)
                    {
                        floorsToBeFulfilled.Add(onLiftButtonScript.floorNumber);
                    }
                }
            }

        }
        else if ((string)userInfo["name"] == "OnFloorButtonPressed")
        {
            OnFloorButtonScript onFloorButtonScript = (OnFloorButtonScript)userInfo["onFloorButtonScript"];

            if (onFloorButtonScript.currentState == OnFloorButtonState.Fulfilled)
            {
                onFloorButtonScript.gameObject.GetComponent<Image>().color = Color.yellow;
                onFloorButtonScript.currentState = OnFloorButtonState.Unfulfilled;

                if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentState == ElevatorState.Stopped)
                {
                    //Don't do anything if the elevator is at the same floor
                    if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentFloor == onFloorButtonScript.floorNumber)
                    {
                        onFloorButtonScript.gameObject.GetComponent<Image>().color = Color.white;
                        onFloorButtonScript.currentState = OnFloorButtonState.Fulfilled;
                    }
                    else if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentFloor < onFloorButtonScript.floorNumber)
                    {
                        ElevatorObject.GetComponent<ElevatorObjectScript>().currentState = ElevatorState.MovingUp;
                        if (floorsToBeFulfilled.Contains(onFloorButtonScript.floorNumber) == false)
                            floorsToBeFulfilled.Add(onFloorButtonScript.floorNumber);
                    }
                    else if (ElevatorObject.GetComponent<ElevatorObjectScript>().currentFloor > onFloorButtonScript.floorNumber)
                    {
                        ElevatorObject.GetComponent<ElevatorObjectScript>().currentState = ElevatorState.MovingDown;
                        if (floorsToBeFulfilled.Contains(onFloorButtonScript.floorNumber) == false)
                            floorsToBeFulfilled.Add(onFloorButtonScript.floorNumber);
                    }

                }
                else
                {
                    if (floorsToBeFulfilled.Contains(onFloorButtonScript.floorNumber) == false)
                        floorsToBeFulfilled.Add(onFloorButtonScript.floorNumber);
                }
            }
        }
        else if ((string)userInfo["name"] == "OpenDoorButtonPressed")
        {
            timePassedSinceLiftLastStopped = 0.0f;
            ElevatorObject.GetComponent<ElevatorObjectScript>().currentState = ElevatorState.DoorOpen;
        }
    }
}
