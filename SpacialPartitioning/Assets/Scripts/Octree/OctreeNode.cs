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

	void GetChildBounds(int _index, ref Vector3 center, ref Vector3 size)
	{
#if UNITY_EDITOR
		// Child not found.
		if (_index < 0 || _index > 8)
		{
			Debug.LogError("Child index out of range!");
			return;
		}
#endif

		size = bounds.size / 2.0f;
		center = bounds.center + size / 2.0f;

		if (_index == 1)
			center.z -= size.z;
		else if (_index == 2)
		{
			center.y -= size.y;
		}
		else if (_index == 3)
		{
			center.y -= size.y;
			center.z -= size.z;
		}
		else if (_index == 4)
			center.x -= size.x;
		else if (_index == 5)
		{
			center.x -= size.x;
			center.z -= size.z;
		}
		else if (_index == 6)
		{
			center.x -= size.x;
			center.y -= size.y;
		}
		else if (_index == 7)
		{
			center.x -= size.x;
			center.y -= size.y;
			center.z -= size.z;
		}
	}

	OctreeNode CreateChild(int _index)
	{
#if UNITY_EDITOR
		// Child not found.
		if (_index < 0 || _index > 8)
		{
			Debug.LogError("Child index out of range!");
			return null;
		}
#endif

		Vector3 size = Vector3.zero;
		Vector3 center = Vector3.zero;

		GetChildBounds(_index, ref center, ref size);

		return CreateChild(_index, center, size);
	}

	OctreeNode CreateChild(int _index, Vector3 _center, Vector3 _size)
	{
#if UNITY_EDITOR
		// Child not found.
		if (_index < 0 || _index > 8)
		{
			Debug.LogError("Child index out of range!");
			return null;
		}
#endif

		children[_index] = new OctreeNode(new Bounds(_center, _size), this);

		return children[_index];
	}

	int GetChildFromPosition(Vector3 _pos)
	{
#if UNITY_EDITOR
		if(!bounds.Contains(_pos))
		{
			Debug.LogError("Position out of bound!");
			return -1;
		}
#endif


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

	bool CanContainObject(OctreeObj _obj)
	{
		// Check obj's min and max bounds contained in bounds.
		return bounds.Contains(_obj.transform.position + _obj.bounds.min) &&
			bounds.Contains(_obj.transform.position + _obj.bounds.max);
	}

	public void Insert(OctreeObj _obj)
	{
		if (state == OctreeNodeState.Empty)
		{
			objects.Add(_obj);
			_obj.node = this;

			state = OctreeNodeState.OneObject;

			return;
		}
		else if (state == OctreeNodeState.OneObject)
		{
			// Split cell: re-insert first object.

			// Clean obj list first.
			OctreeObj firstObj = objects[0];
			objects.Clear();

			Insert_Internal(firstObj);

			state = OctreeNodeState.Complex;
		}

		// New object.
		Insert_Internal(_obj);
	}

	void Insert_Internal(OctreeObj _obj)
	{
		int childIndex = GetChildFromPosition(_obj.transform.position);

#if UNITY_EDITOR
		// Child not found.
		if (childIndex == -1)
		{
			Debug.LogError("Child not found, object may be out of bound!");
			return;
		}
#endif

		OctreeNode child = children[childIndex];

		// First time creation.
		if (child == null)
			child = CreateChild(childIndex);

		if (child.CanContainObject(_obj)) // Check new child bounds can contain object bounds.
			child.Insert(_obj);
		else                        // Object too big for child: insert in this node.
		{
			_obj.node = this;
			objects.Add(_obj);
		}
	}

	public void Remove(OctreeObj _obj)
	{
		if(!objects.Remove(_obj))
		{
			Debug.LogError("Remove object not in list!");
			return;
		}

		if (state == OctreeNodeState.OneObject)
			state = OctreeNodeState.Empty;
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
