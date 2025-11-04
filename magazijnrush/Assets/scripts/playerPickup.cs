using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Setup")]
    public Transform holdPoint;           // Waar het opgepakte object vastgehouden wordt
    public KeyCode pickupKey = KeyCode.E; // Toets om op te pakken/los te laten
    public KeyCode throwKey = KeyCode.Mouse0; // Toets om te gooien (linkermuisknop)

    [Header("Throw Settings")]
    public float throwForce = 10f; // Hoe hard het object wordt gegooid

    // intern
    private GameObject heldObject = null;
    private Rigidbody heldRb = null;
    private HashSet<GameObject> nearby = new HashSet<GameObject>();

    void Update()
    {
        // Oppakken of neerleggen
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldObject == null)
                TryPickupNearest();
            else
                DropHeld();
        }

        // Gooien
        if (Input.GetKeyDown(throwKey) && heldObject != null)
        {
            ThrowHeld();
        }
    }

    // Zoek dichtstbijzijnde oppakbaar object
    void TryPickupNearest()
    {
        GameObject nearest = null;
        float bestDist = float.MaxValue;

        foreach (var go in nearby)
        {
            if (go == null) continue;
            float d = Vector3.Distance(go.transform.position, holdPoint.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = go;
            }
        }

        if (nearest != null)
            Pickup(nearest);
        else
            Debug.Log("Geen oppakbaar object in bereik.");
    }

    // Object oppakken
    void Pickup(GameObject obj)
    {
        heldObject = obj;
        heldRb = obj.GetComponent<Rigidbody>();

        if (heldRb != null)
        {
            heldRb.isKinematic = true;
            heldRb.detectCollisions = false;
        }

        obj.transform.SetParent(holdPoint, true);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        Debug.Log("Oppakken: " + obj.name);
    }

    // Object neerleggen (zonder kracht)
    void DropHeld()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null, true);

        if (heldRb != null)
        {
            heldRb.isKinematic = false;
            heldRb.detectCollisions = true;

            // geef dezelfde snelheid als speler mee
            heldRb.linearVelocity = GetComponent<Rigidbody>() != null
                ? GetComponent<Rigidbody>().linearVelocity
                : Vector3.zero;
        }

        Debug.Log("Losgelaten: " + heldObject.name);
        heldObject = null;
        heldRb = null;
    }

    // Object weggooien
    void ThrowHeld()
    {
        if (heldObject == null || heldRb == null) return;

        // Eerst physics herstellen en losmaken
        heldObject.transform.SetParent(null, true);
        heldRb.isKinematic = false;
        heldRb.detectCollisions = true;

        // Gooirichting gebaseerd op speler in plaats van camera
        Vector3 throwDirection = (transform.forward + Vector3.up * 0.2f).normalized;

        // Voeg kracht toe in die richting
        heldRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

        Debug.Log("Weggegooid: " + heldObject.name);

        // Clear variabelen
        heldObject = null;
        heldRb = null;
    }

    // Trigger-detectie
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickupable"))
        {
            nearby.Add(other.gameObject);
            Debug.Log("In bereik: " + other.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickupable"))
        {
            nearby.Remove(other.gameObject);
            Debug.Log("Uit bereik: " + other.name);
        }
    }
}
