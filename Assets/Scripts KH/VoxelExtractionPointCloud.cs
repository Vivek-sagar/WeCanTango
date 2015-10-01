//#define DEBUG_THIS
//#define JITTER
#define USE_NORMALS
#define USE_UV
//#define USE_CHUNK_FRUSTUM_CULLING
//#define VOXEL_DELETION

using UnityEngine;
using System.Collections;


public static class VoxelConsts
{
	public static int CHUNK_SIZE = 8;
	public static int PT_THRES = 30;
	public static int VOXEL_RES = 10;
	public static int FRAME_THRES = 5;
	public static int DEL_FRAME_THRES = 7;
	public static int PT_DEL_THRES = 5;
	public static Vec3Int[] CardinalDir = new Vec3Int[]{ new Vec3Int(0,0,1), new Vec3Int(0,0,-1), new Vec3Int(-1,0,0), new Vec3Int(1,0,0), new Vec3Int(0,1,0), new Vec3Int(0,-1,0) };
	public static Vector3[] CardinalV3Dir = new Vector3[]{ new Vector3(0,0,1), new Vector3(0,0,-1), new Vector3(-1,0,0), new Vector3(1,0,0), new Vector3(0,1,0), new Vector3(0,-1,0) };
}

public class IndexStack<T>
{
	T[] array;
	int size;
	int count;
	
	public IndexStack(T[] _array)
	{
		size = _array.Length;
		array = _array;
		count = 0;
	}
	
	public void push(T v)
	{
		array [count++] = v;
	}
	
	public T pop()
	{
		return array[--count];
	}
	
	public T peek(int index)
	{
		return array [index];
	}
	
	public bool isEmpty()
	{
		return count == 0;
	}
	
	public int getCount()
	{
		return count;
	}
	
	public T[] getArray()
	{
		return array;
	}
	
	public void clear()
	{
		count = 0;
	}
}

public struct Vec3Int
{
	public int x,y,z;
	public Vec3Int(int _x, int _y, int _z)
	{
		x = _x;
		y = _y;
		z = _z;
	}

	public Vec3Int(Vector3 v)
	{
		x = Mathf.FloorToInt (v.x);
		y = Mathf.FloorToInt (v.y);
		z = Mathf.FloorToInt (v.z);
	}

	public Vector3 ToVec3()
	{
		return new Vector3 (x, y, z);
	}

	public static Vec3Int operator+(Vec3Int a, Vec3Int b)
	{
		return new Vec3Int(a.x + b.x, a.y + b.y, a.z + b.z); 
	}

	public static Vec3Int operator-(Vec3Int a, Vec3Int b)
	{
		return new Vec3Int(a.x - b.x, a.y - b.y, a.z - b.z); 
	}

	public static Vec3Int operator*(Vec3Int a, Vec3Int b)
	{
		return new Vec3Int(a.x * b.x, a.y * b.y, a.z * b.z); 
	}

	public static Vec3Int operator*(Vec3Int a, int b)
	{
		return new Vec3Int(a.x * b, a.y * b, a.z * b); 
	}

	public static Vec3Int operator/(Vec3Int a, Vec3Int b)
	{
		return new Vec3Int(a.x / b.x, a.y / b.y, a.z / b.z); 
	}

	public static Vec3Int operator/(Vec3Int a, int b)
	{
		return new Vec3Int(a.x / b, a.y / b, a.z / b); 
	}

	public static Vec3Int operator%(Vec3Int a, Vec3Int b)
	{
		return new Vec3Int(a.x % b.x, a.y % b.y, a.z % b.z); 
	}
	
	public static Vec3Int operator%(Vec3Int a, int b)
	{
		return new Vec3Int(a.x % b, a.y % b, a.z % b); 
	}
}

public enum VF : int
{
	VX_FRONT_SHOWN = 0,
	VX_BACK_SHOWN = 1,
	VX_LEFT_SHOWN = 2,
	VX_RIGHT_SHOWN = 3,
	VX_TOP_SHOWN = 4,
	VX_BOTTOM_SHOWN = 5,
	VX_OCCUPIED = 6,
	VX_RESERVED = 7
}


public enum DIR : int
{
	DIR_FRONT = 0,
	DIR_BACK = 1,
	DIR_LEFT = 2,
	DIR_RIGHT = 3,
	DIR_UP = 4,
	DIR_DOWN = 5,
}

public enum PLANES : int
{
	TOP = 0,
	BOTTOM = 1,
	LEFT = 2,
	RIGHT = 3,
	NEAR = 4,
	FAR = 5
}

public class Voxel
{
	//guaranteed flags = 0
	//public uint flags;
	public byte pcount = 0;
#if VOXEL_DELETION
	public byte dcount = 0;
#endif
	BitArray flags = new BitArray (8, false);

	public Voxel()
	{
	}

	public bool isOccupied()
	{
		return flags.Get((int)VF.VX_OCCUPIED);
	}

