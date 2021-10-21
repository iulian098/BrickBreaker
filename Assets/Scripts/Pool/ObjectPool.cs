using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public static Dictionary<string, Queue<GameObject>> poolDictionary;

    // Start is called before the first frame update
    void Start()
    {
        CreatePools();
    }

    void CreatePools()
    {

        //Create new dictionary
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        //Foreach pool spawn prefabs
        foreach(Pool p in pools)
        {
            //Create queue with objects
            Queue<GameObject> objects = new Queue<GameObject>();
            //Spawn object
            for(int i = 0; i < p.size; i++)
            {
                GameObject obj = Instantiate(p.prefab);
                obj.SetActive(false);
                objects.Enqueue(obj);
            }

            poolDictionary.Add(p.tag, objects);
        }
    }

    public static GameObject SpawnFromPool(string tag, Vector2 position)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = Quaternion.identity;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
