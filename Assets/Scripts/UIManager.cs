using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager instance;

	public GameObject inspectorTextGO;
	Text inspectorText;

	public GameObject itemSlotPrefab;
	public GameObject specialSlotsGO;
	public GameObject inventoryGO;

	public GraphicRaycaster gr;

	Dictionary<ItemType, GameObject> slots = new Dictionary<ItemType, GameObject>();
	Dictionary<GameObject, ItemType> slotItemTypes = new Dictionary<GameObject, ItemType>();

	void Start () {
		if (instance != null){
			// Something went wrong and we have two GameControllers.
			Debug.LogError ("There are two ui managers for some reason.");
		} else {
			instance = this;
		}

		inspectorText = inspectorTextGO.GetComponent<Text> ();
		gr = GetComponent<GraphicRaycaster> ();
	}

	public void UpdateInspectorText(ItemType itemType){
		inspectorTextGO.transform.parent.gameObject.SetActive (true);
		string ammoRequired = itemType.ammo;
		if (ammoRequired == ""){
			ammoRequired = "None";
		}
		inspectorText.text = itemType.name + ":\n  Value: " + itemType.value + "\n  Range: " + itemType.range.ToString () + "\n  Attack: " + itemType.attack.ToString () + " \n  Break Chance: " + (itemType.breakChance * 100).ToString () + "%\n  Ammo Required: " + ammoRequired;
	}

	public void CloseInspectorText(){
		inspectorTextGO.transform.parent.gameObject.SetActive (false);
	}

	public void SetSpecialSlot (int slot, ItemType itemType){

		// Check the slot is valid.
		if (slot < 0 || slot > specialSlotsGO.transform.childCount){
			return;
		}

		// Now get the correct slot.
		SpecialSlot ss = specialSlotsGO.transform.GetChild (slot).GetComponent<SpecialSlot> ();

		// Set the item type for the mouse over.
		ss.GetComponent<InspectorItemMouseOver> ().item = itemType;

		// Now tell it to update its sprite.
		ss.UpdateUITo (itemType);
	}

	public ItemType GetSlotItemType(GameObject slot){
		ItemType itemType;
		slotItemTypes.TryGetValue (slot, out itemType);
		return itemType;
	}

	public int GetSpecialSlotNumberFromGO(GameObject specialSlot){

		// Run a for loop through all the children of specialSlotsGO.
		for (int i = 0; i < specialSlotsGO.transform.childCount; i++) {
			if (specialSlotsGO.transform.GetChild (i).gameObject == specialSlot){
				// We've found it. It's this slot. So return it.
				return i;
			}
		}

		// We couldn't find it but still need to return something.
		Debug.LogError ("Couldn't find special slot for game object");
		return -1;

	}

	public void AddSlot (ItemType itemType) {

		// Instantiate an item slot.
		GameObject slot = Instantiate (itemSlotPrefab);

		// Tag it as an inventory slot.
		slot.tag = "InventorySlot";

		// Add it to the dictionary.
		slots.Add (itemType, slot);
		slotItemTypes.Add (slot, itemType);

		// Set its parent.
		slot.transform.SetParent (inventoryGO.transform);

		// Set the item image.
		slot.GetComponent<Image> ().sprite = itemType.GetSprite ();

		// Set the text.
		slot.transform.GetChild (0).GetComponent<Text> ().text = "1";

		// Set the item type for the mouse over.
		slot.GetComponent<InspectorItemMouseOver> ().item = itemType;

	}

	public void ChangeSlotAmountTo (ItemType itemType, int value){

		// Need to find out which slot has this current item.
		GameObject slot;
		slots.TryGetValue (itemType, out slot);

		// Now set the text.
		slot.transform.GetChild (0).GetComponent<Text> ().text = value.ToString ();
	}

	public void RemoveSlot (ItemType itemType){

		// Need to find out which slot has this current item.
		GameObject slot;
		slots.TryGetValue (itemType, out slot);

		// Remove it from the list of slots.
		slots.Remove (itemType);
		slotItemTypes.Remove (slot);

		// Now remove that slot.
		Destroy (slot);

	}
}
