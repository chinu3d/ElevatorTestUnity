using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConfiguratorScript : MonoBehaviour
{
    [SerializeField]
    GameObject InitialOptionsPanel;

    [SerializeField]
    GameObject OnLiftControlsPanel;

    [SerializeField]
    GameObject OnFloorControlsPanel;

    // Start is called before the first frame update
    void Start()
    {
        OnLiftControlsPanel.SetActive(false);
        OnFloorControlsPanel.SetActive(false);
        InitialOptionsPanel.SetActive(true);
    }

}
