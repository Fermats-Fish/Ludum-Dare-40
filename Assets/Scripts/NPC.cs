using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Entity {

	protected override void Start(){
		// Need to add yourself to the list of npcs.
		GameController.instance.npcs.Add (this);

		base.Start ();
	}

	public virtual void Move(){
		// Moves the NPC based on AI.
	}

	protected void PathTo(int dx, int dy){
		// Pathfinds the NPC to given coords.

		// For now just move it as directly as possible.

		// If there already then stop.
		if (x == dx && y == dy){
			return;
		}

		dx -= x;
		dy -= y;

		// Randomly choose to move vertically or horizontally.
		int r = Random.Range (0, Mathf.Abs (dx) + Mathf.Abs (dy));

		if (r < Mathf.Abs(dx)){
			// Move horizontally.
			if (dx < 0){
				MoveLeft ();
			} else {
				MoveRight ();
			}
		} else {
			// Move vertically.
			if (dy < 0){
				MoveDown ();
			} else {
				MoveUp ();
			}
		}

		// Finally update our visual position.
		UpdatePosition ();
		
	}

	protected override void Despawn(){
		// Also need to remove from the list of npcs.
		GameController.instance.toRemove.Add (this);

		// If the player isn't dead, update the no. kills.
		if (GameController.instance.player.health > 0){
			GameController.instance.kills += 1;
			GameController.instance.UpdateUIText ();
		}

		// Then do the same stuff as all other entities.
		base.Despawn ();
	}

}
