using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

	public int x = 0;
	public int y = 0;

	public GameObject healthBar;
	SpriteRenderer healthBarSr;

	public float maxHealth = 10f;
	public float health = 10f;

	public int range = 1;
	public float attack = 1f;

	// List of all entities.
	protected static List<Entity> entities = new List<Entity>();

	public static Entity GetEntityAt(int x, int y){

		// Loop through all the entities and see if any have the same coords as this.
		foreach (Entity entity in entities) {
			if (entity.x == x && entity.y == y){
				// This is the target entity.
				return entity;
			}
		}

		// Couldn't find it.
		return null;
	}

	protected virtual void Start(){
		// Find the health bar's sprite renderer for later.
		healthBarSr = healthBar.GetComponent<SpriteRenderer> ();

		// Add yourself to the list of all entities.
		entities.Add (this);
	}

	void UpdateHealthBarVisual(){
		// Update the width.
		healthBarSr.size = new Vector2 (health / maxHealth, healthBarSr.size.y);
	}

	public virtual void UpdatePosition(){
		// If we are too far away from the player, then despawn, but spawn something new near the player.
		if (Mathf.Max (Mathf.Abs(GameController.instance.player.x - x), Mathf.Abs(GameController.instance.player.y - y)) > GameController.instance.map.entityDespawnRadius){
			Despawn ();
			GameController.instance.map.SpawnMonster ();
		}

		transform.position = new Vector3 ((float)x, (float)y);
	}

	public virtual bool Attack(int dx, int dy){
		// Check if the object is in range.

		if (Mathf.Pow (dx - x, 2) + Mathf.Pow (dy - y, 2) > Mathf.Pow (GetRange (), 2)){
			// We are out of range. Abort!
			return false;
		}

		// We are in range, so now check to see if there is any entity at the said position.

		Entity target = GetEntityAt (dx, dy);

		// If we didn't find a target, abort!
		if (target == null){
			return false;
		}

		// Apply damage to the target based on our attack.
		target.TakeDamage (GetAttack ());

		// Draw a line from us to the target.

		// Instantiate the line and get its line renderer component.
		LineRenderer line = Instantiate (GameController.instance.attackLinePrefab).GetComponent<LineRenderer> ();

		// Set the positions of the line.
		line.SetPositions (new Vector3[]{ new Vector3 ( ((float)x) + 0.7f, ((float)y) + 0.7f) , new Vector3 ( ((float)target.x) + 0.3f, ((float)target.y) + 0.3f) });


		return true;
	}

	public void TakeDamage(float damage){
		health -= damage;
		if (health <= 0){
			health = 0;
			Despawn ();
		} else if (health > maxHealth){
			health = maxHealth;
		}
		UpdateHealthBarVisual ();
	}

	protected virtual int GetRange(){
		// For the base entity class return range. For player's it will depend on the equiped item.
		return range;
	}

	protected virtual float GetAttack(){
		// For the base entity class return attack. For player's it will depend on the equiped item.
		return attack;
	}

	public virtual bool MoveTo( int dx, int dy ){
		// Moves the entity to the coords given (IF ABLE! If unable then restore some stamina).

		// First check the destination is adjacent.
		if (Mathf.Abs (x - dx) + Mathf.Abs (y - dy) != 1 ){
			// Not adjacent so abort.
			return false;
		}

		// Now make sure it's not already occupied.
		if (GetEntityAt (dx, dy) != null){
			return false;
		}

		// Get the destination tile.
		Tile destTile = GameController.instance.map.GetTileAt(dx, dy);

		// Now check that the tile they are moving to isn't water.
		if ( destTile.IsImpassable() ){
			return false;
		}

		// At this point we know the player can move there. So move there!
		x = dx; y = dy;

		// Update the entity's visual position.
		UpdatePosition ();

		// Return that the movement was successful!
		return true;

	}

	public void MoveLeft(){
		MoveTo (x - 1, y);
	}

	public void MoveRight(){
		MoveTo (x + 1, y);
	}

	public void MoveDown(){
		MoveTo (x, y - 1);
	}

	public void MoveUp(){
		MoveTo (x, y + 1);
	}

	protected virtual void Despawn(){
		// Despawn the object.
		entities.Remove (this);
		Destroy (gameObject);
	}
}
