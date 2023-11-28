using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Implement proper secret management
// Temporary solution to protect the database connection strings.

[CreateAssetMenu(fileName = "New Secret")]
public class Secret : ScriptableObject
{
    public string Content;
}
