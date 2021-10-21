using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using System.Text;

public class LevelCreator : MonoBehaviour
{
    public static LevelCreator instance;

    public GameData data;
    public GameObject customLevelButtonPrefab;
    public Transform buttonsContainer;
    public LineRenderer gridLineHorizontal, gridLineVertical;
    public Transform leftWall, rightWall, topWall, gameOverTrigger;

    public Animator notification;
    public TMP_InputField fileNameInput;

    [Header("Brick Data")]
    public BrickData.BrickType brickType;
    public GameObject brickPrefab;


    public Vector2 brickOffset; //2.4 1.2
    public BrickListClass brickList;
    public List<BrickData> bricks;
    public List<GameObject> bricksObj;

    public bool removing;

    public float yMinPos = -1f;

    [Header("Grid offset")]
    public int topOffset = 1;
    public int sidesOffset = 2;



    public List<GameObject> levelButtons = new List<GameObject>();
    string currentDir;
    string customLevelsDirectory;
    public string[] customLevelsFiles;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentDir = Application.persistentDataPath;
        customLevelsDirectory = currentDir + "/CustomLevels";

        if (!Directory.Exists(customLevelsDirectory))
        {
            try
            {
                Debug.Log("Making CustomLevels Directory.");
                Directory.CreateDirectory(customLevelsDirectory);
                Debug.Log("Success!");
            }
            catch
            {
                Debug.LogError("Error creating CustomLevels directory.");
            }
        }

        GetCustomLevels();

        DrawGrid();

        GameManager.setWalls(leftWall, rightWall, topWall, gameOverTrigger);
        brickList.expandButton.onClick.AddListener(BrickListButton);

        if(!data)
            data = Resources.Load("GameData") as GameData;

        if (data.testing)
        {
            Time.timeScale = 1;
            data.testing = false;
            bricks = new List<BrickData>(data.testingLevel);

            GameObject newBrick = null;
            foreach (BrickData bd in data.testingLevel)
            {

                switch (bd.brickType)
                {
                    case BrickData.BrickType.normal:
                        brickPrefab = data.brickPrefab;
                        break;
                    case BrickData.BrickType.exploding:
                        brickPrefab = data.explodingBrickPrefab;
                        break;
                    case BrickData.BrickType.metal:
                        brickPrefab = data.metalBrickPrefab;
                        break;
                }
                newBrick = Instantiate(brickPrefab, bd.position, Quaternion.identity);
                bricksObj.Add(newBrick);
            }
        }

