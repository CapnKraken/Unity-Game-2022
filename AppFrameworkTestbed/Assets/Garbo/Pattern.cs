using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pattern : MonoBehaviour
{
    //public SerializableDictionary<string, float> vars;

    //public List<Command> commands;
}
/*
[System.Serializable]
public class Command
{
    public float preDelay, postDelay;

    public BulletProperties properties;

    public int repeatCount;
    public List<Command> repeats;
}
[System.Serializable]
public class BulletProperties
{
    [Header("Bullet Spawning")]
    public float speed;
    public float angle;
    public bool isAimed;
    public int burst;
    public int spread;
    public float spreadAngle;
    public int whipCount;
    public float whipOffset;
    public float whipAngle;
}


[System.Serializable]
public struct KeyValPair<Key_Type, Value_Type>
{
    public Key_Type key;
    public Value_Type value;

    public KeyValPair(Key_Type key, Value_Type value)
    {
        this.key = key;
        this.value = value;
    }
}

[System.Serializable]
public struct SerializableDictionary<Key_Type, Value_Type>
{
    public List<KeyValPair<Key_Type, Value_Type>> dictionaryList;

    public void AddNew(Key_Type key, Value_Type value)
    {
        dictionaryList.Add(new KeyValPair<Key_Type, Value_Type>(key, value));
    }

    public Value_Type Get(Key_Type key)
    {
        foreach(KeyValPair<Key_Type, Value_Type> k in dictionaryList)
        {
            if (k.key.Equals(key))
            {
                return k.value;
            }
        }

        return default(Value_Type);
    }
}
*/
