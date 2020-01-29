using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipesUI : MonoBehaviour {

	public GameObject recipeUIPrefab;
	public GameObject itemSlotPrefab;

	void Start () {
		// Go through all the recipes and create a ui element for them.
		foreach (CraftingRecipe recipe in CraftingRecipe.craftingRecipes) {

			// First instantiate a recipe.
			GameObject go = Instantiate (recipeUIPrefab);

			// Set its parent.
			go.transform.SetParent (transform);

			// Store the recipe that it refers to.
			go.GetComponent<IndividualRecipeUI> ().recipe = recipe;

			// Add in ui elements to show what is required.
			GameObject inputs = go.transform.GetChild (0).gameObject;


			// For each of the recipe inputs add in an item slot.
			foreach (ItemStack stack in recipe.inputs){

				// Instantiate the stack's ui.
				GameObject stackGO = Instantiate (itemSlotPrefab);

				// Set its mouse over.
				stackGO.GetComponent<InspectorItemMouseOver> ().item = stack.itemType;

				// Set its parent.
				stackGO.transform.SetParent (inputs.transform);

				// Set the right sprite.
				stackGO.GetComponent<Image> ().sprite = stack.itemType.GetSprite ();

				// Set the text to display the right number.
				stackGO.transform.GetChild (0).GetComponent<Text> ().text = stack.amount.ToString ();

				////////////////////////////////////////////////////////////////////////////////////////////////////// TODO: Change the colour if you don't have the resources.
			}

			// Finally make sure the output is right.
			GameObject output = go.transform.GetChild (2).gameObject;

			// Set its mouse over.
			output.GetComponent<InspectorItemMouseOver> ().item = recipe.output.itemType;

			// Set the right sprite.
			output.GetComponent<Image> ().sprite = recipe.output.itemType.GetSprite ();

			// Set the text to display the right number.
			output.transform.GetChild (0).GetComponent<Text> ().text = recipe.output.amount.ToString ();
		}
	}

}
