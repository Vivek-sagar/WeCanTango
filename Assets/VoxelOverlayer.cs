using UnityEngine;
using System.Collections;

public class VoxelOverlayer : Singleton<VoxelOverlayer> {
	VoxelExtractionPointCloud vxe;
	public Camera camera;
	public GameObject overlay;
	public GameObject[,,] overlayInstances;
	public Material material;
	const int overlayInstanceCount = 27;
	const int dim = 3;
	// Use this for initialization
	void Start () {
		vxe = VoxelExtractionPointCloud.Instance;
		overlayInstances = new GameObject[dim,dim,dim];
		for (int i=0; i<dim; i++)
			for (int j=0; j<dim; j++)
				for (int k=0; k<dim; k++)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = ChunkTemplate.Instance.vertices;
			mesh.normals = ChunkTemplate.Instance.normals;
			mesh.uv = ChunkTemplate.Instance.uvs;

			overlayInstances [i,j,k] = Instantiate (overlay);
			overlayInstances [i,j,k].GetComponent<MeshFilter>().mesh = mesh;
		}
		overlay.SetActive (false);
	}

	void buildChunkMesh(Chunks chunk, Vec3Int chunkCoords, Mesh _mesh)
	{
		int[] indices = new int[ChunkTemplate.Instance.vertex_count * 3];
		IndexStack<int> istack = new IndexStack<int>(indices);


		for(int x=0;x<vxe.chunk_size;x++)
			for(int y=0;y<vxe.chunk_size;y++)
				for(int z=0;z<vxe.chunk_size;z++)
			{
				Vec3Int vcoord = new Vec3Int(x,y,z);
				Voxel voxel = chunk.getVoxel(vcoord);

				if(voxel.isOccupied())
				{
					Vector3 vwrldcoord = vxe.FromGrid(chunkCoords * vxe.chunk_size + vcoord);
					if( (vwrldcoord - camera.transform.position).sqrMagnitude > 0.64f )
						continue;

					//front
					if(voxel.getFace(VF.VX_FRONT_SHOWN))
					{
						//front
						istack.push(chunk.getIndex(x,y,z + 1) + chunk.getDirOffset(DIR.DIR_FRONT));
						istack.push(chunk.getIndex(x + 1,y,z + 1)+ chunk.getDirOffset(DIR.DIR_FRONT));
						istack.push(chunk.getIndex(x + 1,y + 1,z + 1)+ chunk.getDirOffset(DIR.DIR_FRONT));
						istack.push(chunk.getIndex(x,y + 1,z + 1)+ chunk.getDirOffset(DIR.DIR_FRONT));
					}
					
					if(voxel.getFace(VF.VX_RIGHT_SHOWN))
					{
						//right
						istack.push(chunk.getIndex(x+1,y,z)+ chunk.getDirOffset(DIR.DIR_RIGHT));
						istack.push(chunk.getIndex(x+1,y+1,z)+ chunk.getDirOffset(DIR.DIR_RIGHT));
						istack.push(chunk.getIndex(x+1,y + 1,z + 1)+ chunk.getDirOffset(DIR.DIR_RIGHT));
						istack.push(chunk.getIndex(x+1,y,z+1)+ chunk.getDirOffset(DIR.DIR_RIGHT));
					}
					
					if(voxel.getFace(VF.VX_BACK_SHOWN))
					{
						//back
						istack.push(chunk.getIndex(x,y,z) + chunk.getDirOffset(DIR.DIR_BACK));
						istack.push(chunk.getIndex(x,y + 1,z) + chunk.getDirOffset(DIR.DIR_BACK));
						istack.push(chunk.getIndex(x + 1,y + 1,z) + chunk.getDirOffset(DIR.DIR_BACK));
						istack.push(chunk.getIndex(x + 1,y,z) + chunk.getDirOffset(DIR.DIR_BACK));
					}
					
					if(voxel.getFace(VF.VX_LEFT_SHOWN))
					{
						//left
						istack.push(chunk.getIndex(x,y,z)+ chunk.getDirOffset(DIR.DIR_LEFT));
						istack.push(chunk.getIndex(x,y,z + 1)+ chunk.getDirOffset(DIR.DIR_LEFT));
						istack.push(chunk.getIndex(x,y + 1,z + 1)+ chunk.getDirOffset(DIR.DIR_LEFT));
						istack.push(chunk.getIndex(x,y + 1,z)+ chunk.getDirOffset(DIR.DIR_LEFT));
					}
					
					if(voxel.getFace(VF.VX_TOP_SHOWN))
					{
						//top
						istack.push(chunk.getIndex(x,y+1,z)+ chunk.getDirOffset(DIR.DIR_UP));
						istack.push(chunk.getIndex(x,y+1,z+1)+ chunk.getDirOffset(DIR.DIR_UP));
						istack.push(chunk.getIndex(x+1,y+1,z+1)+ chunk.getDirOffset(DIR.DIR_UP));
						istack.push(chunk.getIndex(x+1,y+1,z)+ chunk.getDirOffset(DIR.DIR_UP));
					}
					
					if(voxel.getFace(VF.VX_BOTTOM_SHOWN))
					{
						//bottom
						istack.push(chunk.getIndex(x,y,z)+ chunk.getDirOffset(DIR.DIR_DOWN));
						istack.push(chunk.getIndex(x+1,y,z)+ chunk.getDirOffset(DIR.DIR_DOWN));
						istack.push(chunk.getIndex(x+1,y,z+1)+ chunk.getDirOffset(DIR.DIR_DOWN));
						istack.push(chunk.getIndex(x,y,z+1)+ chunk.getDirOffset(DIR.DIR_DOWN));
					}
				}
				
			}
		
		int[] indexArray = new int[istack.getCount()];
		System.Array.Copy(istack.getArray(),indexArray,istack.getCount());
		
		_mesh.SetIndices (indexArray , MeshTopology.Quads, 0);
	}

	public void overlayCurrentChunk()
	{
		Vector3 pt = camera.transform.position;
		Vec3Int chunkCoords = vxe.getChunkCoords (pt);


		for(int x=-1;x<=1;x++)
			for(int y=-1;y<=1;y++)
				for(int z=-1;z<=1;z++)
		{
			Vec3Int camchunk = chunkCoords + new Vec3Int(x,y,z);
			Chunks chunk = vxe.grid.voxelGrid[camchunk.x, camchunk.y, camchunk.z];
			GameObject refChunk = vxe.chunkGameObjects [camchunk.x, camchunk.y, camchunk.z];
			GameObject overlayInstance = overlayInstances[x + 1, y + 1, z + 1];
			overlayInstance.transform.position = refChunk.transform.position - camera.transform.forward * 0.01f;
			overlayInstance.GetComponent<MeshRenderer> ().material = material;

			buildChunkMesh(chunk,camchunk,overlayInstance.GetComponent<MeshFilter>().mesh);
		}
	
	}

	// Update is called once per frame
	void Update () {

	}
}
