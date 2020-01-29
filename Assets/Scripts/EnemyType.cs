using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType {

	public static readonly List<EnemyType> enemyTypes = new List<EnemyType>{
		//            name         health range attack     grass  desert    rock    snow 
		new EnemyType("Warrior",   10f,    2,     1f,       1f,    1f,      0f,     1f),
		new EnemyType("Archer",     4f,   10,   0.2f,       1f,    1f,      2f,     2f)
	};

	public string name; // The enemy's name. Don't have two enemies with the same name.
	public float health;
	public int range;
	public float attack;
	public Dictionary<TileType, float> spawnWeights; // The weighting for this enemy to spawn as opposed to others for each tileType.

	EnemyType ( string name, float health, int range, float attack, float grassSpawnWeight, float desertSpawnWeight, float rockSpawnWeight, float snowSpawnWeight){
		this.name = name;
		this.health = health;
		this.range = range;
		this.attack = attack;
		this.spawnWeights = new Dictionary<TileType, float> ();
		this.spawnWeights.Add (TileType.Grass, grassSpawnWeight);
		this.spawnWeights.Add (TileType.Desert, desertSpawnWeight);
		this.spawnWeights.Add (TileType.Rock, rockSpawnWeight);
		this.spawnWeights.Add (TileType.Snow, snowSpawnWeight);
	}

	public static EnemyType GetEnemyTypeWithName(string name){
		// Look for an item with the wrong type.
		foreach (EnemyType enemy in enemyTypes) {
			if (enemy.name == name){
				return enemy;
			}
		}

		// Didn't find it.
		return null;
	}

	public Sprite GetSprite(){
		return Resources.Load<Sprite> ("EnemySprites/" + name);
	}
}
