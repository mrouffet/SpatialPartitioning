using UnityEngine;

[ExecuteAlways]
public class Octree : MonoBehaviour
{
	[SerializeField]
	Vector3 size = Vector3Int.one * 10;

#if UNITY_EDITOR
	[SerializeField]
	int _debugHighlightDepth = -1;
#endif

	public OctreeNode root = null;

	private void Awake()
	{
		ConstructGrid(GetComponentsInChildren<OctreeObj>());
	}

	public void OnValidate()
	{
		ConstructGrid(GetComponentsInChildren<OctreeObj>());
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = OctreeDrawDebugInfos.baseColor;
		OctreeDrawDebugInfos debugInfos = new OctreeDrawDebugInfos(_debugHighlightDepth);

		root.OnDrawGizmos(debugInfos);
	}

	private void ConstructGrid(OctreeObj[] _objects)
	{
		root = new OctreeNode(new Bounds(transform.position, size), null);

		for(int i = 0; i < _objects.Length; ++i)
			root.Insert(_objects[i]);
	}
}
