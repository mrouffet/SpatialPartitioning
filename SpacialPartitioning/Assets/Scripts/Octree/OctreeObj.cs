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

	public List<OctreeObj> QueryCollisions()
	{
		List<OctreeObj> result = new List<OctreeObj>();

		foreach(OctreeNode node in nodes)
		{
			foreach(OctreeObj obj in node.objects)
			{
				if (obj != this && !result.Contains(obj))
					result.Add(obj);
			}
		}

		return result;
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
