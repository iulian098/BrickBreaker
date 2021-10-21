using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Leguar.TotalJSON;

public class LevelGenerator : MonoBehaviour
{
    public float yOffset;
    public float SpawnRate = 0.75f;
    public float explosionBrickSpawnRate = 0.25f;
    public float metalBrickSpawnRate;
    public float strongBrickSpawnRate;

    public GameObject brickPrefab;
    public GameObject explodingBrickPrefab;
    public GameObject metalBrickPrefab;
    public GameObject strongBrickPrefab;

    public int minRows = 2;
    public int maxRows = 5;
    public int minColumns = 3;
    public int maxColumns = 7;

    public Vector2 spacing;

    public bool autoColums = false;
    public bool autoRows = false;

    int columns;
    int rows;
    public List<BrickData> levelBricksLocation;
    public List<string> levelsData;
    void Start()
    {

        //float tempYOffset = yOffset * Camera.main.orthographicSize;
        //yOffset = tempYOffset;

        if (LevelManager.data.testing)
        {
            LevelManager.spawnedBricks = new List<GameObject>();
            GameObject newBrick = null;
            foreach (BrickData bd in LevelManager.data.testingLevel)
            {
                //Check for brick type and spawn it
                switch (bd.brickType)
                {
                    case BrickData.BrickType.normal:
                        newBrick = Instantiate(brickPrefab, bd.position, Quaternion.identity);
                        break;
                    case BrickData.BrickType.exploding:
                        newBrick = Instantiate(explodingBrickPrefab, bd.position, Quaternion.identity);
                        break;
                    case BrickData.BrickType.metal:
                        newBrick = Instantiate(metalBrickPrefab, bd.position, Quaternion.identity);
                        break;
                    case BrickData.BrickType.stronger:
                        newBrick = Instantiate(strongBrickPrefab, bd.position, Quaternion.identity);
                        break;
                }

                //If brick is not metal add to spawnedBrick list
                if (bd.brickType != BrickData.BrickType.metal)
                {
                    LevelManager.spawnedBricks.Add(newBrick);
                }
            }

            //Get all bricks
            LevelManager.GetBricks();

        }
        else
        {


            //Get spawn rates
            SpawnRate = LevelManager.data.bricksSpawnChance;
            explosionBrickSpawnRate = LevelManager.data.explodingBricksSpawnChance;
            metalBrickSpawnRate = LevelManager.data.metalBrickSpawnChance;
            strongBrickSpawnRate = LevelManager.data.strongBrickSpawnChance;

            strongBrickPrefab = LevelManager.data.strongerBrickPrefab;

            LevelManager.spawnedBricks = new List<GameObject>();

            //Get total levels
            int totalLevels = 0;
            totalLevels = LevelManager.data.totalLevels;

            //Load from json file if level is less or equal than totalLevels
            if (LevelManager.data.levelNumber <= totalLevels)
                LoadData();

            if (LevelManager.data.levelNumber > totalLevels && levelBricksLocation.Count == 0)
            {
                //Spawn bricks
                Debug.Log("Spawning bricks");
                SpawnBricks();
            }
            else
            {
                GameObject newBrick = null;
                foreach (BrickData bd in levelBricksLocation)
                {
                    //Check for brick type and spawn it
                    switch (bd.brickType)
                    {
                        case BrickData.BrickType.normal:
                            newBrick = Instantiate(brickPrefab, bd.position, Quaternion.identity);
                            break;
                        case BrickData.BrickType.exploding:
                            newBrick = Instantiate(explodingBrickPrefab, bd.position, Quaternion.identity);
                            break;

                        case BrickData.BrickType.metal:
                            newBrick = Instantiate(metalBrickPrefab, bd.position, Quaternion.identity);
                            break;
                        case BrickData.BrickType.stronger:
                            newBrick = Instantiate(strongBrickPrefab, bd.position, Quaternion.identity);
                            break;
                    }

                    //If brick is not metal add to spawnedBrick list
                    if (bd.brickType != BrickData.BrickType.metal)
                    {
                        Debug.Log("<color=green>Added brick " + newBrick.name + ", brick type: " + bd.brickType.ToString() + "</color>");
                        LevelManager.spawnedBricks.Add(newBrick);
                    }
                }

                //Get all bricks
                LevelManager.GetBricks();
            }
        }

    }

