using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAssets : MonoBehaviour
{
    private static UIAssets i;

    public static UIAssets instance
    {
        get
        {
            if (i == null)
            {
                i = Instantiate(Resources.Load("UIAssets") as GameObject).GetComponent<UIAssets>();
            }
            return i;
        }
    }


    public Sprite smoke;
}
