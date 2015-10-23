using UnityEngine;
using System.Collections;

public class PetMessage : Singleton<PetMessage> {

	enum MessageState
	{
		TUTORIAL = 0,
		ISCLOSE = 1,
		THANKYOU = 2,
		SLEEPING = 3,
		NOTHING = 4
	}

	public Texture2D tutorial;
	public Texture2D thankyou;
	public Texture2D isclose;
	public Texture2D sleeping;
	ItemSpawner itemspawn;
	VoxelExtractionPointCloud vxe;
	public Camera camera;
	MessageState state;
	// Use this for initialization
	void Start () {
		itemspawn = ItemSpawner.Instance;
		vxe = VoxelExtractionPointCloud.Instance;
		state = MessageState.NOTHING;
		GetComponent<JumpingAI>().init ();
		StartCoroutine (messageSystem ());
	}

	IEnumerator messageSystem()
	{
		XZBillboard.Instance.changeTexture (tutorial);
		state = MessageState.TUTORIAL;
		yield return new WaitForSeconds (10.0f);

		XZBillboard.Instance.hide ();

		while(true)
		{
			if(state != MessageState.ISCLOSE)
			{
				for(int i=0;i<itemspawn.items.Length;i++)
				{
					if(itemspawn.spawneditems[i] == null || itemspawn.spawneditems[i].GetComponent<TriggerScript>().triggered || itemspawn.spawneditems[i].CompareTag("Portal"))
						continue;
					
					float groundLength = (itemspawn.spawneditems[i].transform.position - transform.position).magnitude;
					if( groundLength < vxe.voxel_size * 25 )
					{
						XZBillboard.Instance.show ();
						state = MessageState.ISCLOSE;
						XZBillboard.Instance.changeTexture (isclose);
						yield return new WaitForSeconds(10.0f);
						XZBillboard.Instance.hide ();
						state = MessageState.NOTHING;
						break;
					}
				}
			}

			if(state == MessageState.THANKYOU)
			{
				yield return new WaitForSeconds(5.0f);
				XZBillboard.Instance.hide ();
				state = MessageState.NOTHING;
			}

			yield return new WaitForSeconds(1.0f);
		}
	}

	public void setThankYou()
	{
		state = MessageState.THANKYOU;
		XZBillboard.Instance.show ();
		XZBillboard.Instance.changeTexture (thankyou);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
