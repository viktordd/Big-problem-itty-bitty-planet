using UnityEngine;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Local

public class Character : MonoBehaviour
{
    public float GravityMagnitude;

    private Transform planet;
    private ConstantForce2D gravityForce;

    // Use this for initialization
    void Start()
    {
        planet = transform.parent.Find("PlanetSurface");
        gravityForce = GetComponent<ConstantForce2D>();
    }

    void FixedUpdate()
    {
        var direction = ((Vector2)planet.position - (Vector2)transform.position).normalized;

        gravityForce.force = direction * GravityMagnitude;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