	public void insertPoint()
	{
		if (isOccupied())
			return;

		pcount++;

		if(pcount > VoxelConsts.PT_THRES)
		{
			flags.Set ((int)VF.VX_OCCUPIED, true);
		}
	}
#if VOXEL_DELETION
	public void removePoint()
	{
		if (!isOccupied())
			return;
		
		dcount++;
		
		if(dcount > VoxelConsts.PT_DEL_THRES)
		{
			flags.Set ((int)VF.VX_OCCUPIED, false);
		}
	}
#endif
	public void setUnOccupied()
	{
		pcount = 0;
		flags.Set ((int)VF.VX_OCCUPIED, false);
	}

	public void setFace(VF flag, bool visible)
	{
		flags.Set ((int)flag, visible);
	}

	public bool getFace(VF flag)
	{
		return flags.Get ((int)flag);
	}

	public void setInvalid()
	{
		setUnOccupied();
	}

	public void setReserved(bool val)
	{
		flags.Set ((int)VF.VX_RESERVED, val);
	}
}
public class ChunkTemplate
{
	public Vector3[] vertices;
	
#if USE_NORMALS
	public Vector3[] normals;
#if USE_UV
	public Vector2[] uvs;
#endif
#endif
	public float voxel_size;
	
	public int vertex_dim;
	public int vertex_count;

	private static ChunkTemplate instance;

	public static ChunkTemplate Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new ChunkTemplate();
			}
			return instance;
		}
	}
	
	private ChunkTemplate()
	{
		int chunk_size = (int)VoxelConsts.CHUNK_SIZE;

		voxel_size = 1.0f / VoxelConsts.VOXEL_RES;
	
		vertex_dim = chunk_size + 1;
		
		vertex_count = vertex_dim * vertex_dim * vertex_dim;
		
		vertices = new Vector3[vertex_count * 6];
		
#if USE_NORMALS
		normals = new Vector3[vertex_count * 6];
#if USE_UV
		uvs = new Vector2[vertex_count * 6];
#endif
#endif		
		
		for(int i=0;i<vertex_dim;i++)
			for(int j=0;j<vertex_dim;j++)
				for(int k=0;k<vertex_dim;k++)
			{
				Vector3 vert = ResizeVertex(new Vector3(i, j, k));
				setVertex(i,j,k,vert);
			}

	}

	private Vector3 ResizeVertex(Vector3 vert)
	{
		Vector3 newCoords = vert * voxel_size;
		return newCoords;
	}
	
	private int getIndex(int x, int y, int z)
	{
		return x * vertex_dim * vertex_dim + y * vertex_dim + z;
	}
	
	private void setVertex(int x, int y, int z, Vector3 vert)
	{
		#if USE_NORMALS
		vertices [getIndex(x,y,z) + getDirOffset(DIR.DIR_UP)] = vert;
		vertices [getIndex(x,y,z) + getDirOffset(DIR.DIR_DOWN)] = vert;
		vertices [getIndex(x,y,z) + getDirOffset(DIR.DIR_LEFT)] = vert;
		vertices [getIndex(x,y,z) + getDirOffset(DIR.DIR_RIGHT)] = vert;
		vertices [getIndex(x,y,z) + getDirOffset(DIR.DIR_BACK)] = vert;
		vertices [getIndex(x,y,z) + getDirOffset(DIR.DIR_FRONT)] = vert;
		
		normals [getIndex(x,y,z) + getDirOffset(DIR.DIR_UP)] = new Vector3(0,1,0);
		normals [getIndex(x,y,z) + getDirOffset(DIR.DIR_DOWN)] = new Vector3(0,-1,0);
		normals [getIndex(x,y,z) + getDirOffset(DIR.DIR_LEFT)] = new Vector3(-1,0,0);
		normals [getIndex(x,y,z) + getDirOffset(DIR.DIR_RIGHT)] = new Vector3(1,0,0);
		normals [getIndex(x,y,z) + getDirOffset(DIR.DIR_BACK)] = new Vector3(0,0,-1);
		normals [getIndex(x,y,z) + getDirOffset(DIR.DIR_FRONT)] = new Vector3(0,0,1);
		#if USE_UV
		uvs [getIndex(x,y,z) + getDirOffset(DIR.DIR_UP)] = new Vector2(x,z);
		uvs [getIndex(x,y,z) + getDirOffset(DIR.DIR_DOWN)] = new Vector2(x,z);
		uvs [getIndex(x,y,z) + getDirOffset(DIR.DIR_LEFT)] = new Vector2(z,y);
		uvs [getIndex(x,y,z) + getDirOffset(DIR.DIR_RIGHT)] = new Vector2(z,y);
		uvs [getIndex(x,y,z) + getDirOffset(DIR.DIR_BACK)] = new Vector2(x,y);
		uvs [getIndex(x,y,z) + getDirOffset(DIR.DIR_FRONT)] = new Vector2(x,y);
		#endif
		#else
		vertices [getIndex(x,y,z)] = vert;
		#endif
	}
	
	private int getDirOffset(DIR dir)
	{
		#if USE_NORMALS
		return (int)dir * vertex_count;
		#else
		return 0;
		#endif
	}

}

