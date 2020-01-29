using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLine : MonoBehaviour {

	static readonly float timeVisable = 0.1f;

	float time;

	void Start (){
		time = timeVisable;
	}

	void Update () {
		time -= Time.deltaTime;
		if (time <= 0){
			Destroy (gameObject);
		}
	}
}