        brickPrefab = data.brickPrefab;
    }

    void BrickListButton()
    {
        if (brickList.expanded)
        {
            brickList.brickListAnim.Play("LeftShrink");
            brickList.expanded = false;
            brickList.expandButtonImage.localScale = new Vector2(-1, 1);
        }
        else
        {
            brickList.brickListAnim.Play("LeftExpand");
            brickList.expanded = true;
            brickList.expandButtonImage.localScale = new Vector2(1, 1);
        }
    }


    public void PutBrick()
    {
        Debug.Log("Put brick functino called");
        Vector2 mousePos = Input.mousePosition;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 brickPos = new Vector2(Mathf.RoundToInt(worldPos.x / brickOffset.x) * brickOffset.x, Mathf.RoundToInt(worldPos.y / brickOffset.y) * brickOffset.y);

        int maxXBricks = (int)(GameManager.GetBounds().x / brickOffset.x);
        Debug.Log("Max X Axis bricks : " + maxXBricks + " | X bound : " + GameManager.GetBounds().x);
        float remainingX = GameManager.GetBounds().x - (maxXBricks * brickOffset.x) - (brickOffset.x / 2);
        Debug.Log("Remaining X : " + remainingX);

        int maxYBricks = (int)(GameManager.GetBounds().y / brickOffset.y);
        float remainingY = GameManager.GetBounds().y - (maxYBricks * brickOffset.y) - (brickOffset.y / 2);

        float maxX = GameManager.GetBounds().x - (remainingX + brickOffset.x * (sidesOffset - 1));

        Collider2D[] overlapping = Physics2D.OverlapCircleAll(worldPos, 0.3f);
        if (overlapping.Length == 0 &&
            !removing &&
            worldPos.y >= yMinPos &&
            worldPos.y <= GameManager.GetBounds().y - (remainingY + brickOffset.y * topOffset) &&
            worldPos.x <= maxX &&
            worldPos.x >= -maxX)
        {
            Debug.Log("<color=green>Brick successfully placed</color>");
            GameObject obj = Instantiate(brickPrefab, brickPos, Quaternion.identity);
            bricksObj.Add(obj);
            BrickData bd = new BrickData(brickPos, brickType);
            bricks.Add(bd);
        }

        if (removing)
        {
            if (bricksObj.Count > 0)
            {
                int brickIndex = bricksObj.IndexOf(overlapping[0].gameObject);
                if (bricksObj.Contains(overlapping[0].gameObject))
                {
                    Destroy(bricksObj[brickIndex]);
                    bricksObj.RemoveAt(brickIndex);
                    bricks.RemoveAt(brickIndex);
                }
            }
        }
    }

    public void ToggleRemoving()
    {
        if (removing)
        {
            removing = false;
        }
        else
        {
            removing = true;
        }
    }

    public void SetBrickPrefab(GameObject brick, BrickData.BrickType type)
    {
        brickPrefab = brick;
        brickType = type;
    }

    public void TestLevel()
    {

        data.testing = true;
        data.testingLevel = new List<BrickData>(bricks);
        SceneManager.LoadScene("Level1");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void DrawGrid()
    {

        //Get max X axis bricks
        int maxXBricks = (int)(GameManager.GetBounds().x / brickOffset.x) - sidesOffset + 2;
        float remainingX = GameManager.GetBounds().x - (maxXBricks * brickOffset.x) - (brickOffset.x / 2);

        //Get max Y axis bicks
        int maxYBricks = (int)(GameManager.GetBounds().y / brickOffset.y) - topOffset + 1;
        float remainingY = GameManager.GetBounds().y - (maxYBricks * brickOffset.y) - (brickOffset.y / 2);

        #region Hrizontal
        List<Vector3> yPositions = new List<Vector3>();

        //Calculate max X pos
        float maxX = (maxXBricks * brickOffset.x) - (brickOffset.x / 2);//GameManager.GetBounds().x + 2f;
        float minY = Mathf.RoundToInt(yMinPos) * (brickOffset.y / 2) * 2;
        float maxY = (maxYBricks * brickOffset.y) - (brickOffset.y / 2);// - (brickOffset.y - topOffset);

        int yLines = Mathf.RoundToInt(maxY);

        bool yNegate = false;
        for (int i = -2; i < maxYBricks; i++)
        {
            Vector3 pos1;
            Vector3 pos2;
            float yPos = brickOffset.y * i + (brickOffset.y / 2);
            if (i == -2)
                minY = yPos;
            if (!yNegate)
            {
                pos1 = new Vector3(maxX, yPos, 0);
                pos2 = new Vector3(-maxX, yPos, 0);
            }
            else
            {
                pos1 = new Vector3(-maxX, yPos, 0);
                pos2 = new Vector3(maxX, yPos, 0);
            }
            yNegate = !yNegate;

            yPositions.Add(pos1);
            yPositions.Add(pos2);
        }

        gridLineHorizontal.positionCount = yPositions.Count;
        gridLineHorizontal.SetPositions(yPositions.ToArray());

        Debug.Log("Positions count : " + yPositions.Count);

        #endregion

        #region Vertical

        List<Vector3> xPositions = new List<Vector3>();

        

        int xLines = maxXBricks - sidesOffset + 2;//Mathf.RoundToInt(maxX / brickOffset.x);

        bool xNegate = false;

        for (int i = -maxXBricks; i < maxXBricks; i++)
        {
            Vector3 pos1;
            Vector3 pos2;
            float xPos = brickOffset.x * i + (brickOffset.x / 2);
            if (!xNegate)
            {
                pos1 = new Vector3(xPos, maxY, 0);
                pos2 = new Vector3(xPos, minY, 0);
            }
            else
            {
                pos1 = new Vector3(xPos, minY, 0);
                pos2 = new Vector3(xPos, maxY, 0);
            }
            xNegate = !xNegate;

            xPositions.Add(pos1);
            xPositions.Add(pos2);
        }

        gridLineVertical.positionCount = xPositions.Count;
        gridLineVertical.SetPositions(xPositions.ToArray());

        Debug.Log("Positions count : " + xPositions.Count);

        #endregion
    }


    void GetCustomLevels()
    {
        if(levelButtons.Count > 0)
        {
            foreach(GameObject go in levelButtons)
            {
                Destroy(go);
            }
            levelButtons.Clear();
        }

        if (Directory.Exists(customLevelsDirectory))
        {
            customLevelsFiles = Directory.GetFiles(customLevelsDirectory);
        }

        foreach (string file in customLevelsFiles)
        {
            GameObject go = Instantiate(customLevelButtonPrefab, buttonsContainer);
            LoadLevelButton llb = go.GetComponent<LoadLevelButton>();
            string levelName = file.Remove(0, file.LastIndexOf("/") + 1);
            llb.Setup(levelName, file);
            levelButtons.Add(go);
        }
    }

    void SpawnBrick()
    {
        GameObject newBrick = null;
        foreach (BrickData bd in bricks)
        {
            switch (bd.brickType)
            {
                case BrickData.BrickType.normal:
                    brickPrefab = data.brickPrefab;
                    break;
                case BrickData.BrickType.exploding:
                    brickPrefab = data.explodingBrickPrefab;
                    break;
                case BrickData.BrickType.metal:
                    brickPrefab = data.metalBrickPrefab;
                    break;
            }
            newBrick = Instantiate(brickPrefab, bd.position, Quaternion.identity);
            bricksObj.Add(newBrick);
        }

        brickPrefab = data.brickPrefab;
    }

    public void LoadFromFile(string path)
    {

        if(bricks.Count > 0)
        {
            bricks.Clear();
        }

        if(bricksObj.Count > 0)
        {
            foreach(GameObject obj in bricksObj)
            {
                Destroy(obj);
            }
            bricksObj.Clear();
        }

        string[] levelData;
        try
        {
            levelData = File.ReadAllLines(path);
            
            foreach(string s in levelData)
            {
                if (s.Length > 1)
                {
                    BrickData bd = JsonUtility.FromJson<BrickData>(s);
                    bricks.Add(bd);
                }
            }

            SpawnBrick();

        }
        catch (IOException ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public void SaveToFile()
    {
        string fileName = customLevelsDirectory + "/" + fileNameInput.text;
        string json = "";

        foreach(BrickData bd in bricks)
        {
            json = json + JsonUtility.ToJson(bd) + "\n";
        }

        FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate);
        try
        {
            Debug.Log("Writing to file.");
            stream.Write(Encoding.ASCII.GetBytes(json), 0, json.Length);
            Debug.Log("Success!");
            notification.Play("Fade Effect");
        }
        catch
        {
            Debug.LogError("Failed to write in file");
        }

        GetCustomLevels();

    }

   
    public void OnDrawGizmos()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Gizmos.DrawSphere(worldPos, 0.3f);
    }



}
