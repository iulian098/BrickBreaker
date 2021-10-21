using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadLevelButton : MonoBehaviour
{
    public Button btn;
    public TMP_Text btnText;
    public string path;


    private void Start()
    {
        btn.onClick.AddListener(Load);
    }

    public void Setup(string _name, string _path)
    {
        btnText.text = _name;
        path = _path;

    }

    void Load()
    {
        LevelCreator.instance.LoadFromFile(path);
    }
}
