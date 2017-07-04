using System.Collections.Generic;
using UnityEngine;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Local

public class Game : MonoBehaviour
{

    public List<GameObject> Actions;
    public List<GameObject> Characters;

    public float ActionSpawnTime;
    public float RotationDegPerSec;
    public float ShrinkFactor;
    public float MinimumSize;
    public float InitialShrinkTime;
    public float ShrinkTime;
    public int Locations;

    private Transform panel;
    private Transform planet;
    private Transform planetSurface;
    private Vector3 shrinkBy;
    private float minSize;

    // Use this for initialization
    void Start()
    {

        planet = transform.Find("Planet");
        planetSurface = transform.Find("Planet/PlanetSurface");
        panel = GameObject.Find("Canvas/Panel").transform;

        CreatePlayers();

        shrinkBy = planetSurface.localScale * ShrinkFactor;
        minSize = (planetSurface.localScale * MinimumSize).magnitude;
        InvokeRepeating("Shrink", InitialShrinkTime, ShrinkTime);
        InvokeRepeating("SpawnAction", 0, ActionSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {

        planet.Rotate(0, 0, RotationDegPerSec * Time.deltaTime); //rotates x degrees per second around z axis
    }

    private void CreatePlayers()
    {
        var topPos = new List<Vector3>();
        var planetBounds = planetSurface.gameObject.GetComponent<Renderer>().bounds;
        foreach (var character in Characters)
        {
            var playerBounds = character.gameObject.GetComponent<Renderer>().bounds;
            var y = planetBounds.extents.y + playerBounds.extents.y + planetBounds.extents.y * ShrinkFactor * 2;
            topPos.Add(planetSurface.position + new Vector3(0, y, 0));
        }

        var anglePerLoc = 360 / Locations;

        for (int i = 0; i < Locations; i++)
        {
            var ch = Random.Range(0, Characters.Count);
            var angle = -i * anglePerLoc;
            var roration = Quaternion.AngleAxis(angle, Vector3.forward);
            var pos = RotatePointAroundPivot(topPos[ch], planetSurface.position, new Vector3(0, 0, angle));
            var obj = Instantiate(Characters[ch], pos, roration);
            obj.transform.parent = planet;
        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    void Shrink()
    {
        if (planetSurface.localScale.magnitude > minSize)
            planetSurface.localScale -= shrinkBy;
    }

    private void SpawnAction()
    {
        var i = Random.Range(0, Actions.Count);

        var obj = Instantiate(Actions[i], transform.parent);
        obj.transform.parent = panel;
    }
}
