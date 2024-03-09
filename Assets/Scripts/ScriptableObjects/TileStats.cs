using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Tile
{
    public GridTile.TileType tile;
    public Vector2 uv00;
    public Vector2 uv11;
}

[CreateAssetMenu(fileName = "Tiles", menuName = "GameAsset/Tiles")]
public class TileStats : ScriptableObject
{
   public List<Tile> tiles;
}


