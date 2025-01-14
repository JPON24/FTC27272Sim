using UnityEngine;
using UnityEngine.UIElements;

//this file is attached to the arm gameobject on the robot, and is used to control its movement
public class ArmLift : MonoBehaviour
{
    [Header("Configuration")] // constant values that are referenced throughout the file and assigned before runtime
    [SerializeField] float speed; //arm lift speed
    [SerializeField] float scalarJumps; //arm speed scalar step size (unused)
    [SerializeField] float topValue; // maximum value of arm rotation in degrees
    [SerializeField] float bottomValue; // minimum value of arm rotation in degrees

    [Header("Dynamic Values")]
    [SerializeField] float speedScalar; // unused speed shifter

    [Header("Cached")] // makes references to other objects
    [SerializeField] InputHandler input; //reference to input object
    GameManager gm; // reference to game manager object

    private void Awake() // called before first frame 
    {
        // set objects to references in the editor
        input = GetComponentInParent<InputHandler>();
        gm = FindAnyObjectByType<GameManager>();
    }

    void Update() // called every frame
    {
        if (gm.GetProjectState() != 'G') {return;} // if game not in game state, return, otherwise calculate arm movement

        RotateArm();
        
        if (input.GetLeftStick2Reading().y != 0) // unused
        {
            PlayArmSound();
        }
    }

    // rotates arm based on controller input
    private void RotateArm()
    {
        // float yInput = -input.GetLeftStick2Reading().y;
        float yInput = 0;
        if (input.GetRightTrigger2Reading() > 0.1f) // if right trigger pressed
        {
            yInput = 1;
        }
        else if (input.GetLeftTrigger2Reading() > 0.1f) // if left trigger pressed
        {
            yInput = -1;
        }

        if (yInput > 0.1)//down 
        {
            if (transform.localEulerAngles.x > bottomValue) // if rotation not yet at bottom value
            {
                transform.Rotate(0, speed * yInput * Time.deltaTime, 0); // rotate downward
            }
        }
        else if (yInput < -0.1)//up
        {
            if (transform.localEulerAngles.x < topValue) // if rotation not yet at top value
            { 
                transform.Rotate(0, speed * yInput * Time.deltaTime, 0); // rotate upward
            }
        }
    }

    //unused
    private void PlayArmSound()
    {

    }
}
