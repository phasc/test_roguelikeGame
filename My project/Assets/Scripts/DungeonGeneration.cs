using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGeneration : MonoBehaviour {

	[SerializeField]
	private int numberOfRooms;  //number of rooms in the map

	[SerializeField]
	private int numberOfObstacles;  //number of obstacles in each room
	[SerializeField]
	private Vector2Int[] possibleObstacleSizes;  //possible x and y sizes for the obstacles

	[SerializeField]
	private GameObject goalPrefab;  // pre-fabricated object for the exit to the level

	[SerializeField]
	private TileBase obstacleTile;  // pre-fabricated sprite for the visual of the obstacles

	private Room[,] rooms;  //list of rooms

	private Room currentRoom;  //room the player is currently in

	private static DungeonGeneration instance = null;  // reference back to the class for instanciation

	void Awake () {

		//checks if the code is running for the first time and initializes the instance if it is, if not, restarts the dungeon

		if (instance == null) {
			DontDestroyOnLoad (this.gameObject);
			instance = this;
			this.currentRoom = GenerateDungeon ();
		} else {
			string roomPrefabName = instance.currentRoom.PrefabName ();
			GameObject roomObject = (GameObject) Instantiate (Resources.Load (roomPrefabName));
			Tilemap tilemap = roomObject.GetComponentInChildren<Tilemap> ();
			instance.currentRoom.AddPopulationToTilemap (tilemap, instance.obstacleTile);
			Destroy (this.gameObject);
		}
	}

	void Start () {

		//creates a new level

		string roomPrefabName = this.currentRoom.PrefabName ();
		GameObject roomObject = (GameObject) Instantiate (Resources.Load (roomPrefabName));
		Tilemap tilemap = roomObject.GetComponentInChildren<Tilemap> ();
		this.currentRoom.AddPopulationToTilemap (tilemap, this.obstacleTile);
	}

	private Room GenerateDungeon() {

		//main function that generates the level object

		int gridSize = 3 * numberOfRooms;

		rooms = new Room[gridSize, gridSize];

		Vector2Int initialRoomCoordinate = new Vector2Int ((gridSize / 2) - 1, (gridSize / 2) - 1);

		//queue the rooms to be creates
		Queue<Room> roomsToCreate = new Queue<Room> ();
		roomsToCreate.Enqueue (new Room(initialRoomCoordinate.x, initialRoomCoordinate.y));
		List<Room> createdRooms = new List<Room> ();

		//creates the rooms from the queue and add them as neighboors
		while (roomsToCreate.Count > 0 && createdRooms.Count < numberOfRooms) {
			Room currentRoom = roomsToCreate.Dequeue ();
			this.rooms [currentRoom.roomCoordinate.x, currentRoom.roomCoordinate.y] = currentRoom;
			createdRooms.Add (currentRoom);
			AddNeighbors (currentRoom, roomsToCreate);
		}


		int maximumDistanceToInitialRoom = 0;
		Room finalRoom = null;
		foreach (Room room in createdRooms) {
			//makes sure the rooms are connected
			List<Vector2Int> neighborCoordinates = room.NeighborCoordinates ();
			foreach (Vector2Int coordinate in neighborCoordinates) {
				Room neighbor = this.rooms [coordinate.x, coordinate.y];
				if (neighbor != null) {
					room.Connect (neighbor);
				}
			}
			//populates them with obstacles 
			room.PopulateObstacles (this.numberOfObstacles, this.possibleObstacleSizes);

			//sets up the final room based on the distance to the initial room
			int distanceToInitialRoom = Mathf.Abs (room.roomCoordinate.x - initialRoomCoordinate.x) + Mathf.Abs(room.roomCoordinate.y - initialRoomCoordinate.y);
			if (distanceToInitialRoom > maximumDistanceToInitialRoom) {
				maximumDistanceToInitialRoom = distanceToInitialRoom;
				finalRoom = room;
			}
		}

		//puts the exit in the final room
		GameObject[] goalPrefabs = { this.goalPrefab };
		finalRoom.PopulatePrefabs(1, goalPrefabs);

		return this.rooms [initialRoomCoordinate.x, initialRoomCoordinate.y];
	}



	private void AddNeighbors(Room currentRoom, Queue<Room> roomsToCreate) {

		//adds the neighboors to the a room

		//checks how many neighboor can this room have
		List<Vector2Int> neighborCoordinates = currentRoom.NeighborCoordinates ();
		List<Vector2Int> availableNeighbors = new List<Vector2Int> ();
		foreach (Vector2Int coordinate in neighborCoordinates) {
			if (this.rooms[coordinate.x, coordinate.y] == null) {
				availableNeighbors.Add (coordinate);
			}
		}
		
		//gives the room a random number of neighboors
		int numberOfNeighbors = (int)Random.Range (1, availableNeighbors.Count);
		for (int neighborIndex = 0; neighborIndex < numberOfNeighbors; neighborIndex++) {
			float randomNumber = Random.value;
			float roomFrac = 1f / (float)availableNeighbors.Count;
			Vector2Int chosenNeighbor = new Vector2Int(0, 0);
			foreach (Vector2Int coordinate in availableNeighbors) {
				if (randomNumber < roomFrac) {
					chosenNeighbor = coordinate;
					break;
				} else {
					roomFrac += 1f / (float)availableNeighbors.Count;
				}
			}
			roomsToCreate.Enqueue (new Room(chosenNeighbor));
			availableNeighbors.Remove (chosenNeighbor);
		}
	}

	public void MoveToRoom(Room room) {  //updates the current room
		this.currentRoom = room;
	}

	public Room CurrentRoom() {  //gets the current room
		return this.currentRoom;
	}

	public void ResetDungeon() {  //restart the level
		this.currentRoom = GenerateDungeon ();
	}
		
}
