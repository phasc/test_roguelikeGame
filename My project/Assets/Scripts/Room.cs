using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room
{
	public Vector2Int roomCoordinate;   //coordinates of the room

	public Dictionary<string, Room> neighbors;  //neighboors of the room, ordered by direction

	private string[,] population;  //what is in the room

	private Dictionary<string, GameObject> name2Prefab;  //pre-fabricated layout of the room

	public Room (int xCoordinate, int yCoordinate)   //defines the room (like a __init__)
	{
		this.roomCoordinate = new Vector2Int (xCoordinate, yCoordinate);
		this.neighbors = new Dictionary<string, Room> ();
		this.population = new string[18, 10];
		for (int xIndex = 0; xIndex < 18; xIndex += 1) {
			for (int yIndex = 0; yIndex < 10; yIndex += 1) {
				this.population [xIndex, yIndex] = "";
			}
		}
		this.population [8, 5] = "Player";
		this.name2Prefab = new Dictionary<string, GameObject> ();
	}

	public Room (Vector2Int roomCoordinate)  //same thing but redefined to be able to pass a vector2 instead of 2 coordinates (C# is weird)
	{
		this.roomCoordinate = roomCoordinate;
		this.neighbors = new Dictionary<string, Room> ();
		this.population = new string[18, 10];
		for (int xIndex = 0; xIndex < 18; xIndex += 1) {
			for (int yIndex = 0; yIndex < 10; yIndex += 1) {
				this.population [xIndex, yIndex] = "";
			}
		}
		this.population [8, 5] = "Player";
		this.name2Prefab = new Dictionary<string, GameObject> ();
	}

	public List<Vector2Int> NeighborCoordinates () {  //add the coordinates of the neighboors of the room (Up, Right, Down, Left)
		List<Vector2Int> neighborCoordinates = new List<Vector2Int> ();
		neighborCoordinates.Add (new Vector2Int(this.roomCoordinate.x, this.roomCoordinate.y - 1));
		neighborCoordinates.Add (new Vector2Int(this.roomCoordinate.x + 1, this.roomCoordinate.y));
		neighborCoordinates.Add (new Vector2Int(this.roomCoordinate.x, this.roomCoordinate.y + 1));
		neighborCoordinates.Add (new Vector2Int(this.roomCoordinate.x - 1, this.roomCoordinate.y));

		return neighborCoordinates;
	}

	public void Connect (Room neighbor) {  //adds a neighboor if there is a connection on that direction
		string direction = "";
		if (neighbor.roomCoordinate.y < this.roomCoordinate.y) {
			direction = "N";
		}
		if (neighbor.roomCoordinate.x > this.roomCoordinate.x) {
			direction = "E";
		}   
		if (neighbor.roomCoordinate.y > this.roomCoordinate.y) {
			direction = "S";
		}
		if (neighbor.roomCoordinate.x < this.roomCoordinate.x) {
			direction = "W";
		}
		this.neighbors.Add (direction, neighbor);
	}

	public string PrefabName () {  //gets the name of the pre-fabricated layout of the room (standalized to Room_<Connections direction>)
		string name = "Room_";
		foreach (KeyValuePair<string, Room> neighborPair in neighbors) {
			name += neighborPair.Key;
		}
		return name;
	}

	public Room Neighbor (string direction) {  //returns the neighboor given the direction
		return this.neighbors [direction];
	}

	public void PopulateObstacles (int numberOfObstacles, Vector2Int[] possibleSizes) {

		//places a certain number of obstacles in the room in random positions by checking the positions that are free
		for (int obstacleIndex = 0; obstacleIndex < numberOfObstacles; obstacleIndex += 1) {
			int sizeIndex = Random.Range (0, possibleSizes.Length);
			Vector2Int regionSize = possibleSizes [sizeIndex];
			List<Vector2Int> region = FindFreeRegion (regionSize);
			foreach (Vector2Int coordinate in region) {
				this.population [coordinate.x, coordinate.y] = "Obstacle";
			}
		}
	}

	public void PopulatePrefabs (int numberOfPrefabs, GameObject[] possiblePrefabs) {

		//places the gameobjects based on the pre-fabricated layout of the room
		for (int prefabIndex = 0; prefabIndex < numberOfPrefabs; prefabIndex += 1) {
			int choiceIndex = Random.Range (0, possiblePrefabs.Length);
			GameObject prefab = possiblePrefabs [choiceIndex];
			List<Vector2Int> region = FindFreeRegion (new Vector2Int(1, 1));

			this.population [region[0].x, region[0].y] = prefab.name;
			this.name2Prefab [prefab.name] = prefab;
		}
	}

	private List<Vector2Int> FindFreeRegion (Vector2Int sizeInTiles) {

		//finds a random free region in the room 

		List<Vector2Int> region = new List<Vector2Int>();
		do {
			region.Clear();

			Vector2Int centerTile = new Vector2Int(UnityEngine.Random.Range(2, 18 - 3), UnityEngine.Random.Range(2, 10 - 3));

			region.Add(centerTile);

			int initialXCoordinate = (centerTile.x - (int)Mathf.Floor(sizeInTiles.x / 2));
			int initialYCoordinate = (centerTile.y - (int)Mathf.Floor(sizeInTiles.y / 2));
			for (int xCoordinate = initialXCoordinate; xCoordinate < initialXCoordinate + sizeInTiles.x; xCoordinate += 1) {
				for (int yCoordinate = initialYCoordinate; yCoordinate < initialYCoordinate + sizeInTiles.y; yCoordinate += 1) {
					region.Add(new Vector2Int(xCoordinate, yCoordinate));
				}
			}
		} while(!IsFree (region));
		return region;
	}

	private bool IsFree (List<Vector2Int> region) {

		// checks if there is anything populating a region of the room

		foreach (Vector2Int tile in region) {
			if (this.population [tile.x, tile.y] != "") {
				return false;
			}
		}
		return true;
	}

	public void AddPopulationToTilemap (Tilemap tilemap, TileBase obstacleTile) {

		//fills the room with tiles (floors and wall) based on the pre-fabricated layout of the room

		for (int xIndex = 0; xIndex < 18; xIndex += 1) {
			for (int yIndex = 0; yIndex < 10; yIndex += 1) {
				if (this.population [xIndex, yIndex] == "Obstacle") {
					tilemap.SetTile (new Vector3Int (xIndex - 9, yIndex - 5, 0), obstacleTile);
				} else if (this.population [xIndex, yIndex] != "" && this.population [xIndex, yIndex] != "Player") {
					GameObject prefab = GameObject.Instantiate (this.name2Prefab[this.population [xIndex, yIndex]]);
					prefab.transform.position = new Vector2 (xIndex - 9 + 0.5f, yIndex - 5 + 0.5f);
				}
			}
		}
	}
}

