using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMenuButton : MonoBehaviour {

	public GameObject craftingMenu;

	public void Click(){
		craftingMenu.SetActive ( !craftingMenu.activeSelf );
	}

}
