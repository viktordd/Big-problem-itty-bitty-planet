using UnityEngine;

public class ActionChar
{
    public GameObject Character { get; private set; }
    public Renderer Renderer { get; private set; }
    public bool Affected { get; set; }

    public ActionChar(GameObject character)
    {
        Character = character;
        Renderer = character.GetComponent<Renderer>();
    }
}