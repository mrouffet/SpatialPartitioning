using UnityEngine;

public class OctreeObj : MonoBehaviour
{
	public OctreeNode node = null;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;

		Gizmos.DrawCube(transform.position, Vector3.one * 0.1f);
	}
}
