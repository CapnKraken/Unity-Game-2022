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

    /// <summary>
    /// The location of the waiting area in the world.
    /// </summary>
    public Vector3 waitingArea;

    public static int sortOrder;

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

    public Dictionary<string, Queue<PoolableObject>> objectDictionary;
    public Dictionary<string, int> activeObjectCounts;

    private void Start()
    {
        sortOrder = 0;

        //Debug.Log(pools.Count);
        objectDictionary = new Dictionary<string, Queue<PoolableObject>>();
        activeObjectCounts = new Dictionary<string, int>();

        foreach(Pool pool in pools)
        {
            Queue<PoolableObject> objectQueue = new Queue<PoolableObject>();

            int i = 0;
            for(i = 0; i < pool.size; i++)
            {
                PoolableObject g = Instantiate(pool.prefab).GetComponent<PoolableObject>();
                g.SetActiveInScene(false);

                //start it in the waiting area
                g.transform.position = waitingArea;

                objectQueue.Enqueue(g);
            }


            objectDictionary.Add(pool.tag, objectQueue);
            activeObjectCounts.Add(pool.tag, 0);
        }
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

    public PoolableObject GetObjectFromPool(string tag)
    {
        if (objectDictionary[tag].Count > 0)
        {
            PoolableObject g = objectDictionary[tag].Dequeue();
            objectDictionary[tag].Enqueue(g);

            if (!g.GetActiveInScene())
            {
                activeObjectCounts[tag]++;
            }
            else
            {
                return null;
            }

            //Set it so it starts listening to the update loop
            g.excludeTick = false;
            return g;
        }
        else
        {
            return null;
        }
    }

    public void ReturnObjectToPool(string tag, PoolableObject obj)
    {
        obj.gameObject.transform.rotation = Quaternion.identity;

        if (obj.GetActiveInScene())
        {
            activeObjectCounts[tag]--;
        }

        //deactivate it
        obj.excludeTick = true;
        obj.SetActiveInScene(false);

        //put it in the waiting area location
        obj.transform.position = waitingArea;
    }

    public void ClearDictionary()
    {
        foreach(KeyValuePair<string, Queue<PoolableObject>> k in objectDictionary)
        {
            foreach(PoolableObject o in k.Value)
            {
                Destroy(o.gameObject);
            }
        }
    }
}
