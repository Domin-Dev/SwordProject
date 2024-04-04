using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public static Sounds instance;
    private AudioSource audioSource;

    [SerializeField] List<AudioClip> swordSounds;
    [SerializeField] List<AudioClip> hitSounds;
    [SerializeField] List<AudioClip> ShieldSounds;

    private void Awake()
    {
        if(instance == null )
        {

            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Sword()
    {
        audioSource.PlayOneShot(swordSounds[Random.Range(0,2)]);
    }

    public void Hit()
    {
        audioSource.PlayOneShot(hitSounds[Random.Range(0, 1)]);
    }

    public void Shield() 
    {
        audioSource.PlayOneShot(ShieldSounds[Random.Range(0, 1)]);
    }
}

