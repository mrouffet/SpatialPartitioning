using System;
using System.Collections.Generic;
using UnityEngine;

enum OctreeNodeState
{
	// Default empty state.
	Empty,

	// Only one object in cell (first object added).
	OneObject,

	// Children + object active.
	Complex,
}

public class OctreeNode
{
	public readonly Bounds bounds;

	public readonly OctreeNode parent = null;

	// Children of current node.
	public OctreeNode[] children = new OctreeNode[8];

	// Object in current node cell.
	public List<OctreeObj> objects = new List<OctreeObj>();

	OctreeNodeState state = OctreeNodeState.Empty;

	public OctreeNode(Bounds _bounds, OctreeNode _parent)
	{
		bounds = _bounds;

		parent = _parent;
	}

	OctreeNode CreateChild(int _index)
	{
		Vector3 size = bounds.size / 2.0f;
		Vector3 center = bounds.center + size / 2.0f;

		if(_index == 0)
			children[0] = new OctreeNode(new Bounds(center, size), this);

		if (_index == 1)
		{
			center.z -= size.z;
			children[1] = new OctreeNode(new Bounds(center, size), this);
		}

		if (_index == 2)
		{
			center.y -= size.y;
			children[2] = new OctreeNode(new Bounds(center, size), this);
		}

		if (_index == 3)
		{
			center.y -= size.y;
			center.z -= size.z;
			children[3] = new OctreeNode(new Bounds(center, size), this);
		}

		if (_index == 4)
		{
			center.x -= size.x;
			children[4] = new OctreeNode(new Bounds(center, size), this);
		}

		if (_index == 5)
		{
			center.x -= size.x;
			center.z -= size.z;
			children[5] = new OctreeNode(new Bounds(center, size), this);
		}

		if (_index == 6)
		{
			center.x -= size.x;
			center.y -= size.y;
			children[6] = new OctreeNode(new Bounds(center, size), this);
		}

		if (_index == 7)
		{
			center.x -= size.x;
			center.y -= size.y;
			center.z -= size.z;
			children[7] = new OctreeNode(new Bounds(center, size), this);
		}

		return children[_index];
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

	public void Insert(OctreeObj _obj)
	{
		if(state == OctreeNodeState.Empty)
		{
			objects.Add(_obj);
			_obj.node = this;

			state = OctreeNodeState.OneObject;

			return;
		}
		else if(state == OctreeNodeState.OneObject)
		{
			// Split cell.

			// ReInsert first object.
			Insert_Internal(objects[0]);
			objects.Clear();

			state = OctreeNodeState.Complex;
		}

		// New object.
		Insert_Internal(_obj);
	}

	void Insert_Internal(OctreeObj _obj)
	{
		int childIndex = GetChildFromPosition(_obj.transform.position);

		// Child not found.
		if (childIndex == -1)
		{
			Debug.LogError("Child not found, object may be out of bound!");
			return;
		}

		OctreeNode child = children[childIndex];

		// First time creation.
		if (child == null)
			child = CreateChild(childIndex);

		child.Insert(_obj);
	}

	public void Remove(OctreeObj _obj)
	{
		/*
		int childIndex = GetChildFromPosition(_obj.transform.position);

		// Child not found.
		if (childIndex == -1)
		{
			Debug.LogError("Child not found, object may be out of bound!");
			return;
		}

		OctreeNode child = children[childIndex];

		if(child == null)
		{
			Debug.LogWarning("Remove object from an empty cell.");
			return;
		}


		// Child is a node.
		{
			OctreeNode nodeChild = child as OctreeNode;

			if (nodeChild != null)
			{
				nodeChild.Remove(_obj);
				return;
			}
		}

		// Child is a cell.
		{
			OctreeCell cellChild = child as OctreeCell;

			cellChild.objects.Remove(_obj);

			if (cellChild.objects.Count == 0) // Empty cell?
				children[childIndex] = null;
		}
		*/
	}

	public void OnDrawGizmos(OctreeDrawDebugInfos debugInfos)
	{
		// Draw after for better highlight.
		if (!debugInfos.IsHighlight())
			debugInfos.Draw(this);


		debugInfos.AddDepth();

		for (int i = 0; i < 8; ++i)
		{
			if (children[i] != null)
				children[i].OnDrawGizmos(debugInfos);
		}

		debugInfos.RemoveDepth();


		// Draw after for better highlight.
		if (debugInfos.IsHighlight())
			debugInfos.Draw(this);
	}
}
