using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NPC {

	public override void Move(){
		// Figure out where the player is.
		Player p = GameController.instance.player;

		// Try to attack them.
		bool success = Attack (p.x, p.y);

		// If successfull then don't do anything else.
		if (success == true){
			return;
		}

		// Otherwise try to move instead.

		// Move to there.
		PathTo (p.x, p.y);

		// Invoke the parent move command as well.
		base.Move ();
	}

}
