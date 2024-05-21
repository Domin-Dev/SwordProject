using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall", menuName = "GameAsset/Items/BuildingObjects/Wall")]
public class Wall : BuildingObject
{
    public ObjectVariant[] objectVariants;
    #if UNITY_EDITOR
        public ObjectVariant[] _objectVariants
        {
            set
            {
                objectVariants = value;
            }
        }
    #endif
}


[System.Serializable]
public class ObjectVariant
{
    public Vector2[] hitbox;
    public Sprite sprite;

    public ObjectVariant(Vector2[] hitbox,Sprite sprite)
    {
        this.hitbox = hitbox;
        this.sprite = sprite;
    }
}

