using UnityEngine;
using System.Collections;

public class ItemsList : MonoBehaviour
{
	[System.Serializable]
	public class ItemInfo
	{
		public GameObject item;
		public BIOMES biome;
		public int maxSpawnHeightOffFloor;
		public bool spawnOnce;
		public bool DoNotDestroy;
		[HideInInspector]
		public int
			spawnCount;
	}

	public ItemInfo[] ItemInfoList;
}
