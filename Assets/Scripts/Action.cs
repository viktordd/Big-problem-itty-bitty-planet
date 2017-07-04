using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Local

public class Action : MonoBehaviour
{
    public float Speed;
    public int MaxAffectedCharacters;
    public float ActiveColliderRadius;

    private new Renderer renderer;
    private int personLayer;
    private int actionEnablerLayer;
    private int actionDestroyerLayer;
    private int activeActionLayer;

    private bool mouseEnabled;
    private bool move = true;
    private bool fade;
    private float destroyDelay = 1;
    private List<ActionChar> characters = new List<ActionChar>();
    private Vector3 screenPoint;
    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<Renderer>();
        personLayer = LayerMask.NameToLayer("Character");
        actionEnablerLayer = LayerMask.NameToLayer("ActionEnabler");
        actionDestroyerLayer = LayerMask.NameToLayer("ActionDestroyer");
        activeActionLayer = LayerMask.NameToLayer("ActiveAction");

        renderer.material.color = Color.gray;
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            transform.Translate(Vector2.down * Speed * Time.deltaTime);
        }
        if (fade)
        {
            var c = renderer.material.color;
            var a = c.a - 0.75f * Time.deltaTime;
            if (a > 0)
                renderer.material.color = new Color(c.r, c.g, c.b, a);
        }
    }

    void OnMouseDown()
    {
        if (!mouseEnabled) return;

        move = false;
        GetComponent<CircleCollider2D>().radius = ActiveColliderRadius;
        gameObject.layer = activeActionLayer;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(
                     new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (!mouseEnabled) return;

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    void OnMouseUp()
    {
        if (!mouseEnabled) return;

        while (characters.Count > 0)
        {
            var character = characters[0];
            characters.RemoveAt(0);
            if (character.Affected)
                Destroy(character.Character);
        }

        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!mouseEnabled && other.gameObject.layer == actionEnablerLayer)
        {
            mouseEnabled = true;
            renderer.material.color = Color.white;
        }
        else if (move && other.gameObject.layer == actionDestroyerLayer)
        {
            mouseEnabled = false;
            fade = true;
            var c = renderer.material.color;
            renderer.material.color = new Color(c.r, c.g, c.b, 0.75f);
            Destroy(this.gameObject, destroyDelay);
        }
        else if (mouseEnabled && other.gameObject.layer == personLayer)
        {
            AddCharacter(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == personLayer)
        {
            RemoveCharacter(other.gameObject);
        }
    }

    private void AddCharacter(GameObject character)
    {
        if (characters.Any(c => c.Character == character))
            return;

        characters.Add(new ActionChar(character));
        UpdateAffectedCharacters();
    }

    private void RemoveCharacter(GameObject character)
    {
        var i = characters.FindIndex(c => c.Character == character);
        if (i < 0)
            return;

        characters[i].Renderer.material.color = Color.white;
        characters.RemoveAt(i);
        UpdateAffectedCharacters();
    }

    private void UpdateAffectedCharacters()
    {
        if (MaxAffectedCharacters <= 0)
            return;

        characters = characters.OrderBy(ch => Vector3.Distance(ch.Character.transform.position, transform.position)).ToList();

        for (int i = 0; i < characters.Count; i++)
        {
            var ch = characters[i];
            ch.Affected = i < MaxAffectedCharacters;
            ch.Renderer.material.color = ch.Affected ? Color.red : Color.white;
        }
    }
}
