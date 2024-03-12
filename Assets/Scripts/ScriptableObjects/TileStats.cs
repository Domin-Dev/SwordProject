using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Tile
{
    public GridTile.TileType tile;
    public Vector2 uv00;
    public int variants;
    public int chance;
}

[CreateAssetMenu(fileName = "Tiles", menuName = "GameAsset/Tiles")]
public class TileStats : ScriptableObject
{
   public List<Tile> tiles;
}


