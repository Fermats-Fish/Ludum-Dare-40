using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialSlot : MonoBehaviour {

	public Sprite defaultSprite;
	Image image;

	void Start(){
		image = GetComponent<Image> ();
	}

	// Update the ui of this slot to match whatever is specified.
	public void UpdateUITo(ItemType itemType){
		if (itemType == null){
			image.sprite = defaultSprite;
		} else {
			image.sprite = itemType.GetSprite ();
		}
	}

}
