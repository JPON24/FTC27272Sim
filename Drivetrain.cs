using System;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

// this file is placed on the parent object of the robot, with the purpose of moving the robot around the field
public class Drivetrain : MonoBehaviour
{
    // [Header("Ref")]
    // [SerializeField] GameObject FL; 

    [Header("Configuration")] //constant values assigned before runtime (some organization is bad)
    [SerializeField] float scalarJumps; // used in speed shifting, indicating the magnitude of each speed jump

    [SerializeField] float speed; // current speed (should be under dynamic)
    [SerializeField] float maxSpeed; // hardcoded max speed
    [SerializeField] float posLerp; // linear interpolate speed between current speed and maximum speed

    [SerializeField] float rotationSpeed; // current rotation speed (should be under dynamic)
    [SerializeField] float maxRotationSpeed; // hardcoded max rotation speed
    [SerializeField] float rotLerp; // linear interpolate speed between current rotation speed and maximum rotation speed

    [Header("Dynamic Values")] // values that change during runtime
    [SerializeField] Vector3 tempLinearVelocity; // stores (x,y,z) of translation velocity 
    [SerializeField] float tempAngularVelocity; // stores the final rotational movement to be passed into rotation method
    [SerializeField] bool canShift; // can speed shift
    [SerializeField] float speedScalar; // current gear : altered by gear shift also known as speed shift
    [SerializeField] Quaternion angleTarget; // currently unused? (old rotation system)
    [SerializeField] float angleLerpSpeed; // currently unused (old rotation system)

    [Header("Cached")] // makes references to other files
    [SerializeField] Rigidbody rb; //rigidbody object on robot
    [SerializeField] InputHandler input; //input handler object
    [SerializeField] GameManager gm; // game manager object
    [SerializeField] CharacterController ch; //character controller object on robot

    private void Awake() //runs before 1st frame
    {
        //set each object reference to their respective object to allow methods to be referenced
        rb = GetComponent<Rigidbody>();
        input = GetComponent<InputHandler>();
        ch = GetComponent<CharacterController>();
        gm = FindAnyObjectByType<GameManager>();
    } 

    private void Start() //run on first frame, currently unimplemented
    {
        angleTarget = transform.rotation;    
    }

    private void Update() //run every frame
    {
        // if game state is not currently in game mode, return, otherwise translate, rotate and speed shift the robot according to input
        if (gm.GetProjectState() != 'G') {return;} 
        SetLinearVelocity(); //translate 
        SetAngularVelocity(); //rotate
        SpeedShift(); //speed shift
        
        if (input.GetLeftStick1Reading() != Vector2.zero) //unimplemented
        {
            PlayDriveSound();
        }
    }

    //translates robot based on input from controller
    private void SetLinearVelocity()
    {   
        float xread = input.GetLeftStick1Reading().x;
        float yread = input.GetLeftStick1Reading().y;

        // normalized value of the magnitudes of both the x and y reading
        float inputmix = (Mathf.Abs(xread) + Mathf.Abs(yread)) / 2;

        // interpolated current speed based on speed and max amount * input mix
        // input mix term included for fine motor movement

        speed = Mathf.Lerp(speed,maxSpeed * inputmix,posLerp);

        // add polarity, add frame independence 
        tempLinearVelocity.x = xread * speed * speedScalar * Time.deltaTime; //forward
        tempLinearVelocity.z = yread * speed * speedScalar * Time.deltaTime; //right
        rb.linearVelocity = Vector3.zero;
        tempLinearVelocity.y = -9.81f; //gravity

        Vector3 moveVector;
        
        if (gm.GetCameraPosition() != 'T') moveVector = transform.TransformDirection(tempLinearVelocity); // if not top down use robot centric
        else moveVector = new Vector3(tempLinearVelocity.z,tempLinearVelocity.y,-tempLinearVelocity.x); // if top down use field centric

        // apply movement with character controller component
        ch.Move(moveVector);
    }

    // rotate robot based on input from controller
    private void SetAngularVelocity()
    {
        float xread = input.GetRightStick1Reading().x;

        // linear interpolation between rotation speed and maxrotationspeed
        // uses math.abs(xread) term for fine motor movement
        rotationSpeed = Mathf.Lerp(rotationSpeed,maxRotationSpeed * Math.Abs(xread),rotLerp);   
        tempAngularVelocity = xread * rotationSpeed * Time.deltaTime;


        //update local euler angles (x,y,z in local degrees) by the temp variable
        transform.localEulerAngles += new Vector3(0,tempAngularVelocity,0);
        rb.angularVelocity = Vector3.zero; // because movement is handled locally, no physics should act upon object's angular velocity.
    }

    // speed shift based on controller input
    private void SpeedShift()
    {
        // if right bumper pressed
        if (input.GetRightBumper1Reading() > 0.1)
        {
            // if button has not yet been clicked and released, and value will not exceed maximum
            if (canShift && speedScalar+scalarJumps <= 1.0f)
            {
                speedScalar += scalarJumps; //increase speed
                canShift = false;
            }
        }
        else if (input.GetLeftBumper1Reading() > 0.1f)
        {
            // if button has not yet been clicked and released, and value will not be <= 0
            if (canShift && speedScalar - scalarJumps > 0f)
            {
                speedScalar -= scalarJumps; //decrease speed
                canShift = false;
            }
        }
        else // if both bumpers released
        {
            // release button
            canShift = true;
        }
    }

    // unused
    private void PlayDriveSound()
    {

    }

    //currently broken actually, meant to stop robot when game ends
    //would be fixed by overwriting the controller input (movement is not currently velocity based)
    public void ResetVelocity()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}