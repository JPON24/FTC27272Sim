using UnityEngine;

//this file is attached to the extension gameobject on the robot, and is used to control its movement
public class Extension : MonoBehaviour
{
    [Header("Configuration")] // constant values that are referenced throughout the file and assigned before runtime
    [SerializeField] float speed; //extension move speed
    [SerializeField] float lerpSpeed; //lerp (linear interpolate) speed. currently unused (TODO)

    [Header("Dynamic Values")] // values that are referenced throughout the file and have new values assigned to them during runtime
    [SerializeField] float topValue; //top value of the extension in "encoder" ticks (full extend)
    [SerializeField] float bottomValue; //bottom value of extension in "encoder" ticks (full retract)
    [SerializeField] float m_currentTicks; //current value of extension in "encoder" ticks
 
    [Header("Cached")] // references to other files
    InputHandler input; // makes references to controller input
    GameManager gm; // manages game state
    // Rigidbody rb;

    private void Awake() // runs before the first frame
    {
        input = FindAnyObjectByType<InputHandler>(); // assign input object to the input object in the unity scene
        gm = FindAnyObjectByType<GameManager>(); // assign game manager object to the game manager object in the unity scene
        // rb = GetComponent<Rigidbody>();    
    }

    void Update() // called every frame
    {
        if (gm.GetProjectState() != 'G') {return;} //if the project is currently not in game mode, return
        Telescope(); //if the project is in game mode, calculate movement of extension
        
        if (input.GetRightStick2Reading() != Vector2.zero) //unused
        {
            PlayTelescopingSounds();
        }
    }

    private void Telescope() // moves the local position of the extension bar
    {
        Vector2 reading = input.GetRightStick2Reading(); // get the (x,y) reading of the right stick

        if (reading.y > 0.1 && m_currentTicks < topValue) // if pushing up on stick and not yet extension maximum
        {
            m_currentTicks += Time.deltaTime * 100; //increase encoder ticks independently of framerate
            transform.position -= transform.up * speed * Time.deltaTime; // move the object on the local y axis, scaled by a speed scalar and independent of framerate
            // rb.AddForce(Vector3.up * speed,ForceMode.Force);
        }
        else if (reading.y < -0.1 && m_currentTicks > bottomValue) // if pushing down on stick and not yet at extension minimum
        {
            m_currentTicks -= Time.deltaTime * 100; //decrease encoder ticks independently of framerate
            transform.position += transform.up * speed * Time.deltaTime; // move the object on the local y axis, scaled by a speed scalar and independent of framerate
            // rb.AddForce(Vector3.down * speed, ForceMode.Force);
        }
        else // unused, was implemented in old physics system
        {
            // float tempYVelocity = rb.linearVelocity.y;
            // tempYVelocity = Mathf.Lerp(rb.linearVelocity.y, 0f ,lerpSpeed);
            // rb.linearVelocity = new Vector3(0,tempYVelocity,0);
        }
    }

    private void PlayTelescopingSounds()
    {
        
    }

    public double GetCurrentTicks() //returns the current tick value of the extension encoder
    {
        return m_currentTicks;
    }
}
