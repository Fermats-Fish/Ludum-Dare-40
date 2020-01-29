using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType{Water, Grass, Desert, Rock, Snow}

public class Tile {

	public TileType tileType;
	public ItemType item = null;

	public bool IsImpassable(){
		return tileType == TileType.Water;
	}

	public Sprite GetSprite(){
		return Resources.Load<Sprite> ("TileTypeSprites/" + tileType.ToString ());
	}

}

