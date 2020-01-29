using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualRecipeUI : MonoBehaviour {

	public CraftingRecipe recipe;

	public void Click (){
		// Tell the player to try to craft the item.
		GameController.instance.player.TryCraft (recipe);
	}
}
