using Unity.VisualScripting;
using UnityEngine;

public class ParticleAssets : MonoBehaviour
{
    public static ParticleAssets instance 
    {
        private set { instance = value; }
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load("ParticleAssets") as GameObject).GetComponent<ParticleAssets>();
            }
            return instance;
        }
    }

    public GameObject Smoke;
}
