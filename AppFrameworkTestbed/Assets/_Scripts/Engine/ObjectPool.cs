using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPool : MonoBehaviour
{
    /*
     * Pool of bullets
     * List of available, list of in use
     * -whenever a bulletspawner shoots, it grabs one from available, sets it to active, and puts it in "in use"
     * -when the bullet is destroyed, put it back in "available" and set it to inactive
     */


    //public static int bulletCount;

    public GameObject[] specialAttackPrefabs;

    public static int sortOrder;

    public Text text;

    /// <summary>
    /// Object pool.
    /// </summary>
    [System.Serializable]
    public class Pool
    {
        /// <summary>
        /// The tag of the pooled objects.
        /// </summary>
        public string tag;

        /// <summary>
        /// The prefab of the object.
        /// </summary>
        public GameObject prefab;

        /// <summary>
        /// How many objects are in the pool.
        /// </summary>
        public int size;
    }

    /// <summary>
    /// The list of object pools.
    /// </summary>
    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> objectDictionary;
    public Dictionary<string, int> activeObjectCounts;

    private void Start()
    {
        sortOrder = 0;

        //Debug.Log(pools.Count);
        objectDictionary = new Dictionary<string, Queue<GameObject>>();
        activeObjectCounts = new Dictionary<string, int>();

        foreach(Pool pool in pools)
        {
            
            //Debug.Log(pool.tag);
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            int i = 0;
            for(i = 0; i < pool.size; i++)
            {
                GameObject g = Instantiate(pool.prefab);
                g.SetActive(false);

                objectQueue.Enqueue(g);
            }
            //Debug.Log(i);
            //Debug.Log($"{pool.tag} : {objectQueue.Count}");

            objectDictionary.Add(pool.tag, objectQueue);
            activeObjectCounts.Add(pool.tag, 0);
        }

        //Debug.Log(bulletDictionary);
    }


    private string DictionaryToString(Dictionary<string, int> d)
    {
        string s = "";
        foreach(KeyValuePair<string, int> k in d)
        {
            s += $"\n{k.Key}: {k.Value}";
        }
        return s;
    }

    public GameObject GetObjectFromPool(string tag)
    {
        if (objectDictionary[tag].Count > 0)
        {
            GameObject g = objectDictionary[tag].Dequeue();
            objectDictionary[tag].Enqueue(g);

            if (!g.activeSelf)
            {
                activeObjectCounts[tag]++;
            }
            else
            {
                return null;
            }

            return g;//.GetComponent<Bullet>();
        }
        else
        {
            //Debug.Log("There is no bullet with that tag: " + tag);
            return null;
        }
    }

    public void ReturnObjectToPool(string tag, GameObject obj)
    {
        obj.gameObject.transform.rotation = Quaternion.identity;

        if (obj.activeSelf)
        {
            activeObjectCounts[tag]--;
        }

        obj.gameObject.SetActive(false);

        
        //bulletDictionary[tag].Enqueue(bullet);
    }

    public void ClearDictionary()
    {
        foreach(KeyValuePair<string, Queue<GameObject>> k in objectDictionary)
        {
            foreach(GameObject o in k.Value)
            {
                Destroy(o.gameObject);
            }
        }
    }
}