public class Chunks
{
	public Voxel[,,] voxels;
	public Mesh mesh;
	public IndexStack<int> istack;

	int[] indices;

	public bool dirty;
	public uint voxel_count = 0;

	public Chunks() 
	{
		int chunk_size = (int)VoxelConsts.CHUNK_SIZE;
		voxels = new Voxel[chunk_size,chunk_size,chunk_size];

		dirty = false;
		float voxel_size = 1.0f / VoxelConsts.VOXEL_RES;

		for (int i=0; i<chunk_size; i++)
			for (int j=0; j<chunk_size; j++)
				for (int k=0; k<chunk_size; k++)
					voxels [i, j, k] = new Voxel ();
		int vertex_dim = chunk_size + 1;
		
		int vertex_count = vertex_dim * vertex_dim * vertex_dim;
	
		indices = new int[vertex_count * 3];
		istack = new IndexStack<int> (indices);
	}

	public void init(Mesh _mesh)
	{
		mesh = _mesh;
		//mesh.MarkDynamic ();
		mesh.vertices = ChunkTemplate.Instance.vertices;
		//vertices = null;
#if USE_NORMALS
		mesh.normals = ChunkTemplate.Instance.normals;
		//normals = null;
		#if USE_UV
		mesh.uv = ChunkTemplate.Instance.uvs;
		//uvs = null;
		#endif
#endif

	}

	public Vector3 ResizeVertex(Vector3 vert)
	{
		Vector3 newCoords = vert * ChunkTemplate.Instance.voxel_size;
		return newCoords;
	}

	public int getIndex(int x, int y, int z)
	{
		return x * ChunkTemplate.Instance.vertex_dim * ChunkTemplate.Instance.vertex_dim + y * ChunkTemplate.Instance.vertex_dim + z;
	}

	public int getDirOffset(DIR dir)
	{
#if USE_NORMALS
		return (int)dir * ChunkTemplate.Instance.vertex_count;
#else
		return 0;
#endif
	}

	public bool isEmpty()
	{
		return voxel_count == 0;
	}
	
	public Voxel getVoxel(Vec3Int localCoords)
	{
		return voxels[localCoords.x, localCoords.y, localCoords.z];
	}
	
}

public class VoxelGrid
{
	public Chunks[,,] voxelGrid;
	public int NUM_CHUNKS_X;
	public int NUM_CHUNKS_Y;
	public int NUM_CHUNKS_Z;
	float voxel_size;



	public VoxelGrid(int _chunks_x, int _chunks_y, int _chunks_z)
	{
		voxelGrid = new Chunks[_chunks_x,_chunks_y,_chunks_z];
		voxel_size = 1.0f/VoxelConsts.VOXEL_RES;

		NUM_CHUNKS_X = _chunks_x;
		NUM_CHUNKS_Y = _chunks_y;
		NUM_CHUNKS_Z = _chunks_z;
	}

	public bool isChunkValid(Vec3Int chunkCoords)
	{
		return 	chunkCoords.x >= 0 && chunkCoords.y >= 0 && chunkCoords.z >= 0 &&
				chunkCoords.x < NUM_CHUNKS_X && chunkCoords.y < NUM_CHUNKS_Y && chunkCoords.z < NUM_CHUNKS_Z &&
				voxelGrid [chunkCoords.x, chunkCoords.y, chunkCoords.z] != null;//;
	}

	public Voxel getVoxel(Vec3Int coords)
	{
		Vec3Int localCoords = coords % (int)VoxelConsts.CHUNK_SIZE;
		Vec3Int chunkCoords = coords / (int)VoxelConsts.CHUNK_SIZE;

		if(isChunkValid(chunkCoords))
		{
			return voxelGrid[chunkCoords.x, chunkCoords.y, chunkCoords.z].getVoxel(localCoords);
		}
		else
		{
			return new Voxel();
		}
	}

	public bool isOccupied(Vec3Int coords)
	{
		Vec3Int localCoords = coords % (int)VoxelConsts.CHUNK_SIZE;
		Vec3Int chunkCoords = coords / (int)VoxelConsts.CHUNK_SIZE;
		
		if(isChunkValid(chunkCoords))
		{
			return voxelGrid[chunkCoords.x, chunkCoords.y, chunkCoords.z].getVoxel(localCoords).isOccupied();
		}
		else
		{
			return false;
		}

	}

	VF invertflag(VF flag)
	{
		int f = (int)flag;
		return f % 2 == 0 ? (VF)(f + 1) : (VF)(f - 1);
	}

