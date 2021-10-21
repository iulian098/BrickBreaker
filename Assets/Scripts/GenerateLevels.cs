using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateLevels : MonoBehaviour
{
    public GameData gameData;

    public float yOffset;
    public float SpawnRate = 0.75f;
    public float explosionBrickSpawnRate = 0.25f;
    public float metalBrickSpawnRate = 0.1f;
    public float strongBrickSpawnRate;

    public int minRows = 2;
    public int maxRows = 5;
    public int minColumns = 3;
    public int maxColumns = 7;

    public Vector2 spacing;

    public bool autoColums = false;
    public bool autoRows = false;
    public int levels = 100;

    int columns;
    int rows;
    List<BrickData> levelBricksLocation;
    List<BrickData> metalBricks = new List<BrickData>();
    List<string> data = new List<string>();

    public void Generate()
    {
        //Get spawn rates
        SpawnRate = gameData.bricksSpawnChance;
        explosionBrickSpawnRate = gameData.explodingBricksSpawnChance;
        metalBrickSpawnRate = gameData.metalBrickSpawnChance;
        strongBrickSpawnRate = gameData.strongBrickSpawnChance;

        //Start generating
        for (int i = 0; i < levels; i++)
        {
            SpawnBricks(i);
        }

        //Save data in file
        SaveData();

        Debug.Log("<color=green>All levels saved in levels.json</color>");

    }

    void SpawnBricks(int level)
    {
        //Creating new list of brick locations
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
                    if (i == 0)
                    {
                        BrickData bd;
                        if (explosionChance <= explosionBrickSpawnRate && level > gameData.levelExplodingBricks)
                        {
                            //Create exploding brick
                            bd = new BrickData(brickPosition, BrickData.BrickType.exploding);
                        }else if(metalChance >= explosionBrickSpawnRate && metalChance <= explosionBrickSpawnRate + metalBrickSpawnRate && level > gameData.levelMetalBrick)
                        {
                            //Create metal brick
                            bd = new BrickData(brickPosition, BrickData.BrickType.metal);
                        }
                        else if (strongChance > explosionBrickSpawnRate + metalBrickSpawnRate && strongChance <= explosionBrickSpawnRate + metalBrickSpawnRate + strongBrickSpawnRate && level > gameData.levelStrongBrick)
                        {
                            //Create strong brick
                            bd = new BrickData(brickPosition, BrickData.BrickType.stronger);
                        }
                        else
                        {
                            //Create normal brick
                            bd = new BrickData(brickPosition, BrickData.BrickType.normal);
                        }

                        //Add brick to list
                        levelBricksLocation.Add(bd);
                    }
                    else
                    {
                        BrickData bd1, bd2;

                        if (explosionChance <= explosionBrickSpawnRate && level > gameData.levelExplodingBricks)
                        {
                            //Create exploding brick
                            bd1 = new BrickData(brickPosition, BrickData.BrickType.exploding);
                            bd2 = new BrickData(new Vector2(-brickPosition.x, brickPosition.y), BrickData.BrickType.exploding);
                        }
                        else if (metalChance >= explosionBrickSpawnRate && metalChance <= explosionBrickSpawnRate + metalBrickSpawnRate && level > gameData.levelMetalBrick)
                        {
                            //Create metal brick
                            bd1 = new BrickData(brickPosition, BrickData.BrickType.metal);
                            bd2 = new BrickData(new Vector2(-brickPosition.x, brickPosition.y), BrickData.BrickType.metal);
                        }
                        else if (strongChance > explosionBrickSpawnRate + metalBrickSpawnRate && strongChance <= explosionBrickSpawnRate + metalBrickSpawnRate + strongBrickSpawnRate && level > gameData.levelStrongBrick)
                        {
                            //Create strong brick
                            bd1 = new BrickData(brickPosition, BrickData.BrickType.stronger);
                            bd2 = new BrickData(new Vector2(-brickPosition.x, brickPosition.y), BrickData.BrickType.stronger);
                        }
                        else
                        {
                            //Create normal brick
                            bd1 = new BrickData(brickPosition, BrickData.BrickType.normal);
                            bd2 = new BrickData(new Vector2(-brickPosition.x, brickPosition.y), BrickData.BrickType.normal);
                        }

                        //Add bricks to the list
                        levelBricksLocation.Add(bd1);
                        levelBricksLocation.Add(bd2);
                    }
                }
            }
        }

        //Add brick to the list
        AddBricksToList();
    }

    void AddBricksToList()
    {
        foreach (BrickData bd in levelBricksLocation)
        {
            data.Add(JsonUtility.ToJson(bd));
        }
        data.Add("#");
    }

    public void SaveData()
    {
        //Add each element of levelBricksLocation to data list
        foreach (BrickData bd in levelBricksLocation)
        {
            data.Add(JsonUtility.ToJson(bd));
        }

        //Write all data in file
        File.WriteAllLines(Application.dataPath + "/Resources/levels.json", data.ToArray());

        Debug.Log("Data saved");
    }
}
