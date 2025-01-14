using System.Collections;
using UnityEngine;

// specialized class for converting blocks within observation zone into specimens
public class SpecimenToSample : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] GameObject specimenPrefab; // prefabricated specimen object 
    [SerializeField] int conversionTime; // time to wait before converting


    private GameObject tempSample; // temp object to be converted

    // begin countdown
    public void ConvertToSpecimen(GameObject sample)
    {
        tempSample = sample; // set temp object
        StartCoroutine(InstantiateSpecimen()); // start countdown
    }

    // cancel countdown, cancelling the swap
    public void CancelInstantiate()
    {
        StopAllCoroutines();
    }

    //waits, generates specimen and then destroys temp block 
    private IEnumerator InstantiateSpecimen()
    {
        yield return new WaitForSeconds(conversionTime); //wait
        Instantiate(specimenPrefab,tempSample.transform.position, tempSample.transform.rotation); //generate prefab specimen and temp sample position and rotation
        Destroy(tempSample); //destroy temp sample
    }
}
