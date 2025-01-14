using UnityEngine;

//this file is attached to the parent of the claw object in unity, and is used to control the rotation of both claw heads.
public class Claw : MonoBehaviour
{
    [Header("Configuration")] // constant values that are referenced throughout the file and assigned before runtime
    [SerializeField] GameObject servoClawOrigin; //reference to parent of the servo claw head
    [SerializeField] GameObject bearingClawOrigin; //reference to parent of the bearing claw head
    [SerializeField] float clawDelay; //delay before claw state can be changed
    [SerializeField] float speed; // unused, will be used with new physics simulation to simulate claw movement
    [SerializeField] float maxRot; // max position of claw in degrees

    [Header("Dynamic Values")] // values that are referenced throughout the file and have new values assigned to them during runtime
    [SerializeField] bool m_open; //the current state of the claw
    [SerializeField] bool canOpen; //whether or not the claw can open
    [SerializeField] float closeTimestamp = 0; //timestamp of the claw's last closure, used when determining delay

    [Header("Cached")] // references to other files
    InputHandler input; // makes references to controller input
    GameManager gm; // manages game state

    private void Awake() // runs before the first frame
    {
        input = FindAnyObjectByType<InputHandler>(); // assign input object to the input object in the unity scene
        gm = FindAnyObjectByType<GameManager>(); // assign game manager object to the game manager object in the unity scene
    }

    private void Update() 
    {
        if (gm.GetProjectState() != 'G') {return;} //if the project is currently not in game mode, return
        MoveClaw(); //if the project is in game mode, calculate movement of claw
    }

    private void MoveClaw() //alters the rotation of the claw by rotating them to presets
    {
        float openClaw = input.GetXButton2Reading(); //get the reading of the x button on the controller
        float closeClaw = input.GetAButton2Reading(); //get the reading of the a button on the controller

        if (m_open && closeClaw > 0.1 && canOpen) //if close input and can open the claw (better name would be can move) and is open
        {
            canOpen = false; //no longer allow controller input
            Invoke("ResetCanOpen",clawDelay); //allow controller input again on a delay
            m_open = false; //claw is no longer open
            servoClawOrigin.transform.Rotate(0, maxRot, 0); //rotate servo claw to close position
            bearingClawOrigin.transform.Rotate(0, -maxRot, 0); //rotate bearing claw to close position
            PlayClawSound(); //unused
            closeTimestamp = Time.time;//set closing timestamp
        }
        else if (!m_open && openClaw > 0.1 && canOpen) //if open input and can open the claw (better name would be can move) and is not open
        {
            canOpen = false; // no longer allow controlelr input
            Invoke("ResetCanOpen", clawDelay); //allow controller inpujt again on a delay
            m_open = true; //claw is now open
            servoClawOrigin.transform.Rotate(0, -maxRot, 0); //rotate servo claw to open position
            bearingClawOrigin.transform.Rotate(0, maxRot, 0); //rotate bearing claw to open position
            PlayClawSound();//unused
        }
    }

    private void PlayClawSound()
    {

    }

    // allows controller input again after a delay
    private void ResetCanOpen()
    {
        canOpen = true;
    }

    //used in other files to check if the claw is open or not (collision handler)
    public bool GetOpen()
    {
        return m_open;
    }

    //used in other files to check the timestamp of the claw closure
    public float GetCloseTimestamp()
    {
        return closeTimestamp;
    }
}
