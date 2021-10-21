using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallButton : MonoBehaviour
{
    public Image icon;
    public TMP_Text text;

    Material mat;
    Sprite sprite;
    int price;
    string ballName;

    void Start()
    {
        icon.sprite = sprite;
        icon.material = mat;
        
    }

    public void SetupButton(string _ballName, int _price, Sprite _icon)
    {
        sprite = _icon;
        price = _price;
        ballName = _ballName;
        text.text = ballName + "\nPrice: " + price;
    }

    public void SetupButton(string _ballName, int _price, Sprite _icon, Material _mat)
    {
        sprite = _icon;
        price = _price;
        ballName = _ballName;
        mat = _mat;
        text.text = ballName + "\nPrice: " + price;
    }
}
