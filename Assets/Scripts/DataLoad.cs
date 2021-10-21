using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataLoad : MonoBehaviour
{
    string fileDataName = "Data.csv";
    GameData data;
    void Start()
    {
        data = Resources.Load("GameData") as GameData;
        LoadData();
    }

    void LoadData()
    {
        string dataPath = Application.dataPath + "/" + fileDataName;
        
        string[] loadedData = { };
        List<string> values = new List<string>();
        try
        {
            loadedData = File.ReadAllLines(dataPath);
        }
        catch(IOException ex)
        {
            Debug.LogError(ex.Message);
            CreateFile();
        }

        if(loadedData.Length > 0)
        {
            foreach(string s in loadedData)
            {
                string[] dataValues = s.Split(',');
                values.Add(dataValues[1]);
            }

            AssignData(values);
        }
    }

    void AssignData(List<string> _data)
    {
        try
        {
            data.PowerUpDuration = (float)Convert.ToDouble(_data[0]);
            data.difficulties[0] = Convert.ToInt32(_data[1]);
            data.difficulties[1] = Convert.ToInt32(_data[2]);
            data.difficulties[2] = Convert.ToInt32(_data[3]);
            data.paddleSpeed = (float)Convert.ToDecimal(_data[4]);
            data.bricksSpawnChance = (float)Convert.ToDecimal(_data[5]);
            data.explodingBricksSpawnChance = (float)Convert.ToDecimal(_data[6]);
            data.spawnPowerUpChance = (float)Convert.ToDecimal(_data[7]);
            data.spawnCoinsChance = (float)Convert.ToDecimal(_data[8]);
        }
        catch
        {
            Debug.LogError("Some data are missing! Creating new file.");
            CreateFile();
        }
    }

    void CreateFile(){
        string dataPath = Application.dataPath + "/" + fileDataName;

        List<string> dataToSave = new List<string>();

        dataToSave.Add("PowerUpDuration," + data.PowerUpDuration);
        dataToSave.Add("EasyPaddleSpeed," + data.difficulties[0]);
        dataToSave.Add("MediumPaddleSpeed," + data.difficulties[1]);
        dataToSave.Add("HardPaddleSpeed," + data.difficulties[2]);
        dataToSave.Add("PaddleSpeed," + data.paddleSpeed);
        dataToSave.Add("SpawnBrickChance," + data.bricksSpawnChance);
        dataToSave.Add("ExplodingBrickSpawnChance," + data.explodingBricksSpawnChance);
        dataToSave.Add("SpawnPowerUpChance," + data.spawnPowerUpChance);
        dataToSave.Add("SpawnCoinChance," + data.spawnCoinsChance);

        try
        {
            File.WriteAllLines(dataPath, dataToSave.ToArray());

        }
        catch (IOException ex)
        {
            Debug.LogError(ex.Message);
        }

    }
}
