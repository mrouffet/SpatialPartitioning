using System;
using UnityEngine;

[Serializable]
public abstract class OctreeNodeBase
{
	public readonly Bounds bounds;

	[NonSerialized]
	public readonly OctreeNode parent = null;

	public OctreeNodeBase(Bounds _bounds, OctreeNode _parent)
	{
		bounds = _bounds;

		parent = _parent;
	}

	public abstract void Insert(OctreeObj _obj);
	public abstract void Remove(OctreeObj _obj);


	public virtual void OnDrawGizmos(OctreeDrawDebugInfos debugInfos)
	{
		Gizmos.DrawWireCube(bounds.center, bounds.size);
	}
}
