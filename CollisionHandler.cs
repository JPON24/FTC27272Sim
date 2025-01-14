using Unity.VisualScripting;
using UnityEngine;

// collision handler for scoring elements
// attached to every sample and specimen
public class CollisionHandler : MonoBehaviour
{
    [Header("State")] // state booleans
    [SerializeField] bool canScore = true; // can add to score array
    [SerializeField] bool isSpecimen = false; // is specimen object
    [SerializeField] bool isHanging = false; // is hanging (only applicable to specimen)
    [SerializeField] bool canGenerateSpecimen = true; // can turn into specimen (only for samples)

    [Header("Cached")] // object references
    ScoreHandler sh; // score object reference
    SpecimenToSample ss; // specimen conversion refernece
    Claw cs; // claw reference
    Transform initParent; // allows object to reparent after being dropped by claw  

    private void Awake() // runs before 1st frame
    {
        // set object references to clear null obj ref errors
        sh = FindAnyObjectByType<ScoreHandler>();
        ss = FindAnyObjectByType<SpecimenToSample>();
        cs = FindAnyObjectByType<Claw>();
    }

    private void Start() // runs on first frame
    {
        initParent = gameObject.transform.parent; // set init parent for reparenting
    }

    private void Update() // runs every frame
    {
        if (cs.GetOpen()) // if claw is open
        {
            gameObject.transform.parent = initParent; // reset block parents, so no blocks are grabbed
            gameObject.GetComponent<Rigidbody>().isKinematic = false; // allow physics engine to do actions on blocks
        }
        if (isHanging) // if specimen is hanging on bar
        {
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // turn off velocity (stops drift with zero gravity obj)
        }
    }

    // the frame when collision occurs
    // takes in a parameter of the object it collided with
    private void OnCollisionEnter(Collision other)
    {
        // check if hooking as specimen
        SpecimenCheck(other);

        // check if in basket as sample
        SampleCheck(other);
    }

    // checks if object is a specimen, and if it should be hung on rung
    private void SpecimenCheck(Collision other)
    {
        if (!isSpecimen) {return;} // if not specimen return
        if(!canScore){return;} // if cannot score return
        if (other.gameObject.tag == "Rung") // if colliding with a rung, can no longer add to scoring array, add to scoring array and is now hanging
        {
            Debug.Log("specimen checking");
            canScore = false;
            isHanging = true;
            sh.SetToMinimumElement(gameObject,other.gameObject); // add specimen to scoring array
        }
    }

    // checks if object is a sample, and if it should be added to scoring array
    private void SampleCheck(Collision other)
    {
        if (isSpecimen) {return;} // if is a specimen, return
        if(!canScore){return;} // if cannot score return
        if (other.gameObject.tag == "Basket") // if colliding with a basket, can no longer add to scoring array and add to scoring array
        {
            canScore = false;
            sh.SetToMinimumElement(gameObject);
        }
    }

    // the frame when two colliders are no longer touching
    // takes in parameter of other object
    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.tag == "Rung") // if leaving contact with rung, no longer hanging and can score and reset from array
        {
            isHanging = false;
            canScore = true;
            sh.ResetElement(new GameObject(),gameObject); // revert to empty 
        }
        else if (other.gameObject.tag == "Basket") // if leaving contact with basket can score and reset from array
        {
            canScore = true;
            sh.ResetElement(new GameObject(),gameObject); // revert to empty
        }
    }

    // the frame when a trigger touches another
    // used for claw grabbing and observation zone changes
    // takes in parameter of other object touched
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Observation" && canGenerateSpecimen && !isSpecimen) //if inside the observation zone, has not started generating and is not a specimen
        {
            ss.ConvertToSpecimen(gameObject); // convert object to specimen
            canGenerateSpecimen = false; // has started generating
        }    
        else if (other.gameObject.tag == "Claw" && !cs.GetOpen() && Time.time - cs.GetCloseTimestamp() < 0.2f) // if connecting with the claw, the claw is closed and the claw closed recently
        {
            gameObject.transform.parent = other.gameObject.transform; // parent object to claw (essentially a grab)
            gameObject.GetComponent<Rigidbody>().isKinematic = true; // stop making physics interactions with the object
        }
    }

    // the frame when a trigger leaves another
    // used for observation zone
    // takes in parameter of other trigger
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Observation") // if leaving observation zone
        {
            ss.CancelInstantiate(); // stop object from changing to specimen
            canGenerateSpecimen = true;
        }    
        // else if (other.gameObject.tag == "Claw" && cs.GetOpen()) // no timestamp needed for open
        // {
        //     // ?
        // }
    }

    // getter method used in other files
    public bool GetIsSpecimen()
    {
        return isSpecimen;
    }
}
