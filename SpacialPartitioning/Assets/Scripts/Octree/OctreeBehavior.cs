using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class OctreeBehavior : MonoBehaviour
{
	[SerializeField]
	Octree mHandle = null;

	[SerializeField]
	float minCellSize = 1.0f;

	[SerializeField]
	Vector3 size = Vector3.one * 10.0f;

#if UNITY_EDITOR
	[SerializeField]
	int _debugHighlightDepth = -1;
#endif

	private void Awake()
	{
		Construct();
	}

	void Construct()
	{
		mHandle = new Octree(transform.position, size, minCellSize);
		mHandle.Construct(GetComponentsInChildren<OctreeObj>());
	}

	public List<OctreePair> QueryCollisions()
	{
		List<OctreePair> res = new List<OctreePair>();

		mHandle.QueryCollisions(ref res);

		return res;
	}

	public void OnValidate()
	{
		Construct();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = OctreeDrawDebugInfos.baseColor;
		OctreeDrawDebugInfos debugInfos = new OctreeDrawDebugInfos(_debugHighlightDepth);

		mHandle.OnDrawGizmos(debugInfos);
	}
}
