using UnityEngine;
using System.Collections;

public struct Pair
{
    readonly int a;
    readonly string t1;
    readonly string t2;
    readonly string t3;
}

[System.Serializable]
public class Dialogue
{
    public string name;
    public Reply[] reply; 

    [TextArea(3,10)]
    public string[] sentences;
}
[System.Serializable]

public class Reply
{
    [TextArea(3,10)]
    public string[] replys;
    public bool[] Bloqued;
}
 
