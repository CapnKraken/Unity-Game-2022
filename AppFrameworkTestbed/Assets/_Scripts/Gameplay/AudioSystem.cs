using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : NotifiableObj
{
    private AudioSource source;

    protected override void Initialize()
    {
        //Set up to respond to audio events
        relevantCategories.Add(Category.Audio);

        //Set up the audio source
        source = GetComponent<AudioSource>();
    }

    public override void OnNotify(Category category, string message, string senderData)
    {
        
    }


    public override string GetLoggingData()
    {
        return name;
    }
}
