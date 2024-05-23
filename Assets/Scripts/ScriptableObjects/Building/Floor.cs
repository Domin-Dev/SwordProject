using UnityEngine;

public class TileTexture
{
    public Texture2D texture;
    public int tileIndex;
}

[CreateAssetMenu(fileName = "Floor", menuName = "GameAsset/Items/BuildingObjects/Floor")]
public class Floor : BuildingObject 
{
    public TileTexture[] tileTextures;
    public int chanceOfDefaultTile;
}

