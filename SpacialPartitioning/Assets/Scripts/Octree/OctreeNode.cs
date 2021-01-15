using System;
using UnityEngine;

[Serializable]
public class OctreeNode : OctreeNodeBase
{
	public OctreeNodeBase[] children =  new OctreeNodeBase[8];

	public OctreeNode(Bounds _bounds, OctreeNode _parent) : base(_bounds, _parent)
	{
		Vector3 size = _bounds.size / 2.0f;
		Vector3 center = _bounds.center + size / 2.0f;

		children[0] = new OctreeCell(new Bounds(center, size), this);

		center.z -= size.z;
		children[1] = new OctreeCell(new Bounds(center, size), this);

		center.z += size.z;
		center.y -= size.y;
		children[2] = new OctreeCell(new Bounds(center, size), this);

		center.z -= size.z;
		children[3] = new OctreeCell(new Bounds(center, size), this);

		center.z += size.z;
		center.y += size.y;
		center.x -= size.x;
		children[4] = new OctreeCell(new Bounds(center, size), this);

		center.z -= size.z;
		children[5] = new OctreeCell(new Bounds(center, size), this);

		center.z += size.z;
		center.y -= size.y;
		children[6] = new OctreeCell(new Bounds(center, size), this);

		center.z -= size.z;
		children[7] = new OctreeCell(new Bounds(center, size), this);
	}

	int GetChildFromPosition(Vector3 _pos)
	{
		if(!bounds.Contains(_pos))
		{
			Debug.LogError("Position out of bound!");
			return -1;
		}


		if(_pos.x > bounds.center.x)
		{
			// child 0, 1, 2, 3.

			if (_pos.y > bounds.center.y)
			{
				// child 0, 1

				if (_pos.z > bounds.center.z)
					return 0;
				else
					return 1;
			}
			else
			{
				// child 2, 3

				if (_pos.z > bounds.center.z)
					return 2;
				else
					return 3;
			}
		}
		else
		{
			// child 4, 5, 6, 7.

			if (_pos.y > bounds.center.y)
			{
				// child 4, 5

				if (_pos.z > bounds.center.z)
					return 4;
				else
					return 5;
			}
			else
			{
				// child 6, 7

				if (_pos.z > bounds.center.z)
					return 6;
				else
					return 7;
			}
		}
	}

	public override sealed void Insert(OctreeObj _obj)
	{
		int childIndex = GetChildFromPosition(_obj.transform.position);

		// Child not found.
		if(childIndex == -1)
		{
			Debug.LogError("Child not found, object may be out of bound!");
			return;
		}

		OctreeNodeBase child = children[childIndex];

		// Child is already a node.
		{
			OctreeNode nodeChild = child as OctreeNode;

			if (nodeChild != null)
			{
				nodeChild.Insert(_obj);
				return;
			}
		}

		// Child is a cell.
		{
			OctreeCell cellChild = child as OctreeCell;

			if (cellChild.objects.Count == 0) // Empty cell?
				cellChild.Insert(_obj);
			else
			{
				// Split cell in node.
				children[childIndex] = new OctreeNode(cellChild.bounds, this);

				// Reinsert previous objects.
				foreach(var obj in cellChild.objects)
					children[childIndex].Insert(obj);

				// Insert new object.
				children[childIndex].Insert(_obj);
			}
		}
	}

	public override sealed void Remove(OctreeObj _obj)
	{
	}

	public sealed override void OnDrawGizmos(OctreeDrawDebugInfos debugInfos)
	{
		// Draw after for better highlight.
		if(!debugInfos.IsHighlight())
			base.OnDrawGizmos(debugInfos);


		debugInfos.AddDepth();

		for (int i = 0; i < 8; ++i)
			children[i].OnDrawGizmos(debugInfos);

		debugInfos.RemoveDepth();


		// Draw after for better highlight.
		if(debugInfos.IsHighlight())
			base.OnDrawGizmos(debugInfos);
	}
}
