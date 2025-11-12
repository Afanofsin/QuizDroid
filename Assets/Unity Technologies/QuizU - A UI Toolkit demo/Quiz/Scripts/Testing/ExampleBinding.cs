using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExampleBinding", menuName = "Scriptable Objects/ExampleBinding")]
public class ExampleBinding : ScriptableObject
{
    public List<TestItem> items = new();
}

[System.Serializable]
public sealed class TestItem
{
    public string name;
    public bool enabled;
}
