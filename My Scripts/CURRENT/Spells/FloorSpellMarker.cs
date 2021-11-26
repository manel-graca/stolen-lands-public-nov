using StolenLands.Player;
using UnityEngine;
namespace StolenLands.Abilities
{
	public class FloorSpellMarker : MonoBehaviour
	{
		public LayerMask ignoreLayer;
		[SerializeField] private GameObject decalPrefab;
		[SerializeField] float crowdControlMaxDistance;
		[SerializeField] private float yOffset;
		public bool maxDistanceReached = false;

		GameObject player;

		void Start()
		{
			player = GameObject.FindWithTag("Player");
		}

		private void Update()
		{
			MouseRay();
		}

		private void MouseRay()
		{
			RaycastHit hitInfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hitInfo, crowdControlMaxDistance, ~ignoreLayer))
			{
				if (hitInfo.transform.GetComponent<Terrain>())
				{
					SpawnDecal(hitInfo);
				}
			}
		}

		private void SpawnDecal(RaycastHit hitInfo)
		{
			Vector3 offset = new Vector3(0f, yOffset, 0f);
			decalPrefab.transform.position = hitInfo.point + offset;
		}

		public Vector3 GetAoEPosition()
		{
			RaycastHit hitInfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hitInfo, crowdControlMaxDistance, ~ignoreLayer))
			{
				if (hitInfo.transform.GetComponent<Terrain>())
				{
					return hitInfo.point;
				}
			}
			return Vector3.zero; 
		}
	}
}

