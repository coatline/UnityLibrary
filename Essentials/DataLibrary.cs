using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class DataLibrary : Singleton<DataLibrary>
{
    public Getter<SoundType> Sounds { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Sounds = new Getter<SoundType>(Resources.LoadAll<SoundType>("Sounds"));
    }
}
