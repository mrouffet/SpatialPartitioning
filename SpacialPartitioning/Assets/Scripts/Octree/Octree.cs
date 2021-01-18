using System;
using UnityEngine;

public class Octree : OctreeNode
{
	public readonly float minCellSize = 1.0f;

	public Octree(Vector3 _position, Vector3 _size, float _minCellSize = 1.0f) :
		base(new Bounds(_position, _size), null)
	{
		minCellSize = _minCellSize;
	}

	public void Construct(OctreeObj[] _objects)
	{
		for (int i = 0; i < _objects.Length; ++i)
			Insert(_objects[i]);
	}

	public sealed override void Insert(OctreeObj _obj)
	{
		if(!bounds.Contains(_obj.transform.position + _obj.bounds.min) && !bounds.Contains(_obj.transform.position + _obj.bounds.max))
		{
			Debug.LogWarning("Object out of octree bounds!");
			return;
		}

		base.Insert(_obj);
	}
}
