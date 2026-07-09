using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Birdspwan : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] internal GameObject Birds;
    void Start()
    {
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    internal IEnumerator Spawn()
    {
        yield return new WaitForSeconds(8f);
        Birds.SetActive(true);
    } 

}
