using System.Collections.Generic;
using UnityEngine;

enum BoundsType
{
	Custom,

	Collider,

	Mesh,
}

public class OctreeObj : MonoBehaviour
{
	public List<OctreeNode> nodes = new List<OctreeNode>();

	[SerializeField]
	BoundsType boundType = BoundsType.Custom;

	public Bounds bounds;

	private void Awake()
	{
		UpdateBounds();
	}

	private void OnDestroy()
	{
		foreach(OctreeNode node in nodes)
			node.Remove(this);
	}

	void UpdateBounds()
	{
		switch (boundType)
		{
			case BoundsType.Custom:
			{
				break;
			}
			case BoundsType.Collider:
			{
				Collider collider = GetComponent<Collider>();

				if (collider)
					bounds = collider.bounds;

				break;
			}
			case BoundsType.Mesh:
			{
				MeshRenderer render = GetComponent<MeshRenderer>();

				if (render)
					bounds = render.bounds;

				break;
			}
			default:
			{
				Debug.LogWarning("Bounds type not supported!");
				break;
			}
		}
	}


	private void OnValidate()
	{
		UpdateBounds();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;

		Gizmos.DrawCube(transform.position, Vector3.one * 0.1f);

		if(boundType == BoundsType.Custom)
			Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
	}
}
