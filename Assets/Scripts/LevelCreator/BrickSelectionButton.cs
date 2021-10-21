using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrickSelectionButton : MonoBehaviour
{
    public Button btn;
    public BrickData.BrickType brickType;
    public GameObject prefab;

    private void Start()
    {
        btn.onClick.AddListener(onClick);
    }

    public void onClick()
    {
        LevelCreator.instance.SetBrickPrefab(prefab, brickType);
    }
}
