using UnityEngine;

public class playerController : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    // variabelen declareren
    public float speed = 5.0f;
    public float turnspeed;

    public float horizontalInput;
    public float forwardInput;

    void Update()
    {
        // move vehicle forward
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");
        //deze code beweegt de auto naar voor als ik op de horizontale knop duw
        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        //deze code draait de auto als ik op de verticale knop duw
        transform.Rotate(Vector3.up, turnspeed * Time.deltaTime * horizontalInput); 
    }
}
