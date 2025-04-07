using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{

    public Collider colA;
    public Collider colB;
    [SerializeField] private float ignoreDuration = 0.25f;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Player")) return;

    //    // Only respond to collisions with bottom trigger
    //    if (other == m_colB)
    //    {
    //        Collider playerCol = other.GetComponent<Collider>();
    //        if (playerCol != null)
    //        {
    //            StartCoroutine(TemporarilyIgnoreCollision(playerCol));
    //        }
    //    }
    //}

    public IEnumerator TemporarilyIgnoreCollision(Collider playerCol)
    {
        // Ignore collision with both colliders
        Physics.IgnoreCollision(playerCol, colA, true);
        Physics.IgnoreCollision(playerCol, colB, true);

        yield return new WaitForSeconds(ignoreDuration);

        // Re-enable collisions
        Physics.IgnoreCollision(playerCol, colA, false);
        Physics.IgnoreCollision(playerCol, colB, false);
    }

}
