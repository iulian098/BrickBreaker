using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{

    [System.Serializable]
    public class WinText
    {
        public TMP_Text scoreText;
        public TMP_Text lifesScoreText;
        public TMP_Text totalScoreText;
        public TMP_Text collectedCoinsText;
    }
    public static LevelManager instance;

    public static GameData data;

    public int score;
    public int scorePerBrick; //How much score will win per brick destroyed
    public int lifes;

    public GameObject topWall, leftWall, rightWall, gameOverTrigger; //Walls

    public Sprite heartEmpty, heartFill; //Lifes

    public GameObject ballPrefab;

    public static List<GameObject> bricks = new List<GameObject>();
    public static List<GameObject> spawnedBricks = new List<GameObject>();
    public int bricksCount;
    public List<Ball> balls;
    public List<Image> lifesImages;
    public static GameObject paddle;
    public SpriteRenderer paddleRenderer;

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject winPanel;
    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;
    public GameObject testingPanel;

    [Header("Animators")]
    public Animator cameraAnimator;
    public List<Animator> stars = new List<Animator>(); 

    [Header("Text")]
    //Text
    public WinText winPanelText;
    public TMP_Text scoreText;
    public TMP_Text coinsText;
    public TMP_Text levelText;

    Vector2 bounds;
    bool pause;
    bool levelEnded;
    int ballSpeed;
    int collectedCoins;
    int currentDifficulty;

    AudioSource audioManager;

    public static Dictionary<string, bool> powersActive;

    public static List<Ball> stickedBalls = new List<Ball>(); //List of sticked balls

    IEnumerator powerUpCoroutine;
    IEnumerator magnetCoroutine;

    [Header("Android")]
    public List<GameObject> androidObjects;
    public GameObject launchBallButton;

    float lastTimeSpawned;
    bool canSpawnPowerUp;

    private void Awake()
    {
        instance = this;

        //Load game data
        data = Resources.Load("GameData") as GameData;

        //Get paddle
        if (!paddle)
            paddle = GameObject.FindGameObjectWithTag("Player");
        Vector3 tempPaddlePos = paddle.transform.position;
        tempPaddlePos.y = -Camera.main.orthographicSize + 2f;
        paddle.transform.position = tempPaddlePos;

        if (paddle && !paddleRenderer)
            paddleRenderer = paddle.GetComponent<SpriteRenderer>();

        

    }

    void Start()
    {
        //Set default text
        scoreText.text = "Score: 0";
        coinsText.text = "0";
        levelText.text = "Level " + data.levelNumber;

        //Get screen bounds
        bounds = GameManager.GetBounds();

        //Get balls
        GetBalls();
        stickedBalls = new List<Ball>(balls);

        //Load difficulty
        LoadDifficulty(PlayerPrefs.GetInt("Difficulty", 0));


        //Setup walls
        SetWalls();

        #region Events
        //Setup event
        EventManager.gameOverEvent.AddListener(GameOver);

        EventManager.addScoreEvent.AddListener(AddScore);
        EventManager.addScoreEvent.AddListener(SpawnPowerUp);
        EventManager.addScoreEvent.AddListener(SpawnBrickParticles);

        EventManager.powerPickupEvent.AddListener(PowerUp);

        EventManager.stickToPaddleEvent.AddListener(StickToPaddle);

        EventManager.coinPickUpEvent.AddListener(AddCoins);

        EventManager.explodeEvent.AddListener(DestroyNearby);

        EventManager.ballHitEvent.AddListener(BallHit);

        EventManager.brickDamageEvent.AddListener(GiveBrickDamage);

        #endregion

        //Setup powers
        powersActive = GameManager.CreatePowerList();

        audioManager = GetComponent<AudioSource>();

#if UNITY_ANDROID
        //Enable android stuff
        foreach (GameObject androidObj in androidObjects)
        {
            androidObj.SetActive(true);
        }
#else
        //Disable android stuff
        foreach (GameObject androidObj in androidObjects)
        {
            androidObj.SetActive(false);
        }
#endif

    }

    void Update()
    {
        bricksCount = bricks.Count;
#if UNITY_ANDROID
        //Disable/Enable launch ball button
        if(stickedBalls.Count == 0 && launchBallButton.activeSelf)
        {
            launchBallButton.SetActive(false);
        }
        else if(stickedBalls.Count > 0 && !launchBallButton.activeSelf)
        {
            launchBallButton.SetActive(true);
        }

#endif
        //Check for win
        if(bricks.Count == 0 && !winPanel.activeSelf)
        {
            WinGame();
        }

        //Pause
        if (Input.GetButtonDown("Cancel") && !levelEnded)
        {
            pause = !pause;
            if (pause)
                PauseGame();
            else
                ResumeGame();
        }
    }

    void FixedUpdate()
    {
        if(!canSpawnPowerUp && Time.time - lastTimeSpawned > data.timePowerupSpawn)
        {
            canSpawnPowerUp = true;
            Debug.Log("Power Up can be spawned");
        }
    }

    void LoadDifficulty(int difficulty)
    {
        SetupBalls(data.difficulties[difficulty]);
        currentDifficulty = difficulty;
    }

    void SetupBalls(int speed)
    {
        ballSpeed = speed;
        foreach (Ball b in balls)
            b.SetBallSpeed(speed);
    }

    public static void GetBricks()
    {

        //Get all bricks
        bricks = new List<GameObject>(spawnedBricks);
        Debug.Log("<color=orange>Bricks count is " + bricks.Count + "</color>");
    }

    void SetWalls()
    {

        GameManager.setWalls(leftWall.transform, rightWall.transform, topWall.transform, gameOverTrigger.transform);

    }

    #region Win/Lose

    void WinGame()
    {
        mainPanel.SetActive(false);
        if (data.testing)
        {
            testingPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            winPanel.SetActive(true);
            StopBalls();
            data.coins += collectedCoins;
            data.levelNumber++;
            audioManager.PlayOneShot(data.winAudio);
            levelEnded = true;

            //Disable magnet
            if (magnetCoroutine != null)
                StopCoroutine(magnetCoroutine);
            Magnet(false);

            //Save data
            PlayerPrefs.SetInt("Coins", data.coins);
            PlayerPrefs.SetInt("LevelNumber", data.levelNumber);
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);

            //Setup text
            winPanelText.scoreText.text = "Score: " + score;
            winPanelText.lifesScoreText.text = "Remaining lifes: " + (lifes + 1).ToString() + " * 50";
            winPanelText.totalScoreText.text = "Total score: " + (score + ((lifes + 1) * 50));
            winPanelText.collectedCoinsText.text = "Collected coins: " + collectedCoins;

            //Stop coroutine
            StopAllCoroutines();

            //Fill stars
            StartCoroutine(fillStars(0.35f, lifes + 1));
        }
    }

    public void GameOver()
    {
        if (balls.Count == 0 && lifes == 0)
        {
            mainPanel.SetActive(false);
            if (data.testing)
                testingPanel.SetActive(true);
            else
                gameOverPanel.SetActive(true);
            lifesImages[lifes].sprite = heartEmpty;
            audioManager.PlayOneShot(data.loseAudio);
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);
            levelEnded = true;
            StopAllCoroutines();
            Magnet(false);
            Time.timeScale = 0;
        }
        else if(lifes > 0 && balls.Count == 0){

            lifesImages[lifes].sprite = heartEmpty;

            AddBall();

            SetupBalls(ballSpeed);
            lifes--;
        }
    }

    #endregion

    void AddScore(GameObject _brick)
    {
        score += scorePerBrick;
        scoreText.text = "Score: " + score;
        audioManager.PlayOneShot(data.brickDestroyAudio);
        bricks.Remove(_brick);
        _brick.SetActive(false);
    }

    void SpawnPowerUp(GameObject _brick)
    {
        float rand = Random.Range(0f, 1f);
        if(rand <= data.spawnPowerUpChance && canSpawnPowerUp)
        {
            Instantiate(data.powerUpPrefab, _brick.transform.position, Quaternion.identity);
            lastTimeSpawned = Time.time;
            canSpawnPowerUp = false;
        }else if (rand > data.spawnPowerUpChance && rand <= (data.spawnPowerUpChance + data.spawnPowerUpChance) + (0.025f * currentDifficulty))
        {
            Instantiate(data.coinPrefab, _brick.transform.position, Quaternion.identity);
        }
    }

    void SpawnBrickParticles(GameObject _brick)
    {
        Instantiate(data.brickParticles, _brick.transform.position, Quaternion.identity);
    }

    public void LeadLevel(string level)
    {
        SceneManager.LoadScene(level);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenuPanel.SetActive(false);
        pause = false;
    }

    void GetBalls()
    {
        //Get balls
        balls = new List<Ball>(FindObjectsOfType<Ball>());
    }

    void AddBall()
    {
        GameObject newBall = Instantiate(ballPrefab);
        Ball newBallComponent = newBall.GetComponent<Ball>();
        balls.Add(newBallComponent);
        stickedBalls.Add(newBallComponent);
        
    }

    void StopBalls()
    {
        foreach (Ball b in balls)
            b.StopBall();
    }

    void ChangeNewBalls()
    {
        if(powersActive["BigBall"])
        {
            foreach (Ball b in balls)
            {
                b.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            }
        }else if(powersActive["SmallBall"])
        {
            foreach (Ball b in balls)
            {
                b.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            }
        }
    }

