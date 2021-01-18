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

// Offset between node child index on each axis.
/*
 *				  _______	  _______
 *				 /______/|	 /______/|
 *				|		||	|		||
 *				|	4	||	|	5	||
 *				|_______|/	|_______|/
 *				  _______	  _______
 *				 /______/|	 /______/|
 *				|		||	|		||
 *				|	6	||	|	7	||
 *				|_______|/	|_______|/
 *	  _______	  _______
 *	 /______/|	 /______/|
 *	|		||	|		||
 *	|	0	||	|	1	||
 *	|_______|/	|_______|/
 *	  _______	  _______
 *	 /______/|	 /______/|
 *	|		||	|		||
 *	|	2	||	|	3	||
 *	|_______|/	|_______|/
 */
enum OctreeNodeIndexOffset
{
	X = 1,

	Y = 2,

	Z = 4,
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
			center.x -= size.x;
		else if (_index == 2)
		{
			center.y -= size.y;
		}
		else if (_index == 3)
		{
			center.x -= size.x;
			center.y -= size.y;
		}
		else if (_index == 4)
			center.z -= size.z;
		else if (_index == 5)
		{
			center.x -= size.x;
			center.z -= size.z;
		}
		else if (_index == 6)
		{
			center.z -= size.z;
			center.y -= size.y;
		}
		else if (_index == 7)
			center -= size;
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
		// Clamp out of bound positions.
		if(!bounds.Contains(_pos))
			_pos = bounds.ClosestPoint(_pos);


		if (_pos.z > bounds.center.z)
		{
			// child 0, 1, 2, 3.

			if (_pos.y > bounds.center.y)
			{
				// child 0, 1

				if (_pos.x > bounds.center.x)
					return 0;
				else
					return 1;
			}
			else
			{
				// child 2, 3

				if (_pos.x > bounds.center.x)
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

				if (_pos.x > bounds.center.x)
					return 4;
				else
					return 5;
			}
			else
			{
				// child 6, 7

				if (_pos.x > bounds.center.x)
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
		Vector3 objMin = _obj.transform.position + _obj.bounds.min;
		Vector3 objMax = _obj.transform.position + _obj.bounds.max;

		int minChildIndex = GetChildFromPosition(objMin);
		int maxChildIndex = GetChildFromPosition(objMax);

		// Same child.
		if (minChildIndex == maxChildIndex)
		{
			OctreeNode child = children[minChildIndex];

			// First time creation.
			if (child == null)
				child = CreateChild(minChildIndex);

			child.Insert(_obj);
			return;
		}

		// Multiple child.
		List<int> childIndices = null;

		{
			int indexDiff = minChildIndex - maxChildIndex;

			if (indexDiff == 3)
			{
				// min and max bounds are on the same Z plan (X and Y difference).
				childIndices = new List<int> { maxChildIndex, maxChildIndex + 1, maxChildIndex + 2, minChildIndex };
			}
			else if (indexDiff == 5)
			{
				// min and max bounds are on the same Y plan (X and Z difference).
				childIndices = new List<int> { maxChildIndex, maxChildIndex + 1, minChildIndex - 1, minChildIndex };
			}
			else if (indexDiff == 6)
			{
				// min and max bounds are on the same X plan (Y and Z difference).
				childIndices = new List<int> { maxChildIndex, maxChildIndex + 2, minChildIndex - 2, minChildIndex };
			}
			else if (indexDiff == 7)
			{
				// min and max bounds don't share any plan.
				childIndices = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
			}
			else
			{
				// min and max bounds share 2 plans.
				childIndices = new List<int> { maxChildIndex, minChildIndex };
			}
		}


		// Add obj to each child.
		foreach (int childIndex in childIndices)
		{
			OctreeNode child = children[childIndex];

			// First time creation.
			if (child == null)
				child = CreateChild(childIndex);

			child.Insert(_obj);
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

		// TODO: Re-compute opti while Complex.
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