	void setVoxelFaces(Voxel vx, Vec3Int coords)
	{
		Vector3 vec = coords.ToVec3 () + new Vector3(0.5f,0.5f,0.5f);
		for(int i=0;i<6;i++)
		{
			VF flag = (VF)i;
			Vector3 dir = VoxelConsts.CardinalV3Dir[i];
			
			Voxel neighbour = getVoxel(new Vec3Int(vec + dir));
			bool occupied = neighbour.isOccupied();

			if(occupied)
			{
				vx.setFace(flag,false);
				neighbour.setFace(invertflag(flag),false);
			}
			else
			{
				vx.setFace(flag,true);
				neighbour.setFace(invertflag(flag),true);
			}
		}
	}

	void unSetVoxelFaces(Voxel vx, Vec3Int coords)
	{
		Vector3 vec = coords.ToVec3 () + new Vector3(0.5f,0.5f,0.5f);
		for(int i=0;i<6;i++)
		{
			VF flag = (VF)i;
			Vector3 dir = VoxelConsts.CardinalV3Dir[i];
			
			Voxel neighbour = getVoxel(new Vec3Int(vec + dir));
			bool occupied = neighbour.isOccupied();
			
			if(occupied)
			{
				vx.setFace(flag,true);
				neighbour.setFace(invertflag(flag),true);
			}
			else
			{
				vx.setFace(flag,false);
				neighbour.setFace(invertflag(flag),false);
			}
		}
	}

	public void setVoxel(Vec3Int coords)
	{
		Vec3Int localCoords = coords % (int)VoxelConsts.CHUNK_SIZE;
		Vec3Int chunkCoords = coords / (int)VoxelConsts.CHUNK_SIZE;
		
		if(isChunkValid(chunkCoords))
		{
			Chunks chunk = voxelGrid[chunkCoords.x, chunkCoords.y, chunkCoords.z];
			Voxel vx = chunk.getVoxel(localCoords);

			if(vx.isOccupied())
				return;

			vx.insertPoint();
			if(vx.isOccupied())
			{
				setVoxelFaces(vx,coords);
				chunk.voxel_count++;
				chunk.dirty = true;
			}
		}
	}

	public void unSetVoxel(Vec3Int coords)
	{
		Vec3Int localCoords = coords % (int)VoxelConsts.CHUNK_SIZE;
		Vec3Int chunkCoords = coords / (int)VoxelConsts.CHUNK_SIZE;
		
		if(isChunkValid(chunkCoords))
		{
			Chunks chunk = voxelGrid[chunkCoords.x, chunkCoords.y, chunkCoords.z];
			if(chunk == null)
				return;
			Voxel vx = chunk.getVoxel(localCoords);

			if(!vx.isOccupied())
				return;

			vx.setUnOccupied();
			unSetVoxelFaces(vx,coords);
			chunk.voxel_count--;
			chunk.dirty = true;
		}
	}

	public void unSetFast(Voxel vx,Vec3Int coords, Chunks chunk)
	{
		vx.setUnOccupied();
		unSetVoxelFaces(vx,coords);
		chunk.voxel_count--;
		chunk.dirty = true;
	}
}

public class ChunkPool
{
	Chunks[] chunks;
	int max_chunks;
	int num_alloced = 0;
	private object lockthis = new object ();
	public ChunkPool(int numchunks)
	{
		chunks = new Chunks[numchunks];

		for (int i=0; i<numchunks; i++)
			chunks [i] = new Chunks ();

		max_chunks = numchunks;
	}

	public Chunks allocNew()
	{
		//lock (lockthis) 
		//{
		if(num_alloced < max_chunks)
			return chunks[num_alloced++];
		else return new Chunks();
		//}
	}

	public int getNumAlloced()
	{
		return num_alloced;
	}
}

public class VoxelExtractionPointCloud : Singleton<VoxelExtractionPointCloud>
{
	public int num_chunks_x = 30;
	public int num_chunks_y = 8;
	public int num_chunks_z = 30;
	public GameObject ChunkInstance;
	static int framecount = 0;

	Vector3 offset;
	float scale;
	
	public int num_voxels_x;
	public int num_voxels_y;
	public int num_voxels_z;

	int num_verts_x;
	int num_verts_y;
	int num_verts_z;
	
	public float voxel_size;
	
	public VoxelGrid grid;

    [HideInInspector]
	public GameObject[,,] chunkGameObjects;

	ChunkPool pool;
	private object lockthis = new object(); 

	int vertex_count;
	public int chunk_size;
	//Vector3 jitter;
	public string debugString;
	public Camera camera;