#region PowerUp

    void PowerUp(PowerUpType type)
    {
        Debug.Log("PowerUp picked");
        if (type == PowerUpType.MultiBall)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject newBall = Instantiate(ballPrefab, balls[0].transform.position, Quaternion.identity);
                Ball newBallScript = newBall.GetComponent<Ball>();
                balls.Add(newBallScript);
                newBallScript.SetBallSpeed(ballSpeed);
                switch (i)
                {
                    case 0:
                        newBallScript.StartNewBall(balls[0], new Vector2(-1, 0));
                        break;
                    case 1:
                        newBallScript.StartNewBall(balls[0], new Vector2(1, 0));
                        break;
                }
                
                ChangeNewBalls();
            }
        }else if(type == PowerUpType.BigBall)
        {

            if (powerUpCoroutine != null)
            {
                StopCoroutine(powerUpCoroutine);
                powersActive["SmallBall"] = false;
                powersActive["BigBall"] = false;
            }
            powerUpCoroutine = BigBallPower(data.PowerUpDuration);
            StartCoroutine(powerUpCoroutine);

        }else if(type == PowerUpType.SmallBall)
        {

            if (powerUpCoroutine != null)
            {
                StopCoroutine(powerUpCoroutine);
                powersActive["BigBall"] = false;
                powersActive["SmallBall"] = false;
            }
            powerUpCoroutine = SmallBallPower(data.PowerUpDuration);
            StartCoroutine(powerUpCoroutine);

        }
        else
        {
            StartCoroutine(MagnetPower(data.PowerUpDuration));
        }

        audioManager.PlayOneShot(data.powerUpPickupAudio);
    }

    IEnumerator BigBallPower(float time)
    {
        ChangeBallSize(new Vector3(1.5f, 1.5f, 1));
        powersActive["BigBall"] = true;

        yield return new WaitForSeconds(time);

        ChangeBallSize(Vector3.one);
        powersActive["BigBall"] = false;
    }

    IEnumerator SmallBallPower(float time)
    {
        ChangeBallSize(new Vector3(0.5f, 0.5f, 1));
        powersActive["SmallBall"] = true;


        yield return new WaitForSeconds(time);

        ChangeBallSize(Vector3.one);
        powersActive["SmallBall"] = false;

    }

    void ChangeBallSize(Vector3 _size)
    {
        foreach(Ball b in balls)
        {
            b.gameObject.transform.localScale = _size;
        }
    }

    void Magnet(bool enable)
    {
        if(enable)
            paddleRenderer.material.SetInt("_Magnet", 1);
        else
            paddleRenderer.material.SetInt("_Magnet", 0);
        PlayerController.magnetActive = enable;
        powersActive["Magnet"] = enable;
    }

    IEnumerator MagnetPower(float time)
    {
        Magnet(true);

        yield return new WaitForSeconds(time);

        Magnet(false);
    }

    void StickToPaddle(GameObject _ball)
    {
        if (PlayerController.magnetActive)
        {
            _ball.transform.SetParent(paddle.transform);
            Ball b = _ball.GetComponent<Ball>();
            b.rb.simulated = false;
            b.sticked = true;
            stickedBalls.Add(b);
        }
    }

