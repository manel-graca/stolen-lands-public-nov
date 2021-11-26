using System;
using StolenLands.Cinematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPassage : MonoBehaviour
{
    public float power = 50f;
    public float upForce = 20f;
    public float explosionRadius = 30f;
    public bool hasDetonated = false;
    public float timeToWaitBeforeDestroy = 5f;


    public GameObject bombDetonationPosition;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;

    private void Start()
    {
	    hasDetonated = false;
    }

    void Update()
    {
	    if (Input.GetKeyDown(KeyCode.G))
	    {
		    Detonate();
	    }
	    if (hasDetonated)
        {
            StayStatic();
            DestroyRock();
        }
    }

    public void Detonate()
	{
        Collider[] col = Physics.OverlapSphere(transform.position, explosionRadius);
        hasDetonated = true;
        //audioSource.PlayOneShot(explosionSound);
        foreach(Collider hit in col)
		{
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb!=null)
			{
                if(rb.gameObject.tag == "Player")
				{
                    return;
				}
                
                rb.isKinematic = false;
                rb.AddExplosionForce(power, bombDetonationPosition.transform.position, explosionRadius, upForce, ForceMode.Impulse);
            }
		}
	}

    void DestroyRock()
	{
        foreach(Transform rock in transform)
		{
            GameObject obj = rock.transform.gameObject;
            if(obj != null)
			{
                Destroy(gameObject, timeToWaitBeforeDestroy);
			}
		}
	}

    void StayStatic()
	{
        foreach(Transform rock in transform)
		{
            Rigidbody rb = rock.GetComponent<Rigidbody>();
            if(rb!=null)
			{
                if(rb.velocity.magnitude <= 0.1f)
				{
                    //rb.isKinematic = true;
                    rb.mass = 10000;
				}
				else
				{
                    return;
				}
			}
		}
	}
}