    void SpawnBricks()
    {
        levelBricksLocation = new List<BrickData>();

        //Set default brick position
        Vector2 brickPosition = Vector2.zero;

        //Calculate the distance between the x bound
        if (autoColums)
        {
            maxColumns = (int)GameManager.GetBounds().x / 2 - 1;
        }

        //Calculate the distance between cented and y bound
        if (autoRows)
        {
            maxRows = (int)GameManager.GetBounds().y - 2 - (int)yOffset;
        }

        //Assign random number of rown and columns
        columns = Random.Range(minColumns, maxColumns + 1);
        rows = Random.Range(minRows, maxRows + 1);


        //Start spawning bricks
        for (int i = 0; i < columns; i++)
        {
            //Set brick x position
            brickPosition.x = i * spacing.x;

            for (int j = 0; j < rows; j++)
            {

                //Generate chances
                float chance = Random.Range(0f, 1f);
                float explosionChance = Random.Range(0f, 1f);
                float metalChance = Random.Range(0f, 1f);
                float strongChance = Random.Range(0f, 1f);

                //Set brick y position
                brickPosition.y = j * spacing.y + yOffset;
                if (chance <= SpawnRate)
                {
                    bool skipBrick = false;
                    if (i == 0)
                    {
                        skipBrick = false;
                        GameObject brick;
                        if (explosionChance <= explosionBrickSpawnRate)
                        {
                            //Spawn exploding brick
                            brick = Instantiate(explodingBrickPrefab, brickPosition, Quaternion.identity);
                        }
                        else if(metalChance > explosionBrickSpawnRate && metalChance <= explosionBrickSpawnRate + metalBrickSpawnRate)
                        {
                            //Spawn metal brick
                            skipBrick = true;
                            brick = Instantiate(metalBrickPrefab, brickPosition, Quaternion.identity);
                        }
                        else if (strongChance > explosionBrickSpawnRate + metalBrickSpawnRate && strongChance <= explosionBrickSpawnRate + metalBrickSpawnRate + strongBrickSpawnRate)
                        {
                            //Spawn strong brick
                            brick = Instantiate(strongBrickPrefab, brickPosition, Quaternion.identity);
                        }
                        else
                        {
                            //Spawn normal brick
                            brick = Instantiate(brickPrefab, brickPosition, Quaternion.identity);
                        }

                        //Add brick to list
                        if(!skipBrick)
                            LevelManager.spawnedBricks.Add(brick);
                    }
                    else
                    {
                        skipBrick = false;
                        GameObject brick1;
                        GameObject brick2;
                        
                        if (explosionChance <= explosionBrickSpawnRate)
                        {
                            //Spawn exploding brick
                            brick1 = Instantiate(explodingBrickPrefab, brickPosition, Quaternion.identity);
                            brick2 = Instantiate(explodingBrickPrefab, new Vector2(-brickPosition.x, brickPosition.y), Quaternion.identity);
                        }
                        else if (metalChance >= explosionBrickSpawnRate && metalChance <= explosionBrickSpawnRate + metalBrickSpawnRate)
                        {
                            //Spawn metal brick
                            skipBrick = true;
                            brick1 = Instantiate(metalBrickPrefab, brickPosition, Quaternion.identity);
                            brick2 = Instantiate(metalBrickPrefab, new Vector2(-brickPosition.x, brickPosition.y), Quaternion.identity);
                        }
                        else if (strongChance > explosionBrickSpawnRate + metalBrickSpawnRate && strongChance <= explosionBrickSpawnRate + metalBrickSpawnRate + strongBrickSpawnRate)
                        {
                            //Spawn strong brick
                            brick1 = Instantiate(strongBrickPrefab, brickPosition, Quaternion.identity);
                            brick2 = Instantiate(strongBrickPrefab, new Vector2(-brickPosition.x, brickPosition.y), Quaternion.identity);
                        }
                        else
                        {
                            //Spawn normal brick
                            brick1 = Instantiate(brickPrefab, brickPosition, Quaternion.identity);
                            brick2 = Instantiate(brickPrefab, new Vector2(-brickPosition.x, brickPosition.y), Quaternion.identity);
                        }

                        if (!skipBrick)
                        {
                            //Add bricks to the list
                            LevelManager.spawnedBricks.Add(brick1);
                            LevelManager.spawnedBricks.Add(brick2);
                        }
                    }
                }
            }
        }

        //Add bricks to the list
        LevelManager.GetBricks();
    }

    List<string> data = new List<string>();

    void AddBricksToList()
    {
        foreach(BrickData bd in levelBricksLocation)
        {
            data.Add(JsonUtility.ToJson(bd));
        }
        data.Add("#");
    }

    public void SaveData()
    {
        
        foreach(BrickData bd in levelBricksLocation)
        {
            data.Add(JsonUtility.ToJson(bd));
        }
        File.WriteAllLines(Application.dataPath + "/levels.json", data.ToArray());

        Debug.Log("Data saved");
    }


    public TextAsset textAsset;
    public void LoadData()
    {
        List<List<BrickData>> dataList = new List<List<BrickData>>();
        List<BrickData> levelData = new List<BrickData>();

        levelBricksLocation = new List<BrickData>();
        BrickData bd;


        string[] levelsSplited = textAsset.text.Split('#');
        print("levels : " + levelsSplited[LevelManager.data.levelNumber - 1]);
        
        string[] bricksList = levelsSplited[LevelManager.data.levelNumber - 1].Split('\n');

        
        foreach(string s in bricksList)
        {
            if (s.Length > 1)
            {
                bd = JsonUtility.FromJson<BrickData>(s);
                levelBricksLocation.Add(bd);
                levelData.Add(bd);
            }
        }

        LevelManager.data.levels = new List<List<BrickData>>(dataList);
    }


}
