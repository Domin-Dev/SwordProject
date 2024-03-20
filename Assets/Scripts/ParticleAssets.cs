using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleAssets : MonoBehaviour
{

    private static ParticleAssets i;

    public static ParticleAssets instance 
    {
        get
        {
            if (i == null)
            {
                i = Instantiate(Resources.Load("ParticleAssets") as GameObject).GetComponent<ParticleAssets>();
            }
            return i;
        }
    }

    public GameObject smoke;
    public GameObject leaves;
}
