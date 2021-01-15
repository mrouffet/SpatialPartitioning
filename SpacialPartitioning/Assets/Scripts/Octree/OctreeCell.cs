using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OctreeCell : OctreeNodeBase
{
	public List<OctreeObj> objects = new List<OctreeObj>();

	public OctreeCell(Bounds _bounds, OctreeNode _parent) : base(_bounds, _parent)
	{
	}

	public override sealed void Insert(OctreeObj _obj)
	{
		objects.Add(_obj);
	}

	public override sealed void Remove(OctreeObj _obj)
	{
		objects.Remove(_obj);
	}

	public sealed override void OnDrawGizmos(OctreeDrawDebugInfos debugInfos)
	{
		Color baseColor = Gizmos.color;

		// Draw empty cell with alpha 0.1.
		if (objects.Count == 0)
		{
			Color color = Gizmos.color;
			color.a = 0.1f;

			Gizmos.color = color;
		}

		base.OnDrawGizmos(debugInfos);

		Gizmos.color = baseColor;
	}
}
