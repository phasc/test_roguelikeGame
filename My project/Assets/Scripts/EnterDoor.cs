using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterDoor : MonoBehaviour {

	[SerializeField]
	string direction;

	void OnCollisionEnter2D(Collision2D col) {  //check for colision with the object

		if (col.gameObject.tag == "Player") {   // if colided with the player

			GameObject dungeon = GameObject.FindGameObjectWithTag ("Dungeon");  //finds the Dungeon object
			DungeonGeneration dungeonGeneration = dungeon.GetComponent<DungeonGeneration> ();  //gets the object
			Room room = dungeonGeneration.CurrentRoom ();  //gets the current room
			dungeonGeneration.MoveToRoom (room.Neighbor (this.direction));  //move to the neighboor room in the directiokn of the door
			SceneManager.LoadScene ("Demo");  //loads the scene
		
		}
	}
}