	Matrix4x4 MVP = Matrix4x4.identity;


#if DEBUG_THIS
	public bool fakeData;
	public TangoPointCloud debugPtCloud;
#endif
	// Use this for initialization
	void Start () 
	{
		chunk_size = (int)VoxelConsts.CHUNK_SIZE;

		num_voxels_x = chunk_size * num_chunks_x;
		num_voxels_y = chunk_size * num_chunks_y;
		num_voxels_z = chunk_size * num_chunks_z;

		num_verts_x = num_voxels_x + 1;
		num_verts_y = num_voxels_y + 1;
		num_verts_z = num_voxels_z + 1;

		offset = new Vector3 (((float)num_voxels_x) / 2.0f, ((float)num_voxels_y) / 2.0f, ((float)num_voxels_z) / 2.0f);
		scale = VoxelConsts.VOXEL_RES;

		voxel_size = 1.0f / scale;

		grid = new VoxelGrid (num_chunks_x, num_chunks_y, num_chunks_z);
		chunkGameObjects = new GameObject[num_chunks_x, num_chunks_y, num_chunks_z];

		pool = new ChunkPool (1500);

		for(int x=0;x<num_chunks_x;x++)
			for(int y=0;y<num_chunks_y;y++)
				for(int z=0;z<num_chunks_z;z++)
			{
				chunkGameObjects[x,y,z] = (GameObject)Instantiate(ChunkInstance,(new Vector3(x * chunk_size, y * chunk_size,z * chunk_size) - offset) * voxel_size,Quaternion.identity);
				chunkGameObjects[x,y,z].GetComponent<MeshFilter>().mesh = new Mesh();
				chunkGameObjects[x,y,z].GetComponent<MeshRenderer>().enabled = false;
			}

		ChunkInstance.SetActive (false);
	}

	bool isInFrustum(Vector3 p, ref Matrix4x4 MVP) 
	{
		Vector4 clip = MVP * new Vector4(p.x,p.y,p.z,1.0f);
		return Mathf.Abs(clip.x) < clip.w && Mathf.Abs(clip.y) < clip.w && clip.z > 0 && clip.z < clip.w;
	}

#if USE_CHUNK_FRUSTUM_CULLING
	bool isChunkInFrustum(Vec3Int cc)
	{
		for(uint i=0;i<8;i++)
		{
			Vec3Int corner = new Vec3Int((int)(i & 1), (int)((i & 2) >> 1), (int)((i & 4) >> 2));
			Vec3Int point = (cc + corner) * chunk_size;
			Vector3 wpoint = FromGrid(point);
			if(isInFrustum(wpoint, ref MVP))
				return true;
		}
		
		return false;
	}
#endif

