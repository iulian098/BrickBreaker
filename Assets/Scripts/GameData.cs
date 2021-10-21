using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    public int[] difficulties;
    [Header("Shop")]
    public List<BallShopClass> balls = new List<BallShopClass>();
    public List<BallShopClass> paddles = new List<BallShopClass>();

    public Dictionary<string, BallShopClass> ballsDictionary = new Dictionary<string, BallShopClass>();

    [Header("Brick skins")]
    public List<BricksSkins> brickSkins = new List<BricksSkins>();

    [Header("Prefabs")]
    public GameObject ballButtonPrefab;
    public GameObject paddleButtonPrefab;
    public GameObject powerUpPrefab;
    public GameObject coinPrefab;
    public GameObject ballHitEffect;

    [Header("Bricks prefabs")]

    public GameObject brickParticles;
    public GameObject brickExplosionParticles;
    [Space]
    public GameObject brickPrefab;
    public GameObject explodingBrickPrefab;
    public GameObject metalBrickPrefab;
    public GameObject strongerBrickPrefab;

    [Header("Data")]
    public Material defaultBallMaterial;

    public int selectedBall;
    public int selectedPaddle;
    public int selectedBrick;

    public int inputType;

    [Header("PowerUps data")]
    public float timePowerupSpawn = 5f;
    public float PowerUpDuration = 15f;
    public float paddleSpeed = 75;

    public int coins;

    [Header("Levels data")]
    public int totalLevels;
    public int levelNumber;

    [Header("Spawn chances")]
    public float bricksSpawnChance;
    public int levelExplodingBricks;
    public float explodingBricksSpawnChance;
    public int levelMetalBrick;
    public float metalBrickSpawnChance;
    public int levelStrongBrick;
    public float strongBrickSpawnChance;
    [Space]
    public float spawnPowerUpChance;
    public float spawnCoinsChance;

    public List<List<BrickData>> levels = new List<List<BrickData>>();

    public bool testing;
    public List<BrickData> testingLevel = new List<BrickData>();
    [Header("Audio")]
    public AudioClip buttonClickAudio;

    public AudioClip powerUpPickupAudio;
    public AudioClip coinPickupAudio;
    public AudioClip winAudio;
    public AudioClip loseAudio;
    public AudioClip hitAudio;
    public AudioClip brickDestroyAudio;
    public AudioClip brickExplosionAudio;

    public AudioClip[] music;

    [Header("Colors")]
    public Color[] brickColors;


    [Header("Android gameObjects")]
    public GameObject[] androidObjects;
}
