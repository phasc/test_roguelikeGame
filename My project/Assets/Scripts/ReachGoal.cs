using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReachGoal : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {   //check for colision with the object

		if (col.gameObject.tag == "Player") {  // if colided with the player

			GameObject dungeon = GameObject.FindGameObjectWithTag ("Dungeon");  //finds the Dungeon object
			DungeonGeneration dungeonGeneration = dungeon.GetComponent<DungeonGeneration> ();   //gets the object
			dungeonGeneration.ResetDungeon ();  //resets the dungeon
			SceneManager.LoadScene ("Demo");  //loads the scene
		
		}
	}
}
