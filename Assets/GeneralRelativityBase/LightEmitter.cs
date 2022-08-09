using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEmitter : Object
{
    public GameObject photon;
    [Range(0f, 5f)]
    public float strength;
    [Range(0.001f, 4f)]
    public float spread;
    public bool radialLock;
    Quaternion q;
    
    new void Start()
    {
        q = Quaternion.FromToRotation(-transform.position.normalized, transform.forward);
        base.Start();
        StartCoroutine(SlowUpdate());
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < Mathf.Clamp(strength * 5f, 0f, 20f); i++)
        {
            Vector3 dir = transform.rotation * (Random.insideUnitSphere + Vector3.forward * 2f / spread).normalized * Random.Range(5f, 15f);
            Gizmos.DrawRay(transform.position, dir);
        }
    }

    IEnumerator SlowUpdate()
    {
        while (true)
        {
            for (int i = 0; i < 2f / (strength * properTimeStep); i++)
            {
                yield return new WaitForEndOfFrame();

                if (radialLock)
                    transform.rotation = q * Quaternion.FromToRotation(transform.forward, -transform.position.normalized) * transform.rotation;
            }

            if (!global.paused)
            {
                Object g = Instantiate(photon.GetComponent<Object>(), spaceTimePos, transform.rotation * (Random.insideUnitSphere + Vector3.forward * 2f / spread).normalized, Quaternion.identity);
                Destroy(g.gameObject, Mathf.Clamp(Random.Range(.05f, .5f) / (strength * properTimeStep), 1f, 30f));
            }
        }
    }
}
