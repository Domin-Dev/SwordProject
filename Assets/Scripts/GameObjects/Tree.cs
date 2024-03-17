using System;
using UnityEngine;


public class Tree : MonoBehaviour, ILifePoints,ITransparent
{
    SpriteRenderer renderer;
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(isHit) UpdateHit();
    }

    void ILifePoints.Hit(int damage, Vector2 dir)
    {
        SetHit(20, dir);
    }
    void ILifePoints.Kill()
    {
    
    }

    void ITransparent.Hide()
    {
        Debug.Log("hide");
        renderer.color = new Color(1, 1, 1, 0.5f);
    }

    void ITransparent.Show()
    {
        renderer.color = new Color(1, 1, 1, 1f);
    }


    bool isHit;
    float repulseRotation;
    float lastRotation;
    bool standUp;


    public void SetHit(float angle,Vector2 hitDir)
    {
            isHit = true;
            if (hitDir.x < transform.position.x)
            {
                angle = -angle;
            }

            repulseRotation = angle;
            lastRotation = transform.rotation.z;
            standUp = false;
    }
    public void UpdateHit()
    {
        if (!standUp)
        {
            float value = Mathf.LerpAngle(transform.localEulerAngles.z, repulseRotation, Time.deltaTime * 12f);
            transform.eulerAngles = new Vector3(0, 0, value);

            if (Math.Abs(Mathf.DeltaAngle(value, repulseRotation)) <= 2)
            {
                standUp = true;
            }
        }
        else
        {
            float value = Mathf.LerpAngle(transform.localEulerAngles.z, lastRotation, Time.deltaTime * 15f);
            transform.eulerAngles = new Vector3(0, 0, value);

            if (Math.Abs(Mathf.DeltaAngle(value, lastRotation)) <= 2)
            {
                transform.eulerAngles = new Vector3(0, 0, lastRotation);
                isHit = false;
            }
        }
    }


}
