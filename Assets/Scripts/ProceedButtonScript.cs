using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProceedButtonScript : MonoBehaviour
{
    [SerializeField]
    GameObject InitialOptionsPanel;

    [SerializeField]
    GameObject OnLiftControlsPanel;

    [SerializeField]
    GameObject OnFloorControlsPanel;

    [SerializeField]
    ElevatorControllerScript elevatorController;

    [SerializeField]
    TMPro.TMP_InputField NumberOfFloorsInputTextField;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void ButtonPressed()
    {
        elevatorController.ConfigureElevator(int.Parse(NumberOfFloorsInputTextField.text));

        OnLiftControlsPanel.SetActive(true);
        OnFloorControlsPanel.SetActive(true);
        InitialOptionsPanel.SetActive(false);
    }
}
