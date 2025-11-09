using UnityEngine;
using System.Collections.Generic;

public class PlayerPickup : MonoBehaviour
{
    [Header("Setup")]
    public Transform holdPoint;           // Waar het object vastgehouden wordt
    public KeyCode pickupKey = KeyCode.E; // Toets om op te pakken/los te laten
    public KeyCode throwKey = KeyCode.Mouse0; // Toets om te gooien (linkermuisknop)
    public bool isCarrying = false; //voor sprint

    [Header("Throw Settings")]
    public float throwForce = 10f;        // Kracht bij gooien

    private GameObject heldObject = null;
    private Rigidbody heldRb = null;
    private HashSet<GameObject> nearbyObjects = new HashSet<GameObject>();

    void Update()
    {
        // Oppakken of neerleggen
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldObject == null)
            {
                PickupNearest();

            }
                
            else
            {
                DropHeld();

            }

        }

        // Gooien
        if (Input.GetKeyDown(throwKey) && heldObject != null)
        {
            ThrowHeld();

        }

                

        // Houd object netjes op holdPoint
        if (heldObject != null)
        {
            heldObject.transform.position = holdPoint.position;
            heldObject.transform.rotation = holdPoint.rotation;

        }
    }

    // Zoek dichtstbijzijnde object in trigger
    void PickupNearest()
    {
        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (var obj in nearbyObjects)
        {
            if (obj == null) continue;
            float dist = Vector3.Distance(obj.transform.position, holdPoint.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = obj;
            }
        }

        if (nearest != null)
        {
            Pickup(nearest);
            isCarrying = true;
        }

        void Pickup(GameObject obj)
        {
            heldObject = obj;
            heldRb = obj.GetComponent<Rigidbody>();

            if (heldRb != null)
            {
                heldRb.isKinematic = true;      // Physics uit
                heldRb.detectCollisions = false; // botsingen uit
            }

            obj.transform.SetParent(holdPoint);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }
    }

    public void DropHeld()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);

        if (heldRb != null)
        {
            heldRb.isKinematic = false;
            heldRb.detectCollisions = true;
        }

        heldObject = null;
        heldRb = null;
        isCarrying = false;
    }

    void ThrowHeld()
    {
        if (heldObject == null || heldRb == null) return;

        heldObject.transform.SetParent(null);
        heldRb.isKinematic = false;
        heldRb.detectCollisions = true;

        // Gooirichting: vooruit + een beetje omhoog
        Vector3 throwDir = (transform.forward + Vector3.up * 0.2f).normalized;
        heldRb.AddForce(throwDir * throwForce, ForceMode.Impulse);

        heldObject = null;
        heldRb = null;
        isCarrying = false;
    }

    // Detecteer objecten in de buurt
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickupable") || other.CompareTag("SpawnedPickup"))
            nearbyObjects.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickupable") || other.CompareTag("SpawnedPickup"))
            nearbyObjects.Remove(other.gameObject);
    }
}


