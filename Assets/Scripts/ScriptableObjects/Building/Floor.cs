using UnityEngine;

public class TileTexture
{
    public Texture2D texture;
    public int tileIndex;
}

[CreateAssetMenu(fileName = "Floor", menuName = "GameAsset/Items/BuildingItems/Floor")]
public class Floor : BuildingItem 
{
    public Texture2D grassTexture;
    public int chanceOfDefaultTile;
}

