using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	public int generateRadius = 40;
	public int entityDespawnRadius = 50;

	public float seed;

	public float wavelength = 25f;

	public GameObject tilePrefab;
	public GameObject itemPrefab;

	public GameObject enemyPrefab;

	public int nStartingMonsters = 5;

	public float baseSpawnProbability = 0.03f;
	public float valueProbabilityWeight = 0.1f;
	public int maxSpawnAttempts = 50;

	public float minSpawnDistance = 20;
	public float maxSpawnDistance = 30;

	public Dictionary<TileType, float> biomeSpawnChance = new Dictionary<TileType, float>{
		{TileType.Water, 0f},
		{TileType.Grass, 0.03f},
		{TileType.Desert, 0.03f},
		{TileType.Rock, 0.03f},
		{TileType.Snow, 0.02f}
	};

	Dictionary<TileType, float> biomeItemSpawnWeightSum = new Dictionary<TileType, float>();
	Dictionary<TileType, float> biomeEnemySpawnWeightSum = new Dictionary<TileType, float>();


	Array2D<Tile> map = new Array2D<Tile> (null);
	Dictionary<Vector2, GameObject> tileGOs = new Dictionary<Vector2, GameObject> ();

	public float waterCutOff = 0.3f;
	public float mountainCutOff = 0.7f;
	public float wetnessCutOff = 0.5f;

	void Start () {
		// If we don't have a seed generate a random one.
		if (seed == 0f){
			seed = Random.Range (1000f, 100000f);
		}


		// Calculate the total weighting for choosing which item spawns in each biome.
		foreach (TileType tileType in new List<TileType>{TileType.Grass, TileType.Desert, TileType.Rock, TileType.Snow}) {

			float sum = 0f;

			foreach (ItemType itemType in ItemType.items) {
				float delta;
				itemType.spawnWeights.TryGetValue (tileType, out delta);
				sum += delta;
			}

			biomeItemSpawnWeightSum.Add (tileType, sum);

		}

		// Now do the same for enemies.
		foreach (TileType tileType in new List<TileType>{TileType.Grass, TileType.Desert, TileType.Rock, TileType.Snow}) {

			float sum = 0f;

			foreach (EnemyType enemyType in EnemyType.enemyTypes) {
				float delta;
				enemyType.spawnWeights.TryGetValue (tileType, out delta);
				sum += delta;
			}

			biomeEnemySpawnWeightSum.Add (tileType, sum);

		}


		GenerateTileAt (-1000, -1000); 
		GenerateTileAt (1000, 1000);

		// Generate the map around the player.
		GenerateMapAround (0, 0);

		// Geerate some starting monsters.
		for (int i = 0; i < nStartingMonsters; i++) {
			SpawnMonster ();
		}
		
	}

	public Tile GetTileAt(int x, int y){

		Tile t = map.GetAt (x, y);

		// If the tile is null then that tile hasn't been generated yet. So generate it!
		if (t == null){
			GenerateTileAt (x, y);
			t = map.GetAt (x, y);
		}

		return t;
	}

	public void EntitySpawnTick(){
		// Called every tick to decide if an entity should spawn.

		// Get a random number.
		float r = Random.Range (0f, 1f);

		// Calculate spawn probability, and see if an enemy spawns.                                 // SPAWN CHANCE
		float spawnProb = baseSpawnProbability * (1 + GameController.instance.player.GetTotalValue () * valueProbabilityWeight);

		if (r < spawnProb){
			// Spawn a monster!
			SpawnMonster ();
		}

	}

	public void SpawnMonster(){
		// First calculate where to spawn it.

		Tile spawnTile = null;
		int attempts = 0;

		int x = 0;
		int y = 0;

		// Keep calculating new places to spawn until either exceed the maximum amount of attempts allowed, or you find a valid spawn tile which isn't water.
		while ( (spawnTile == null || spawnTile.tileType == TileType.Water) && attempts < maxSpawnAttempts ) {

			// Get a random direction.
			Vector2 dir = (Vector2) Random.onUnitSphere;
			dir = dir.normalized;

			// Get a random distance.
			float distance = Random.Range (minSpawnDistance, maxSpawnDistance);

			dir *= distance;

			// Figure out the closest tile.
			x = GameController.instance.player.x + Mathf.RoundToInt (dir.x);
			y = GameController.instance.player.y + Mathf.RoundToInt (dir.y);

			// Get that tile.
			spawnTile = GetTileAt (x, y);

			attempts += 1;
		}

		// If we failed to find a spawn tile, abort!
		if (attempts >= maxSpawnAttempts){
			return;
		}

		// Now figure out which monster to spawn based on the biome.

		// Generate a random number between 0 and the sum of the weights for this biome.
		float sumOfWeights;
		biomeEnemySpawnWeightSum.TryGetValue (spawnTile.tileType, out sumOfWeights);
		float r = Random.Range (0f, sumOfWeights);

		// Sum through all the monster's weights until the sum is more than the random number generated.
		EnemyType enemyType = null;
		float sum = 0f;

		foreach (EnemyType eT in EnemyType.enemyTypes) {
			float weight;
			eT.spawnWeights.TryGetValue (spawnTile.tileType, out weight);
			sum += weight;

			if (sum > r){
				enemyType = eT;
				break;
			}
		}

		if (enemyType == null){
			Debug.LogError ("Item type didn't get chosen for some reason.");
		}


		// We should now have figured out which enemy type to spawn, so spawn one!
		GameObject enemyGO = Instantiate (enemyPrefab);

		// Get the enemy component.
		Enemy enemy = enemyGO.GetComponent<Enemy> ();

		// Now setup all of its values.                                 /// ENEMY SETUP - Add more things? e.g. name / armour.
		enemy.maxHealth = enemy.health = enemyType.health;
		enemy.attack = enemyType.attack;
		enemy.range = enemyType.range;
		enemy.x = x; enemy.y = y;
		enemy.transform.position = new Vector3 (enemy.x, enemy.y, -1);

		// Get the sprite renderer, and set the enemy's sprite.
		enemy.GetComponent<SpriteRenderer> ().sprite = enemyType.GetSprite ();

		// Should be done.

	}

	public TileType GetTileTypeFromValues(float height, float wetness, int x, int y){

		// Can't be water in a 10 tile radius of start.
		if (height < waterCutOff && x*x + y*y > 100){
			return TileType.Water;
		} 

		else if (height < mountainCutOff){
			if (wetness < wetnessCutOff){
				return TileType.Desert;
			} else {
				return TileType.Grass;
			}
		} 

		else {
			if (wetness < wetnessCutOff){
				return TileType.Rock;
			} else {
				return TileType.Snow;
			}
		}
	}

	public void RemoveItemAt(int x, int y){

		// Double check there is a tile there.
		if(GetTileAt(x, y).item == null){
			print ("No item there!");
			return;
		}

		// Remove the tiledata for the item there.
		GetTileAt (x, y).item = null;

		// Get the GO there.
		GameObject go;
		tileGOs.TryGetValue (new Vector2((float) x, (float) y), out go);

		// Destroy its child if there's one to destroy.
		if (go != null && go.transform.childCount != 0) {
			Destroy (go.transform.GetChild (0).gameObject);
		}
	}

	public void GenerateMapAround(int px, int py){
		
		// First generate the actual tiles within a certain rectangle of the player.
		for (int x = px - generateRadius; x <= px + generateRadius; x++) {
			for (int y = py - generateRadius; y <= py + generateRadius; y++) {
				GenerateTileAt (x, y);
			}
		}

		// Figure out the x / y bounds which the camera can see.
		int ySize = (int) Camera.main.orthographicSize;

		int minY = py - (ySize + 1);
		int maxY = py +  ySize;

		int minX = px - (int)((ySize + 1) * Camera.main.aspect);
		int maxX = px + (int)( ySize      * Camera.main.aspect);

		// Now loop through all the currently generated gameObjects, and delete any outside view of the camera, by adding them to a toRemove list.
		List<Vector2> toRemove = new List<Vector2>();

		foreach (Vector2 coord in tileGOs.Keys) {

			// Check if out of range...
			if (coord.x < minX || coord.x > maxX || coord.y < minY || coord.y > maxY){

				// If so add to the toRemove list.
				toRemove.Add (coord);
			}
		}

		foreach (Vector2 coord in toRemove){

			// Delete the object and remove the key.
			GameObject go;
			tileGOs.TryGetValue (coord, out go);
			tileGOs.Remove (coord);
			Destroy (go);

		}

		// Now loop through the gameobjects which we currently need visible, and make sure they are.
		for (int x = minX; x <= maxX; x++){
			for (int y = minY; y <= maxY; y++){

				Vector2 pos = new Vector2 (x, y);

				// If the gameobject isn't there...
				if (tileGOs.ContainsKey ( pos ) == false){
					// Make it!!

					// Instantiate a Game Object.
					GameObject go = Instantiate (tilePrefab);

					// Set its position.
					go.transform.position = new Vector3 ((float)x, (float)y, 10f);

					// Set the correct sprite.
					go.GetComponent<SpriteRenderer> ().sprite = GetTileAt(x, y).GetSprite();

					// Add the go to the dictionary.
					tileGOs.Add (pos, go);

					// Now see if it has a tile or not.
					if (GetTileAt(x, y).item != null){

						// It has an item! So make a gameobject for that too.
						GameObject itemGO = Instantiate (itemPrefab);

						// Set its position.
						itemGO.transform.position = new Vector3 ((float)x, (float)y, 5f);

						// Set its parent.
						itemGO.transform.SetParent (go.transform);

						// Finally set its texture.
						itemGO.GetComponent<SpriteRenderer> ().sprite = GetTileAt (x, y).item.GetSprite();
					}
				}

			}
		}

	}

	void GenerateTileAt(int x, int y){

		// Maybe don't generate it again if its already generated. This tooottallly wasn't something I forgot to add, which caused me to spend half an hour wondering why I couldn't pick up any items.
		//     (its because every time I moved all of the items changed position because they were generated again).

		if (map.GetAt (x, y) != null){
			return;
		}

		// Create the tile to go at these coords.
		Tile t = new Tile ();

		// Figure out the wetness and height level.
		float height  = Mathf.PerlinNoise (    seed + ((float)x) / wavelength,     seed + ((float)y) / wavelength);
		float wetness = Mathf.PerlinNoise (2 * seed + ((float)x) / wavelength, 2 * seed + ((float)y) / wavelength);

		// Set the tiletype based on the wetness and height level.
		t.tileType = GetTileTypeFromValues (height, wetness, x, y);

		// Set the tile into the map.
		map.setAt (x, y, t);

		// Now see if a random item will spawn on the tile.
		float r = Random.Range (0f, 1f);

		float spawnChance;
		biomeSpawnChance.TryGetValue (t.tileType, out spawnChance);

		if (r < spawnChance){
			// We can generate an item here.

			// First figure out what type of item it should be.

			// Generate a random number between 0 and the sum of the weights for this biome.
			float sumOfWeights;
			biomeItemSpawnWeightSum.TryGetValue (t.tileType, out sumOfWeights);
			r = Random.Range (0f, sumOfWeights);

			// Now go through all the items summing their weights until the sum is more than the random number generated.
			ItemType itemType = null;
			float sum = 0f;

			foreach (ItemType iT in ItemType.items) {
				float weight;
				iT.spawnWeights.TryGetValue (t.tileType, out weight);
				sum += weight;

				if (sum > r){
					itemType = iT;
					break;
				}
			}

			if (itemType == null){
				Debug.LogError ("Item type didn't get chosen for some reason.");
			}

			// Now we have an item type, so spawn an item!
			t.item = itemType;

		}
	}
}

