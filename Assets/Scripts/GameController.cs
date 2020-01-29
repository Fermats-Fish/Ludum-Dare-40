using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public GameObject attackLinePrefab;

	public GameObject playerGO;
	public Player player;

	public GameObject mapGO;
	public Map map;

	public List<NPC> npcs;
	public List<NPC> toRemove;

	public static GameController instance;

	public int steps = 0;
	public int kills = 0;

	public GameObject scoreUI;
	Text uiText;

	void Start () {
		if (instance != null){
			// Something went wrong and we have two GameControllers.
			Debug.LogError ("There are two game controllers for some reason.");
		} else {
			instance = this;
		}

		uiText = scoreUI.GetComponent<Text> ();
		player = playerGO.GetComponent<Player> ();
		map = mapGO.GetComponent<Map> ();
	}

	void Update () {

		// Check if the user clicks.


		// Left Click.
		if (Input.GetMouseButtonDown (0)){


			// GAME LEFT CLICK!


			// Find out where the user clicked.
			RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);

			if (hit.collider != null){
				// The user clicked on something.

				// Check if it's a tile.
				if (hit.collider.gameObject.tag == "Tile"){
					
					// Get the coordinates of the tile.
					int x = Mathf.FloorToInt (hit.collider.transform.position.x);
					int y = Mathf.FloorToInt (hit.collider.transform.position.y);

					//TODO: For now we will assume the player wants to pick up whatever is on the tile. But later on there will be other things that they can do.
					bool success = player.PickUpItemAt (x, y);

					// If the player picked something up, run the next tick.
					if (success == true){
						RunNextTick ();
					}
				}
			}
		}

		// Right Click.
		else if (Input.GetMouseButtonDown (1)){


			// USER INTERFACE RIGHT CLICK!


			// Find out where the user clicked (UI).
			List<RaycastResult> results = new List<RaycastResult> ();

			PointerEventData ped = new PointerEventData (null);
			ped.position = Input.mousePosition;
			UIManager.instance.gr.Raycast (ped, results);

			// Should now have a list of things which were hit. Loop through the list and see if anything is tagged as either an InventorySlot, or a SpecialSlot.
			foreach (RaycastResult result in results) {

				// Check if it's a special item slot.
				if (result.gameObject.tag == "SpecialSlot"){

					// Figure out which slot it was.
					int slot = UIManager.instance.GetSpecialSlotNumberFromGO (result.gameObject);

					// Tell the player to switch out from that slot.
					player.RemoveFromSpecial (slot);

					// Break out of the loop now.
					return;
				}

				// Otherwise check if it's a normal item slot.
				else if (result.gameObject.tag == "InventorySlot"){

					// Get the type of item in the normal slot.
					ItemType itemType = UIManager.instance.GetSlotItemType (result.gameObject);

					// Tell the player to switch it to their hand.
					player.SwitchToSpecial (itemType);

					// Break out of the loop now.
					return;
				}

			}


			// GAME RIGHT CLICK!


			// Find out where the user clicked.
			RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);

			if (hit.collider != null){
				// The user clicked on something.

				// Check if it's a tile.
				if (hit.collider.gameObject.tag == "Tile"){

					// Get the coordinates of the tile.
					int x = Mathf.FloorToInt (hit.collider.transform.position.x);
					int y = Mathf.FloorToInt (hit.collider.transform.position.y);

					// Tell the player to try to attack the tile.
					bool success = player.Attack (x, y);

					// If successful, run the next tick.
					if (success == true) {
						RunNextTick ();
					}
				}
			}

		}


		// If the user presses a movement key, move the player, which will trigger the map to generate around the player.

		else if (LeftKeyPressed ()){

			player.MoveLeft ();
			RunNextTick ();

		} else if (RightKeyPressed ()){

			player.MoveRight ();
			RunNextTick ();

		} else if (DownKeyPressed ()){

			player.MoveDown ();
			RunNextTick ();

		} else if (UpKeyPressed ()){

			player.MoveUp ();
			RunNextTick ();

		} else if (PassKeyPressed ()){
			RunNextTick ();
		}

	}

	public void UpdateUIText(){
		uiText.text = "Steps: " + steps.ToString () + "\nKills: " + kills;
	}

	bool LeftKeyPressed(){
		return Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow);
	}

	bool RightKeyPressed(){
		return Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow);
	}

	bool DownKeyPressed(){
		return Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow);
	}

	bool UpKeyPressed(){
		return Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow);
	}

	bool PassKeyPressed(){
		return Input.GetKeyDown (KeyCode.Space);
	}

	public void RunNextTick(){

		// Loop through all the npc's and tell them to move according to their AI (might involve despawning if too far away from character).
		foreach (NPC npc in npcs) {
			npc.Move ();
		}

		// See if any npcs need removing from the list of npcs.
		foreach (NPC npc in toRemove) {
			npcs.Remove (npc);
		}

		// Clear the toRemove list.
		toRemove.Clear();

		// See if any npcs need spawning.
		map.EntitySpawnTick ();

		// Regenerate player health.
		player.RegenerateHealth ();

		// If the player isn't dead, update the no. steps taken.
		if (player.health > 0){
			steps += 1;
			UpdateUIText ();
		}
	}
}
