using UnityEngine;
using UnityEditor;

public class OctreeDrawDebugInfos
{
	public static readonly Color baseColor = Color.white;
	public static readonly Color highlightColor = Color.yellow;

	public int currDepth = 0;
	public readonly int highlightDepth = -1;

	public OctreeDrawDebugInfos(int _highlightDepth)
	{
		highlightDepth = _highlightDepth;

		if (currDepth == highlightDepth)
			Gizmos.color = highlightColor;
		else
			Gizmos.color = baseColor;
	}

	public bool IsHighlight()
	{
		return currDepth == highlightDepth;
	}

	public void AddDepth()
	{
		Color currColor = ++currDepth == highlightDepth ? highlightColor : baseColor;
		currColor.a = Gizmos.color.a;

		Gizmos.color = currColor;
	}

	public void RemoveDepth()
	{
		Color currColor = --currDepth == highlightDepth ? highlightColor : baseColor;
		currColor.a = Gizmos.color.a;

		Gizmos.color = currColor;
	}

	public void Draw(OctreeNode _node)
	{
		Color currColor = Gizmos.color;

		// Empty object alpha 0.1.
		if (_node.objects.Count == 0)
		{
			Color newColor = currColor;
			newColor.a = 0.1f;

			Gizmos.color = newColor;
		}
		else
		{
			// Highlight cell with selected object.
			foreach (OctreeObj obj in _node.objects)
			{
				if (Selection.Contains(obj.gameObject))
				{
					Gizmos.color = highlightColor;
					break;
				}
			}
		}

		Gizmos.DrawWireCube(_node.bounds.center, _node.bounds.size);

		Gizmos.color = currColor;
	}
}