	void renderVoxelGrid()
	{
		
		int timeslice = framecount % VoxelConsts.FRAME_THRES;
		int del_timeslice = framecount % VoxelConsts.DEL_FRAME_THRES;

		for(int i=0;i<num_chunks_x;i++)
			for(int j=0;j<num_chunks_y;j++)
				for(int k=0;k<num_chunks_z;k++)
			{
				Chunks chunk = grid.voxelGrid[i,j,k];
			
				if(chunk == null)
					continue;
				if(chunk.isEmpty())
				{
					chunkGameObjects [i,j,k].GetComponent<MeshRenderer>().enabled = false;
					continue;
				}
				Vec3Int chunkcoords = new Vec3Int(i,j,k);

				chunk.istack.clear ();

				
				//if((camera.transform.position - chunkGameObjects [i,j,k].transform.position).sqrMagnitude > 64)
				//	continue;

				if(chunk.dirty)
				{
					if(chunk.mesh == null)
					{
						chunk.init(chunkGameObjects [i,j,k].GetComponent<MeshFilter>().mesh);
						chunkGameObjects [i,j,k].GetComponent<MeshRenderer>().enabled = true;
					}


					for(int x=0;x<chunk_size;x++)
						for(int y=0;y<chunk_size;y++)
							for(int z=0;z<chunk_size;z++)
						{
							Vec3Int vcoord = new Vec3Int(x,y,z);
							Voxel voxel = chunk.getVoxel(vcoord);

							if(timeslice == 0)
							{
								voxel.pcount = 0;

							}
#if VOXEL_DELETION
							if(del_timeslice == 0)
							{
								voxel.dcount = 0;
							}
#endif
							if(voxel.isOccupied())
							{
								//front
								if(voxel.getFace(VF.VX_FRONT_SHOWN))
								{
									//front
									chunk.istack.push(chunk.getIndex(x,y,z + 1) + chunk.getDirOffset(DIR.DIR_FRONT));
									chunk.istack.push(chunk.getIndex(x + 1,y,z + 1)+ chunk.getDirOffset(DIR.DIR_FRONT));
									chunk.istack.push(chunk.getIndex(x + 1,y + 1,z + 1)+ chunk.getDirOffset(DIR.DIR_FRONT));
									chunk.istack.push(chunk.getIndex(x,y + 1,z + 1)+ chunk.getDirOffset(DIR.DIR_FRONT));
								}

								if(voxel.getFace(VF.VX_RIGHT_SHOWN))
								{
									//right
									chunk.istack.push(chunk.getIndex(x+1,y,z)+ chunk.getDirOffset(DIR.DIR_RIGHT));
									chunk.istack.push(chunk.getIndex(x+1,y+1,z)+ chunk.getDirOffset(DIR.DIR_RIGHT));
									chunk.istack.push(chunk.getIndex(x+1,y + 1,z + 1)+ chunk.getDirOffset(DIR.DIR_RIGHT));
									chunk.istack.push(chunk.getIndex(x+1,y,z+1)+ chunk.getDirOffset(DIR.DIR_RIGHT));
								}

								if(voxel.getFace(VF.VX_BACK_SHOWN))
								{
									//back
									chunk.istack.push(chunk.getIndex(x,y,z) + chunk.getDirOffset(DIR.DIR_BACK));
									chunk.istack.push(chunk.getIndex(x,y + 1,z) + chunk.getDirOffset(DIR.DIR_BACK));
									chunk.istack.push(chunk.getIndex(x + 1,y + 1,z) + chunk.getDirOffset(DIR.DIR_BACK));
									chunk.istack.push(chunk.getIndex(x + 1,y,z) + chunk.getDirOffset(DIR.DIR_BACK));
								}

								if(voxel.getFace(VF.VX_LEFT_SHOWN))
								{
									//left
									chunk.istack.push(chunk.getIndex(x,y,z)+ chunk.getDirOffset(DIR.DIR_LEFT));
									chunk.istack.push(chunk.getIndex(x,y,z + 1)+ chunk.getDirOffset(DIR.DIR_LEFT));
									chunk.istack.push(chunk.getIndex(x,y + 1,z + 1)+ chunk.getDirOffset(DIR.DIR_LEFT));
									chunk.istack.push(chunk.getIndex(x,y + 1,z)+ chunk.getDirOffset(DIR.DIR_LEFT));
								}

								if(voxel.getFace(VF.VX_TOP_SHOWN))
								{
									//top
									chunk.istack.push(chunk.getIndex(x,y+1,z)+ chunk.getDirOffset(DIR.DIR_UP));
									chunk.istack.push(chunk.getIndex(x,y+1,z+1)+ chunk.getDirOffset(DIR.DIR_UP));
									chunk.istack.push(chunk.getIndex(x+1,y+1,z+1)+ chunk.getDirOffset(DIR.DIR_UP));
									chunk.istack.push(chunk.getIndex(x+1,y+1,z)+ chunk.getDirOffset(DIR.DIR_UP));
								}

								if(voxel.getFace(VF.VX_BOTTOM_SHOWN))
								{
									//bottom
									chunk.istack.push(chunk.getIndex(x,y,z)+ chunk.getDirOffset(DIR.DIR_DOWN));
									chunk.istack.push(chunk.getIndex(x+1,y,z)+ chunk.getDirOffset(DIR.DIR_DOWN));
									chunk.istack.push(chunk.getIndex(x+1,y,z+1)+ chunk.getDirOffset(DIR.DIR_DOWN));
									chunk.istack.push(chunk.getIndex(x,y,z+1)+ chunk.getDirOffset(DIR.DIR_DOWN));
								}
							}

						}

					int[] indexArray = new int[chunk.istack.getCount()];
					System.Array.Copy(chunk.istack.getArray(),indexArray,chunk.istack.getCount());

					chunk.mesh.SetIndices (indexArray , MeshTopology.Quads, 0);
					//chunk.mesh.RecalculateBounds();
					chunk.dirty = false;

				}

			}
			framecount++;
	}

	public Vec3Int ToGrid(Vector3 pt)
	{
		Vector3 p = pt;
		p = ToGridUnTrunc (p);
		Vec3Int vec = new Vec3Int (Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z));
		return vec;
	}

	public Vector3 ToGridUnTrunc(Vector3 pt)
	{
		Vector3 p = pt;

		p *= scale;
		p += offset;


		return p;
	}

	public Vector3 FromGridUnTrunc(Vector3 pt)
	{
		Vector3 p = pt;
		p -= offset;
		p *= voxel_size;
		
		return p;
	}

	public Vector3 FromGrid(Vec3Int pt)
	{
		Vector3 p = new Vector3 (pt.x, pt.y, pt.z);
		p = FromGridUnTrunc (p);

		return p;
	}

	//optimize later
	public bool RayCast(Vector3 start, Vector3 dir, float dist, ref Vector3 vxcood, ref Vector3 normal)
	{
		Vector3 pt = ToGridUnTrunc (start);
		dir = dir.normalized;

		for(float i=0;i<dist;i+=1.0f)
		{
			Vec3Int cvCoord = new Vec3Int(pt);
			Voxel vx = grid.getVoxel(cvCoord);

			if(vx.isOccupied())
			{
				vxcood = FromGridUnTrunc(cvCoord.ToVec3() + new Vector3(0.5f,0.5f,0.5f));
				Vector3 _normal = new Vector3();

				for(int j=0;j<6;j++)
				{
					VF flag = (VF)j;
					if(vx.getFace(flag) && Vector3.Dot (dir,VoxelConsts.CardinalV3Dir[j]) < 0)
					{
						_normal += VoxelConsts.CardinalV3Dir[j];
					}
				}

				normal = _normal.normalized;

				return true;
			}

			pt += dir;
		}

		return false;
	}

	//optimize later
	public bool OccupiedRayCast(Vector3 start, Vector3 dir, float dist, ref Vector3 vxcood, ref Vector3 normal)
	{
		Vector3 pt = ToGridUnTrunc (start);
		dir = dir.normalized;
		
		for(float i=0;i<dist;i+=1.0f)
		{
			Vec3Int cvCoord = new Vec3Int(pt);
			Voxel vx = grid.getVoxel(cvCoord);
			
			if(!vx.isOccupied())
			{
				Voxel ovx = grid.getVoxel(new Vec3Int(pt - dir));

				vxcood = FromGridUnTrunc(cvCoord.ToVec3() + new Vector3(0.5f,0.5f,0.5f));
				Vector3 _normal = new Vector3();
				
				for(int j=0;j<6;j++)
				{
					VF flag = (VF)j;
					if(ovx.getFace(flag) && Vector3.Dot (dir,VoxelConsts.CardinalV3Dir[j]) < 0)
					{
						_normal += VoxelConsts.CardinalV3Dir[j];
					}
				}
				
				normal = _normal.normalized;
				
				return true;
			}
			
			pt += dir;
		}
		
		return false;
	}

