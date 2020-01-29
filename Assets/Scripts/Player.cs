using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity {

	public Dictionary<ItemType, int> inventory = new Dictionary<ItemType, int>();
	public List<ItemType> specialInventorySlots = new List<ItemType>{null};

	public GameObject deathText;

	public override bool MoveTo( int dx, int dy ){

		// Do default movement stuff.
		bool output = base.MoveTo (dx, dy);

		// Generate map around the player if not already done, and only show gameobjects for tiles within a certain range of the player.
		GameController.instance.map.GenerateMapAround (x, y);

		// Return whether the movement was successful. Note the ai's get their turn even if the movement was unsuccesssful.
		return output;

	}

	public override bool Attack(int dx, int dy){
		// Check you have the ammo required to the the attack.
		if (specialInventorySlots[0] != null && specialInventorySlots [0].ammo != "" && GetAmountOf (ItemType.GetItemTypeWithName(specialInventorySlots [0].ammo)) <= 0){
			return false;
		}

		// Do attack as normal and store if successful.
		bool success = base.Attack (dx, dy);

		// If successful, there is a random chance the weapon used will break.
		if (success == true && specialInventorySlots[0] != null){

			ItemType itemType = specialInventorySlots [0];

			// See if it breaks based on its break chance.
			float r = Random.Range (0f, 1f);
			if (r < itemType.breakChance){

				// The item breaks!

				// Remove it from the special slot if there are none left in your inventory (otherwise remove one of the ones already in the inventory
				//   so that it automatically equips that one.
				if (GetAmountOf (itemType) == 0) {
					RemoveFromSpecial (0);
				}

				// Now remove one of those items from the inventory.
				RemoveFromInventory (itemType);

			}

			// Use up ammo (if it takes ammo).
			if (itemType.ammo != ""){
				RemoveFromInventory (ItemType.GetItemTypeWithName (itemType.ammo));
			}

		}

		return success;
	}

	public void RegenerateHealth(){
		//TakeDamage (- maxHealth / 100f );     /// GO HERE TO TURN ON HEALTH REGENERATION.

	}

	protected override int GetRange(){
		// If nothing in special slot 1, as normal.
		if (specialInventorySlots[0] == null){
			return base.GetRange ();
		}

		// Otherwise it is the range of whatever is in that slot.
		else {
			return specialInventorySlots [0].range;
		}

	}

	protected override float GetAttack(){
		// If nothing in special slot 1, as normal.
		if (specialInventorySlots[0] == null){
			return base.GetAttack ();
		}

		// Otherwise it is the range of whatever is in that slot.
		else {
			return specialInventorySlots [0].attack;
		}

	}

	int GetAmountOf(ItemType itemType){

		if (inventory.ContainsKey (itemType) == false){
			return 0;
		}

		int currAmount;
		inventory.TryGetValue (itemType, out currAmount);
		return currAmount;
	}

	// Return the sum of the value in the inventory.
	public int GetTotalValue(){

		int sum = 0;

		// Loop through inventory.
		foreach (ItemType item in inventory.Keys) {

			// For each item type in the inventory, add that item types value * how many of that item there are.
			sum += item.value * GetAmountOf (item);

		}

		// Loop through special inventory.
		foreach (ItemType item in specialInventorySlots) {

			// Add the item's value.
			if (item != null) {
				sum += item.value;
			}

		}

		// return the sum.
		return sum;

	}

	public bool TryCraft (CraftingRecipe recipe){

		// Check we have the required items.
		foreach (ItemStack itemStack in recipe.inputs) {

			// If we don't have enough of it, return false and abort the crafting.
			if (GetAmountOf (itemStack.itemType) < itemStack.amount){
				return false;
			}

		}

		// We have enough of each of the resources, so remove the inputs from our inventory.
		foreach (ItemStack itemStack in recipe.inputs) {
			RemoveFromInventory (itemStack);
		}

		// Finally add in the item we are crafting.
		AddToInventory (recipe.output);

		// Return a successful craft, and run the next game tick.
		GameController.instance.RunNextTick ();
		return true;

	}

	public bool SwitchToSpecial(ItemType itemType){

		// Figure out if that item CAN switch to a special slot.
		if (itemType.specialSlot < 0 || itemType.specialSlot > specialInventorySlots.Count){
			return false;
		}

		// Now check what's currently in that special slot, and add it to the inventory (if its not null).
		ItemType currItem = specialInventorySlots [itemType.specialSlot];
		if (currItem != null) {
			AddToInventory (currItem);
		}

		// Now remove one of this from the inventory.
		RemoveFromInventory (itemType);

		// Now make what's in the special slot this, and update that visually.
		specialInventorySlots [itemType.specialSlot] = itemType;
		UIManager.instance.SetSpecialSlot (itemType.specialSlot, itemType);

		return true;
	}

	public bool RemoveFromSpecial(int slot){

		// Figure out if that slot exists.
		if (slot < 0 || slot > specialInventorySlots.Count){
			return false;
		}

		ItemType currItem = specialInventorySlots [slot];

		// Make sure somethings in there.
		if (currItem == null){
			return false;
		}

		// Add what's in there to the inventory.
		AddToInventory (currItem);

		// Remove the item from the special slot.
		specialInventorySlots [slot] = null;

		// Update the visuals.
		UIManager.instance.SetSpecialSlot (slot, null);

		return true;

	}

	public bool PickUpItemAt (int dx, int dy){

		// First make sure we can pick up that item.

		// Make sure it is adjacent to us.
		if (Mathf.Abs (x - dx) + Mathf.Abs (y - dy) != 1) {
			// Not adjacent so abort.
			return false;
		}

		// Make sure there is an item there which we can pick up.
		ItemType itemType = GameController.instance.map.GetTileAt(dx, dy).item;

		if (itemType == null) {
			// No tile there so abort!
			return false;
		}

		// Remove the item from the map.
		GameController.instance.map.RemoveItemAt (dx, dy);

		// Now place it in our inventory somewhere.
		AddToInventory (itemType);

		return true;
	}

	void AddToInventory(ItemType itemType){
		AddToInventory (itemType, 1);
	}

	void AddToInventory(ItemStack itemStack){
		AddToInventory (itemStack.itemType, itemStack.amount);
	}

	void AddToInventory (ItemType itemType, int amount){

		// If the amount is less than or equal to 0 then just abort.
		if (amount <= 0){
			return;
		}

		// If there isn't yet any of this itemType in our inventory, create a new key and set to 1.
		if (inventory.ContainsKey (itemType) == false){
			inventory.Add (itemType, amount);

			// Also set correct UI
			UIManager.instance.AddSlot (itemType);
			UIManager.instance.ChangeSlotAmountTo (itemType, amount);
		}

		// Otherwise just add the value for this type.
		else {
			int currAmount = GetAmountOf (itemType);

			// Remove from inventory before you add it back in again with a changed value.
			inventory.Remove (itemType);
			inventory.Add (itemType, currAmount + amount);

			// Also set correct UI
			UIManager.instance.ChangeSlotAmountTo (itemType, currAmount + amount);
		}

	}

	void RemoveFromInventory(ItemType itemType){
		RemoveFromInventory (itemType, 1);
	}

	void RemoveFromInventory(ItemStack itemStack){
		RemoveFromInventory (itemStack.itemType, itemStack.amount);
	}

	void RemoveFromInventory(ItemType itemType, int amount){

		// Double check we have some.
		if (inventory.ContainsKey (itemType) == false){
			return;
		}

		// Get the current amount in the inventory.
		int currAmount = GetAmountOf (itemType);

		// If the amount we are removing is more than or equal to the current amount...
		if (amount >= currAmount){
			// Remove the key from the inventory.
			inventory.Remove (itemType);

			// Remove the UI for the item in our inventory.
			UIManager.instance.RemoveSlot (itemType);
		}

		// Otherwise...
		else {
			// Remove from inventory before you add it back in again with a changed value.
			inventory.Remove (itemType);
			inventory.Add (itemType, currAmount - amount);

			// Also set correct UI
			UIManager.instance.ChangeSlotAmountTo (itemType, currAmount - amount);
		}

	}

	protected override void Despawn(){

		// Display the death text.
		deathText.SetActive (true);

		// Change the text to match score.
		deathText.GetComponent<Text> ().text = "YOU DIED!!!\nSteps Taken: " + GameController.instance.steps + "\nEnemies Killed: " + GameController.instance.kills;

	}
}
