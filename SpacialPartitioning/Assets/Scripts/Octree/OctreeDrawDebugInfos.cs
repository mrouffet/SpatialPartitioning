using UnityEngine;

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
}