#if VOXEL_DELETION
	public void KillerRayCast(Vector3 start)
	{
		const float step = 0.5f;
		Vector3 end = camera.transform.position;

		Vector3 vstart = ToGridUnTrunc (start);
		Vector3 vend = ToGridUnTrunc (end);
		Vector3 dir = (vend - vstart).normalized;
		float mag = (vend - vstart).magnitude;
		float offset = mag * 0.3f + 1.0f;

		Vector3 pt = vstart + dir * offset;


		for(float i=offset;i<(mag - 4.0f);i+=step)
		{
			Vec3Int cvCoord = new Vec3Int(pt);
			Voxel vx = grid.getVoxel(cvCoord);
			Vec3Int cc = cvCoord / new Vec3Int(chunk_size,chunk_size,chunk_size);
			Chunks chunk = grid.voxelGrid[cc.x,cc.y,cc.z];

			if(vx.isOccupied())
			{
				vx.removePoint();
				if(!vx.isOccupied())
					grid.unSetFast(vx,cvCoord,chunk);
			}
			
			pt += dir * step;
		}
	}
#endif

	public void InstantiateChunkIfNeeded(Vec3Int coords)
	{
		Vec3Int cc = coords / VoxelConsts.CHUNK_SIZE;


		if (grid.voxelGrid [cc.x, cc.y, cc.z] == null) {
			grid.voxelGrid [cc.x, cc.y, cc.z] = pool.allocNew();
		}

	}

	public bool isChunkEmpty(Vec3Int coords)
	{
		Vec3Int cc = coords / (int)VoxelConsts.CHUNK_SIZE;
		return grid.voxelGrid [cc.x, cc.y, cc.z] == null;
	}

	float sqrtApprox(float inX)
	{
		int x = System.BitConverter.ToInt32(System.BitConverter.GetBytes(inX),0);
		x = 0x1FBD1DF5 + (x >> 1);
		return System.BitConverter.ToSingle(System.BitConverter.GetBytes(x),0);
	}

	public void addAndRender (TangoPointCloud pointCloud) 
	{
		int count = pointCloud.m_pointsCount;

#if VOXEL_DELETION
		Random.seed = framecount % 20;
		for (int i=0; i<50; i++) 
		{
			int index = Random.Range(0,count);
			Vector3 pt = pointCloud.m_points[index];
			Vector3 ranvec = new Vector3(Random.value - 0.5f,Random.value - 0.5f,Random.value - 0.5f) * voxel_size * 2;
			KillerRayCast(pt + ranvec);

		}
#endif

		for(int i=0; i< count; i++)
		{

			Vector3 pt = pointCloud.m_points[i];

			Vec3Int coords = ToGrid(pt);
			InstantiateChunkIfNeeded(coords);

			grid.setVoxel(coords);

		}

		renderVoxelGrid ();
	}


	void Update()
	{
		#if USE_CHUNK_FRUSTUM_CULLING
		MVP = camera.projectionMatrix * camera.worldToCameraMatrix;

		for (int i=0; i<num_chunks_x; i++)
			for (int j=0; j<num_chunks_y; j++)
				for (int k=0; k<num_chunks_z; k++) 
			{
					Chunks chunk = grid.voxelGrid [i, j, k];
				
					if (chunk == null)
						continue;
					if (chunk.isEmpty ())
						continue;
				
					Vec3Int chunkcoords = new Vec3Int (i, j, k);

					if(isChunkInFrustum(chunkcoords))
					{
						chunkGameObjects[i,j,k].GetComponent<MeshRenderer>().enabled = true;
					}
					else
					{
						chunkGameObjects[i,j,k].GetComponent<MeshRenderer>().enabled = false;
						continue;
					}
			}
		#endif





		#if DEBUG_THIS
		if (fakeData) 
		{
			debugPtCloud.m_pointsCount = 100;

			Random.seed = (int)(Time.time * 1000) % 10;
			
			for(int i=0;i<10;i++)
			{
				debugPtCloud.m_points[i] = new Vector3(Random.Range (-2.0f,2.0f), Random.Range (-2.0f,2.0f), Random.Range (-2.0f,2.0f));
			}

			int count = debugPtCloud.m_pointsCount;
			for(int i=0; i< count; i++)
			{
				
				Vector3 pt = debugPtCloud.m_points[i];
				
				Vec3Int coords = ToGrid(pt);
				InstantiateChunkIfNeeded(coords);
				
				grid.setVoxel(coords);
			}
			
			renderVoxelGrid ();
		}
		#endif
	}

	void OnGUI()
	{
		//GUI.Label (new Rect (10,500,1000,500), "PlanePts: " + depthPlanePts [0] + " " + depthPlanePts [1] + " " + depthPlanePts [2] + " " + depthPlanePts [3]  );
	}

	#if VOXEL_DELETION_PREV
	bool isInDepthFrustum(Vector3 pt)
	{
		return (depthPlanes [(int)PLANES.LEFT].GetSide (pt) &&
		        depthPlanes [(int)PLANES.RIGHT].GetSide (pt) &&
		        depthPlanes [(int)PLANES.TOP].GetSide (pt) &&
		        depthPlanes [(int)PLANES.BOTTOM].GetSide (pt) &&
		        depthPlanes [(int)PLANES.FAR].GetSide (pt));
	}
	
	bool isChunkInDepthFrustum(Vec3Int cc)
	{
		for(uint i=0;i<8;i++)
		{
			Vec3Int corner = new Vec3Int((int)(i & 1), (int)((i & 2) >> 1), (int)((i & 4) >> 2));
			Vec3Int point = (cc + corner) * chunk_size;
			Vector3 wpoint = FromGrid(point);
			if(isInDepthFrustum(wpoint))
				return true;
		}
		
		return false;
	}
	#endif

	
	#if VOXEL_DELETION_PREV
	public void computeDepthPlanes(ref Matrix4x4 mat, Vector3 depthPos, Vector3 depthMinPts, Vector3 depthMaxPts)
	{
		Vector3 lowerLeft = mat.MultiplyPoint(new Vector3 (depthMinPts.x, depthMinPts.y, depthMaxPts.z));
		Vector3 upperLeft = mat.MultiplyPoint(new Vector3 (depthMinPts.x, depthMaxPts.y, depthMaxPts.z));
		Vector3 lowerRight = mat.MultiplyPoint(new Vector3 (depthMaxPts.x, depthMinPts.y, depthMaxPts.z));
		Vector3 upperRight = mat.MultiplyPoint(new Vector3 (depthMaxPts.x, depthMaxPts.y, depthMaxPts.z));
		
		///Vector3 campos = camera.transform.position;
		
		//depthPlanes [(int)PLANES.LEFT] = new Plane (Vector3.Cross (lowerLeft-campos,upperLeft-campos), lowerLeft);
		//depthPlanes [(int)PLANES.RIGHT] = new Plane (Vector3.Cross (upperRight-campos,lowerRight-campos), upperRight);
		//depthPlanes [(int)PLANES.TOP] = new Plane (Vector3.Cross (upperLeft-campos,upperRight-campos), upperLeft);
		//depthPlanes [(int)PLANES.BOTTOM] = new Plane (Vector3.Cross (lowerRight-campos, lowerLeft-campos), lowerRight);
		
		Vector3 topEdge = upperRight - upperLeft;
		Vector3 bottomEdge = lowerRight - lowerLeft;
		Vector3 leftEdge = upperLeft - lowerLeft;
		Vector3 rightEdge = upperRight - lowerRight;
		
		depthPlanes [(int)PLANES.LEFT] = new Plane (-Vector3.Cross (-camera.transform.forward,leftEdge), lowerLeft);
		depthPlanes [(int)PLANES.RIGHT] = new Plane (-Vector3.Cross (rightEdge,-camera.transform.forward), upperRight);
		depthPlanes [(int)PLANES.TOP] = new Plane (Vector3.Cross (topEdge, -camera.transform.forward), upperLeft);
		depthPlanes [(int)PLANES.BOTTOM] = new Plane (Vector3.Cross (-camera.transform.forward, bottomEdge), lowerRight);
		
		
		depthPlanes [(int)PLANES.FAR] = new Plane (camera.transform.forward, upperRight);
		
		depthCamPos = depthPos;
		depthPlanePts [0] = lowerLeft;
		depthPlanePts [1] = upperLeft;
		depthPlanePts [2] = upperRight;
		depthPlanePts [3] = lowerRight;
	}
	#endif
	
}
