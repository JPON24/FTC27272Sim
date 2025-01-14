using Microsoft.MixedReality.Toolkit;
using Unity.VisualScripting;
using UnityEngine;

// handles score logic for baskets and rungs
// on game manager object in unity (separate from GameManager.cs, but as a component on the same object)
public class ScoreHandler : MonoBehaviour
{
    [SerializeField] int score = 0; // current score
    [SerializeField] GameObject[] scoringElements = new GameObject[20]; // array to store all objects

    private void Start() // run on first frame
    {
        GenerateScoringElements();
    }

    void Update()
    {
        score = CalculateScore(); // calcualte score based on array
    }

    // generates temp objects to avoid null object references (null checks do not work as unity sends errors anyway)
    private void GenerateScoringElements()
    {
        for (int i = 0; i < scoringElements.Length;i++) // for every location in scoring elements
        {
            scoringElements[i] = new GameObject(); // make an empty gameobject to fill that position
        }
    }

    //getter method used in other scripts for the score
    public int GetScore()
    {
        return score;
    }

    // called in other scripts to assign the lowest possible index to a new scoring object
    // takes in parameter of an object (scoring element)
    // sets lowest possible empty object in array to temp object
    public void SetToMinimumElement(GameObject temp)
    {
        for (int i = 0; i < scoringElements.Length; i++) 
        {
            if (scoringElements[i].tag != "Scoring") // if not a scoring element
            {
                Destroy(scoringElements[i]); // destroy empty
                scoringElements[i] = temp; // replace with temp
                break;
            }
        }
    }

    // overload method of set to minimum element but for specimens
    // adds additional functionality for disabling specimen physics on hook
    public void SetToMinimumElement(GameObject temp, GameObject rung)
    {
        for (int i = 0; i < scoringElements.Length; i++)
        {
            if (scoringElements[i].tag != "Scoring") // if not a scoring element
            {
                Destroy(scoringElements[i]); // destroy empty
                scoringElements[i] = temp; // replace with temp
                temp.transform.parent = rung.transform; // set parent of specimen to the rung
                temp.GetComponent<Rigidbody>().useGravity = false; // turn off specimen gravity
                //turn off velocity of specimen
                temp.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                temp.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                break;
            }
        }
    }

    // will refill array when a scoring object leaves the array
    public void ResetElement(GameObject temp, GameObject reference)
    {
        for (int i = 0; i < scoringElements.Length; i++)
        {
            if (scoringElements[i] == reference) // if at currently lost position
            {
                scoringElements[i] = temp; // set that position in the array to a new empty (temp is generally an empty)
                break;
            }
        }
    }

    // calculate score based on array
    // takes in scoring elements
    // gets sum of all scores
    // returns temp sum
    private int CalculateScore()
    {
        int temp = 0;
        for (int i = 0; i < scoringElements.Length; i++) // for every scoring element
        {
            if (scoringElements[i].GetComponent<CollisionHandler>() == null) {continue;} // if not a scoring element shuffle to next element

            if (scoringElements[i].GetComponent<CollisionHandler>().GetIsSpecimen())
            {
                temp += 10; 
            }
            else if (!scoringElements[i].GetComponent<CollisionHandler>().GetIsSpecimen())
            {
                temp += 4;
            }
            else // unnecessary? yes.
            {
                continue;
            }
        }

        return temp;
    }
}
