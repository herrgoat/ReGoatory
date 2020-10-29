﻿using System.Collections;
using System.Collections.Generic;
using Bomberfox;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
	private readonly Vector3[] directions =
	{
		Vector3.up, 
		Vector3.up + Vector3.right, 
		Vector3.right, 
		Vector3.right + Vector3.down,
		Vector3.down, 
		Vector3.down + Vector3.left,
		Vector3.left,
		Vector3.left + Vector3.up
	};
	private readonly Vector3[] directionsSimple =
	{
		Vector3.up,
		Vector3.right,
		Vector3.down,
		Vector3.left,
	};

	private Transform blocksParent = null;
	private Transform obstaclesParent = null;
	private Transform enemiesParent = null;

	// bottom left and top right corners to limit creation loops
	private Vector3Int min = new Vector3Int(-7, -4, 0);
	private Vector3Int max = new Vector3Int(7, 4, 0);

	private int blocksLevel = 0;
	private int obstaclesLevel = 0;
	private int randomBlockChance = 50;
	private int randomObstacleChance = 75;
	private List<Vector3> freePositions = new List<Vector3>();

	[SerializeField] private GameObject[] blockPrefabs = null;
	[SerializeField] private GameObject[] obstaclePrefabs = null;
	[SerializeField] private GameObject[] enemyPrefabs = null;
	[SerializeField] private GameObject playerPrefab = null;

	private void Awake()
	{
		FindParentObjects();
		freePositions.Clear();
	}

	private void Start()
	{
		CalculateBuildParameters(GameManager.Instance.CurrentLevel);
		CreateFreePositions();
		CreatePlayer();
		CreateNormalBlocks();
		CreateRandomBlocks(randomBlockChance);
		CheckForDeadEnds();
		CreateObstacles(randomObstacleChance);
		CreateKeyObstacle();
	}

	private void CalculateBuildParameters(int currentLevel)
	{
		// change build parameters according to current level
	}

	private void FindParentObjects()
	{
		blocksParent = GameObject.Find("Blocks").transform;
		obstaclesParent = GameObject.Find("Obstacles").transform;
		enemiesParent = GameObject.Find("Enemies").transform;

		if (blocksParent == null || obstaclesParent == null || enemiesParent == null)
		{
			Debug.Log("One of the required parent GameObjects is missing from scene.");
		}
	}

	/// <summary>
	/// Populate the free positions list with all coordinates
	/// </summary>
	private void CreateFreePositions()
	{
		for (int y = min.y; y <= max.y; y++)
		{
			for (int x = min.x; x <= max.x; x++)
			{
				Vector3 pos = new Vector3(x, y);
				freePositions.Add(pos);
			}
		}
	}

	/// <summary>
	/// Instantiate the player to the position of a random child of this game object
	/// and remove the child positions from free positions.
	/// </summary>
	private void CreatePlayer()
	{
		// create player
		Quaternion q = Quaternion.identity;
		Vector3 v = transform.GetChild(Random.Range(0, transform.childCount)).position;
		Instantiate(playerPrefab, v, q, null);

		// remove player start locations from free positions
		for (int i = 0; i < transform.childCount; i++)
		{
			freePositions.Remove(transform.GetChild(i).position);
		}
	}

	/// <summary>
	/// Instantiate blocks in rows and columns along the edges of the level with empty spaces in between and
	/// remove each location from free positions.
	/// </summary>
	private void CreateNormalBlocks()
	{
		// copy collection because we can't remove one from original while iterating over it
		Vector3[] freePositionsCopy = freePositions.ToArray();

		foreach (Vector3 v in freePositionsCopy)
		{
			if ((v.x % 2 == 0 && Mathf.Abs(v.y) == 3) 
			    || (Mathf.Abs(v.y) == 1) && Mathf.Abs(v.x) == 6)
			{
				Quaternion q = Quaternion.identity;
				Instantiate(blockPrefabs[blocksLevel], v, q, blocksParent);
				freePositions.Remove(v);
			}
		}
	}

	/// <summary>
	/// Create blocks at random and remove them from the free positions. Doesn't create a block if there are
	/// more than 2 blocks around a location. Removes from free positions if block instantiated.
	/// </summary>
	/// <param name="chance">the percentage chance to create a block</param>
	private void CreateRandomBlocks(int chance)
	{
		Vector3[] freePositionsCopy = freePositions.ToArray();

		foreach (Vector3 v in freePositionsCopy)
		{
			if (Random.Range(0, 101) < chance && LocationNotSurrounded(v))
			{
				Quaternion q = Quaternion.identity;
				Instantiate(blockPrefabs[blocksLevel], v, q, blocksParent);
				freePositions.Remove(v);
			}
		}
	}

	/// <summary>
	/// Checks if any location in level is surrounded by blocks in 4 directions. If found, removes one at random.
	/// </summary>
	private void CheckForDeadEnds()
	{
		for (int y = min.y; y <= max.y; y++)
		{
			for (int x = min.x; x <= max.x; x++)
			{
				if (CheckPositionForDeadEnd(new Vector3(x, y)))
				{
					RemoveOneFromAround(new Vector3(x, y));
				}
			}
		}
	}

	private bool CheckPositionForDeadEnd(Vector3 pos)
	{
		int numberOfBlocks = 0;

		foreach (Vector3 dir in directionsSimple)
		{
			Collider2D collider = Physics2D.OverlapPoint(pos + dir);
			
			if (collider != null && collider.gameObject.CompareTag("Block")) numberOfBlocks++;
		}

		return numberOfBlocks == 4;
	}

	private void RemoveOneFromAround(Vector3 pos)
	{
		Vector3 randomDir = directionsSimple[Random.Range(0, directionsSimple.Length)];
		Collider2D collider = Physics2D.OverlapPoint(pos + randomDir);
		freePositions.Add(collider.gameObject.transform.position);
		Destroy(collider.gameObject);
	}

	/// <summary>
	/// Creates obstacles at random to free positions and removes it from the list.
	/// </summary>
	/// <param name="chance">Percentage chance to create an obstacle</param>
	private void CreateObstacles(int chance)
	{
		Vector3[] freePositionsCopy = freePositions.ToArray();

		foreach (Vector3 v in freePositionsCopy)
		{
			if (Random.Range(0, 101) < chance)
			{
				Quaternion q = Quaternion.identity;
				Instantiate(obstaclePrefabs[obstaclesLevel], v, q, obstaclesParent);
				freePositions.Remove(v);
			}
		}
	}

	public void CreateKeyObstacle()
	{
		GameObject[] allObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
		GameObject keyObstacle = allObstacles[Random.Range(0, allObstacles.Length)];
		keyObstacle.GetComponent<Obstacle>().IsKey = true;
		Debug.Log("Key was hidden in " + keyObstacle.transform.position);

		keyObstacle.GetComponentInChildren<SpriteRenderer>().color = Color.red;
	}

	private bool LocationNotSurrounded(Vector3 v)
	{
		int blockedDirections = 0;

		foreach (Vector3 dir in directions)
		{
			if (!freePositions.Contains(v + dir)) blockedDirections++;

			if (blockedDirections > 2) return false;
		}

		return true;
	}
}
