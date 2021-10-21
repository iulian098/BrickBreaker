using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{

    [Header("Buttons containers")]
    public Transform ballListShop;
    public Transform paddleListShop;
    public Transform bricksListShop;

    [Header("Preview")]
    public Image previewSprite;
    public TMP_Text previewText;

    [Header("Buy buttons")]
    public GameObject buyBallButton;
    public GameObject buyPaddleButton;
    public GameObject buyBrickButton;

    [Header("Others")]
    public TMP_Text coinsText;

    public Button[] allButtons;

    static GameData data;
    Dictionary<string, BallShopClass> ballsDictionary = new Dictionary<string, BallShopClass>();
    Dictionary<string, BallShopClass> paddlesDictionary = new Dictionary<string, BallShopClass>();
    Dictionary<string, BricksSkins> bricksDictionary = new Dictionary<string, BricksSkins>();

    List<BallButton> ballsButtons = new List<BallButton>();
    List<BallButton> paddleButtons = new List<BallButton>();
    List<BallButton> brickButtons = new List<BallButton>();

    BallShopClass selectedBall;
    BallShopClass selectedPaddle;
    BricksSkins selectedBrick;
    AudioSource audioSource;

    void Start()
    {
        data = Resources.Load("GameData") as GameData;

        //Load Data
        data.selectedBall = PlayerPrefs.GetInt("SelectedBall", 0);
        data.selectedPaddle = PlayerPrefs.GetInt("SelectedPaddle", 0);
        data.selectedBrick = PlayerPrefs.GetInt("SelectedBrick", 0);
        data.coins = PlayerPrefs.GetInt("Coins", 0);
        data.levelNumber = PlayerPrefs.GetInt("LevelNumber", 1);
        data.testing = false;
        //Change brick
        ChangeBrickSkin(data.selectedBrick);

        //Get audio source component
        audioSource = GetComponent<AudioSource>();

        //Create dictionaries for each skin type
        CreateDictionary();

        //Spawn buttons
        SpawnBallButtons();
        SpawnPaddleButtons();
        SpawnBricksButtons();

        //Change coins text
        UpdateCoinsText();

        //Add for each button sound effect
        ButtonsAudio();
    }

    public void Play(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void Quit()
    {
        Application.Quit();
    }

    void ButtonsAudio()
    {
        foreach(Button btn in allButtons)
        {
            btn.onClick.AddListener(PlayAudio);
        }
    }

    void PlayAudio()
    {
        audioSource.PlayOneShot(data.buttonClickAudio);
    }

    public void ChangeDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 0:
                Debug.Log("Easy");
                break;
            case 1:
                Debug.Log("Medium");
                break;
            case 2:
                Debug.Log("Hard");
                break;
        }

        PlayerPrefs.SetInt("Difficulty", difficulty);
    }

    #region Spawn Buttons

    void SpawnBallButtons()
    {
        for (int i = 0; i < data.balls.Count; i++) {


            BallShopClass bsc = ballsDictionary[data.balls[i].name];
            if (i > 0)
            {
                bsc.Unlocked = Convert.ToBoolean(PlayerPrefs.GetInt(bsc.name + "_Ball", 0));
            }
            else
            {
                bsc.Unlocked = true;
            }

            GameObject btn = Instantiate(data.ballButtonPrefab, ballListShop);
            Button btnScript = btn.GetComponent<Button>();
            BallButton bBtn = btn.GetComponent<BallButton>();

            btnScript.onClick.AddListener(delegate { SelectBall(bsc); });
            if (bsc.changeMaterial)
            {
                bBtn.SetupButton(bsc.name, bsc.price, bsc.sprite, bsc.material);
            }
            else
            {
                bBtn.SetupButton(bsc.name, bsc.price, bsc.sprite, null);
            }

            if (bsc.Unlocked)
                bBtn.text.text = bsc.name;


            ballsButtons.Add(bBtn);
        }

        SelectBall(ballsDictionary[data.balls[data.selectedBall].name]);

    }

    void SpawnPaddleButtons()
    {
        for (int i = 0; i < data.paddles.Count; i++)
        {
            BallShopClass bsc = paddlesDictionary[data.paddles[i].name];
            if (i > 0)
            {
                bsc.Unlocked = Convert.ToBoolean(PlayerPrefs.GetInt(bsc.name, 0));
            }
            else
            {
                bsc.Unlocked = true;
            }

            GameObject btn = Instantiate(data.ballButtonPrefab, paddleListShop);
            Button btnScript = btn.GetComponent<Button>();
            BallButton bBtn = btn.GetComponent<BallButton>();

            btnScript.onClick.AddListener(delegate { SelectPaddle(bsc); });

            bBtn.SetupButton(bsc.name, bsc.price, bsc.sprite);

            if (bsc.Unlocked)
                bBtn.text.text = bsc.name;

            ballsButtons.Add(bBtn);
        }
    }

    void SpawnBricksButtons()
    {
        for(int i = 0; i < data.brickSkins.Count; i++)
        {
            BricksSkins bs = bricksDictionary[data.brickSkins[i].name];

            if (i == 0)
                bs.unlocked = true;
            else
                bs.unlocked = Convert.ToBoolean(PlayerPrefs.GetInt(bs.name, 0));

            GameObject btn = Instantiate(data.ballButtonPrefab, bricksListShop);
            Button btnScript = btn.GetComponent<Button>();
            BallButton bBtn = btn.GetComponent<BallButton>();

            bBtn.SetupButton(bs.name, data.brickSkins[i].price, bs.normalBrick);

            if (bs.unlocked)
                bBtn.text.text = bs.name;

            btnScript.onClick.AddListener(delegate { SelectBrick(bs); });

            brickButtons.Add(bBtn);
        }
    }

    #endregion

    #region Selection

    void SelectBall(BallShopClass _ball)
    {
        //buyPaddleButton.SetActive(false);

        previewSprite.sprite = _ball.sprite;

        if (_ball.material)
            previewSprite.material = _ball.material;
        else
            previewSprite.material = null;

        if (!_ball.Unlocked)
        {
            previewText.text = _ball.name + "\nPrice: " + _ball.price;
            buyBallButton.SetActive(true);
        }
        else
        {
            buyBallButton.SetActive(false);
            previewText.text = _ball.name;
            data.selectedBall = data.balls.IndexOf(_ball);
            PlayerPrefs.SetInt("SelectedBall", data.selectedBall);

        }

        selectedBall = _ball;
    }
    void SelectPaddle(BallShopClass _paddle)
    {
        //buyBallButton.SetActive(false);
        previewSprite.sprite = _paddle.sprite;
        if (!_paddle.Unlocked)
        {
            previewText.text = _paddle.name + "\nPrice: " + _paddle.price;
            buyPaddleButton.SetActive(true);
        }
        else
        {
            buyPaddleButton.SetActive(false);
            previewText.text = _paddle.name;
            data.selectedPaddle = data.paddles.IndexOf(_paddle);
            PlayerPrefs.SetInt("SelectedPaddle", data.selectedPaddle);
        }

        selectedPaddle = _paddle;
    }

    void SelectBrick(BricksSkins bs)
    {
        previewSprite.sprite = bs.normalBrick;
        if (!bs.unlocked)
        {
            buyBrickButton.SetActive(true);
            previewText.text = bs.name + "\nPrice: " + bs.price;
        }
        else
        {
            buyBrickButton.SetActive(false);
            previewText.text = bs.name;
            data.selectedBrick = data.brickSkins.IndexOf(bs);
            PlayerPrefs.SetInt("SelectedBrick", data.selectedBrick);
            ChangeBrickSkin(data.selectedBrick);
        }

        selectedBrick = bs;
    }

    #endregion

    #region Buy

    public void BuyBall()
    {
        if(data.coins >= selectedBall.price)
        {
            selectedBall.Unlocked = true;
            data.coins -= selectedBall.price;
        }

        PlayerPrefs.SetInt("Coins", data.coins);
        PlayerPrefs.SetInt(selectedBall.name + "_Ball", 1);
        SelectBall(selectedBall);
        UpdateCoinsText();

        UpdateButton(ballsButtons[data.balls.IndexOf(selectedBall)], selectedBall.name);

    }

    public void BuyPaddle()
    {
        if(data.coins >= selectedPaddle.price)
        {
            selectedPaddle.Unlocked = true;
            data.coins -= selectedPaddle.price;
        }

        PlayerPrefs.SetInt("Coins", data.coins);
        PlayerPrefs.SetInt(selectedPaddle.name, 1);
        SelectPaddle(selectedPaddle);
        UpdateCoinsText();

        UpdateButton(paddleButtons[data.paddles.IndexOf(selectedPaddle)], selectedPaddle.name);
    }

    public void BuyBrick()
    {
        if(data.coins >= selectedBrick.price)
        {
            selectedBrick.unlocked = true;
            data.coins -= selectedBrick.price;
        }

        PlayerPrefs.SetInt("Coins", data.coins);
        PlayerPrefs.SetInt(selectedBrick.name, 1);
        SelectBrick(selectedBrick);
        UpdateCoinsText();

        UpdateButton(brickButtons[data.brickSkins.IndexOf(selectedBrick)], selectedBrick.name);
    }

    #endregion
    
    public void GoToEditor()
    {
        SceneManager.LoadScene("LevelCreator");
    }

    void ChangeBrickSkin(int index)
    {
        data.brickPrefab.GetComponent<SpriteRenderer>().sprite = data.brickSkins[index].normalBrick;
        data.metalBrickPrefab.GetComponent<SpriteRenderer>().sprite = data.brickSkins[index].metalBrick;
        data.explodingBrickPrefab.GetComponent<SpriteRenderer>().sprite = data.brickSkins[index].normalBrick;
        data.strongerBrickPrefab.GetComponent<SpriteRenderer>().sprite = data.brickSkins[index].normalBrick;
    }

    void CreateDictionary()
    {
        for(int i = 0; i < data.balls.Count; i++)
        {
            ballsDictionary.Add(data.balls[i].name, data.balls[i]);
        }

        for (int i = 0; i < data.paddles.Count; i++)
        {
            paddlesDictionary.Add(data.paddles[i].name, data.paddles[i]);
        }

        for(int i = 0; i < data.brickSkins.Count; i++)
        {
            bricksDictionary.Add(data.brickSkins[i].name, data.brickSkins[i]);
        }
    }

    void UpdateButton(BallButton bb, string _text)
    {
         bb.text.text = _text;
    }

    void UpdateCoinsText()
    {
        coinsText.text = data.coins.ToString();
    }

    public void ChangeInputType(Dropdown dropdown)
    {
        data.inputType = dropdown.value;
    }
}
