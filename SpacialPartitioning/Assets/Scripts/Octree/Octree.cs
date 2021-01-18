using UnityEngine;

[ExecuteAlways]
public class Octree : MonoBehaviour
{
	[SerializeField]
	Vector3 size = Vector3.one * 10;

	// TODO: Implement.
	[SerializeField]
	public static Vector3 minCellSize = Vector3.one;

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
			Insert(_objects[i]);
	}

	public void Insert(OctreeObj _obj)
	{
		root.Insert(_obj);
	}

	public void Remove(OctreeObj _obj)
	{
		_obj.node.Remove(_obj);
	}
}
