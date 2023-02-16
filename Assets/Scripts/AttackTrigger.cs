using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {
    [SerializeField] private float backwardForce = 5;

    private void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag($"Enemy") )
        {
            Debug.Log("Trigger Enter: " + other.tag);
            var rigidbody = other.GetComponent<Rigidbody>();
            if ( rigidbody != null )
            {
                rigidbody.AddForce(rigidbody.transform.forward * -backwardForce, ForceMode.Impulse);
            }
        }
    }
}