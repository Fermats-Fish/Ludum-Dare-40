using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorItemMouseOver : MonoBehaviour {

	public ItemType item;

	public void OnMouseOver (){
		if (item != null){
			UIManager.instance.UpdateInspectorText (item);
		}
	}

	public void OnMouseExit (){
		UIManager.instance.CloseInspectorText ();
	}
}
