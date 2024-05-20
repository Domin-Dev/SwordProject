using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "GameAsset/Items/BuildingObjects/Wall")]
public class Wall : BuildingObject
{
    public Sprite[] sprites { private set; get; }
    #if UNITY_EDITOR
        public Sprite[] _sprites
        {
            set
            {
                sprites = value;
            }
        }
    #endif
    public Texture2D texture;
}


