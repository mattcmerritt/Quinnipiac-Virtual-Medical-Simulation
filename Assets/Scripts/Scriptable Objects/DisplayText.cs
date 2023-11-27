using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DisplayText : ScriptableObject
{
    [TextArea(5, 10)]
    public string Text;
}
