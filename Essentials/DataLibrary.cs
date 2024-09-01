using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class DataLibrary : Singleton<DataLibrary>
{
    public Getter<PartType> Parts { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Parts = new Getter<PartType>(Resources.LoadAll<PartType>("Parts"));
    }
}
