using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElevatorState
{
    MovingUp,
    MovingDown,
    Stopped,
    DoorOpen
}

public class ElevatorObjectScript : MonoBehaviour
{
    public int currentFloor = 0;
    public ElevatorState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentFloor = 0;
        currentState = ElevatorState.Stopped;
    }
}
