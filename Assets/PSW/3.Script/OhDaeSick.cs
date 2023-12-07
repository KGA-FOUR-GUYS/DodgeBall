using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OhDaeSick : NetworkBehaviour
{
    public GameObject Model;
    public float destroyAfter = 2f;
    public Rigidbody rb;
    public float shootForce = 500f;
    private float rotSpeed = 0f;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    private void Start()
    {
        rb.AddForce(transform.forward * shootForce);
    }
    private void FixedUpdate()
    {
        rotSpeed += 30f;
        Model.transform.Rotate(new Vector3(rotSpeed, 0, 0));
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider col) => DestroySelf();
}
