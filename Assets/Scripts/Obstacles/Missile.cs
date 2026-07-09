using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

/// <summary>
/// Obstacle that starts moving forward in its lane when the player is close enough.
/// </summary>
public class Missile : Obstacle
{
	static int s_DeathHash = Animator.StringToHash("Death");
	public static int s_RunHash = Animator.StringToHash("Run");
	public static int s_IdleHash = Animator.StringToHash("Idle");

	public Animator animator;
	public AudioClip[] movingSound;

	protected TrackSegment m_OwnSegement;

    protected bool m_Ready { get; set; }
	protected bool m_IsMoving;
	protected AudioSource m_Audio;

    protected const int k_LeftMostLaneIndex = -1;
    protected const int k_RightMostLaneIndex = 1;
    protected const float k_Speed = 5f;

	public void Awake()
	{
		m_Audio = GetComponent<AudioSource>();
	}

	public override IEnumerator Spawn(TrackSegment segment, float t)
	{
        int lane = Random.Range(k_LeftMostLaneIndex, k_RightMostLaneIndex + 1);

		Vector3 position;
		Quaternion rotation;
		segment.GetPointAt(t, out position, out rotation);

	    AsyncOperationHandle op = Addressables.InstantiateAsync(gameObject.name, position, rotation);
	    yield return op;
	    if (op.Result == null || !(op.Result is GameObject))
	    {
	        Debug.LogWarning(string.Format("Unable to load obstacle {0}.", gameObject.name));
	        yield break;
	    }
        GameObject obj = op.Result as GameObject;

        obj.transform.SetParent(segment.objectRoot, true);
        obj.transform.position += obj.transform.right * lane * segment.manager.laneOffset;

        obj.transform.forward = -obj.transform.forward;
	    Missile missile = obj.GetComponent<Missile>();
	    missile.m_OwnSegement = segment;

        //TODO : remove that hack related to #issue7
        Vector3 oldPos = obj.transform.position;
        obj.transform.position += Vector3.back;
        obj.transform.position = oldPos;

        missile.Setup();
    }

    public override void Setup()
    {
        m_Ready = true;
    }

    public override void Impacted()
	{
		base.Impacted();

		if (animator != null)
		{
			animator.SetBool(s_IdleHash,true);
		}
	}

	public void Update()
	{
		if (m_Ready && m_OwnSegement.manager.isMoving)
		{
			if (m_IsMoving)
			{
                transform.position += transform.forward * k_Speed * Time.deltaTime;
			}
			else
			{
				if (TrackManager.instance.segments[1] == m_OwnSegement)
				{
					if (animator != null)
					{
						animator.SetTrigger(s_RunHash);
					}

					if(m_Audio != null && movingSound != null && movingSound.Length > 0)
					{
						m_Audio.clip = movingSound[Random.Range(0, movingSound.Length)];
						m_Audio.Play();
						m_Audio.loop = true;
					}

					m_IsMoving = true;
				}
			}
		}
	}


	internal void OnTriggerEnter(Collider other)
	{
		/*if (other.gameObject.name == "ObstacleWheelyBin(Clone)")
		{
			Debug.Log("Coin Found");
			gameObject.SetActive(false);
			gameObject.transform.position = new Vector3(gameObject.transform.position.x,
				gameObject.transform.position.y + other.gameObject.transform.localScale.y, gameObject.transform.position.z);
		} 
	    
		if (other.gameObject.name == "ObstacleLowBarrier(Clone)")
		{
			gameObject.SetActive(false);
			Debug.Log("Coin Found");
		} 
	    
		if (other.gameObject.name == "ObstacleRoadworksCone(Clone)")
		{
			gameObject.SetActive(false);
			Debug.Log("Coin Found");
		}*/
	}
}
