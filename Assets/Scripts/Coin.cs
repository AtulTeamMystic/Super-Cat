using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
	static public Pooler coinPool;
    public bool isPremium = false;


    private void OnTriggerEnter(Collider other)
    {
	    if (other.gameObject.name == "ObstacleWheelyBin(Clone)")
	    {
		    gameObject.SetActive(false);
	    } 
	    
	    if (other.gameObject.name == "ObstacleLowBarrier(Clone)")
	    {
		    gameObject.SetActive(false);
	    } 
	    
	    if (other.gameObject.name == "ObstacleRoadworksCone(Clone)")
	    {
		    gameObject.SetActive(false);
	    }
    }
}