#endregion

    void AddCoins()
    {
        collectedCoins++;
        coinsText.text = collectedCoins.ToString();
        audioManager.PlayOneShot(data.coinPickupAudio);
    }

    void BallHit(Vector2 _point)
    {
        audioManager.PlayOneShot(data.hitAudio);

        Instantiate(data.ballHitEffect, _point, Quaternion.identity);
    }

    void DestroyNearby(GameObject _brick)
    {
        if(cameraAnimator)
            cameraAnimator.Play("CameraShaking");

        Collider2D[] colls = Physics2D.OverlapCircleAll(_brick.transform.position, 3f);
        foreach(Collider2D coll in colls)
        {
            if (coll.tag == "Brick")
            {
                EventManager.addScoreEvent.Invoke(coll.gameObject);
            }
            else if(coll.tag == "ExplodingBrick")
            {
                EventManager.addScoreEvent.Invoke(coll.gameObject);

                EventManager.explodeEvent.Invoke(coll.gameObject);
            }
            else if(coll.tag == "StrongBrick")
            {
                EventManager.brickDamageEvent.Invoke(coll.GetComponent<StrongBrick>());
            }
        }

        audioManager.PlayOneShot(data.brickExplosionAudio);
        Instantiate(data.brickExplosionParticles, _brick.transform.position, Quaternion.identity);
    }

    void GiveBrickDamage(StrongBrick sb)
    {
        sb.GetDamage();
    }

    public void RestartLevel()
    {
        foreach(GameObject b in spawnedBricks)
        {
            b.SetActive(true);
        }

        foreach(Image img in lifesImages)
        {
            img.sprite = heartFill;
        }
        lifes = 2;
        score = 0;
        collectedCoins = 0;

        scoreText.text = "Score: " + score;
        Time.timeScale = 1;
        GameObject ball = Instantiate(ballPrefab);
        Ball ballScript = ball.GetComponent<Ball>();
        ballScript.SetBallSpeed(ballSpeed);
        balls.Add(ballScript);
        stickedBalls.Add(ballScript);
        bricks = new List<GameObject>(spawnedBricks);

        levelEnded = false;
    }

    public void GoToEditor()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelCreator");
    }

    IEnumerator fillStars(float time, int number)
    {
        for(int i = 0; i < number; i++)
        {
            yield return new WaitForSeconds(time);
            stars[i].Play("Star");
        }
    }

    public void StartBalls()
    {
#if UNITY_ANDROID
        foreach(Ball b in balls)
        {
            b.StartBall();
        }
#endif
    }

}
