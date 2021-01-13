﻿using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class IGrid : MonoBehaviour
{
	public delegate void CellDelegate(ICell cell);
	//public delegate void CellDelegate(Vector3 position, Vector3 extents);

#if UNITY_EDITOR
	[SerializeField]
	protected GridDrawDebug _debugDrawGrid = GridDrawDebug.UnselectedTransparent;

	protected GridDrawMode GetDrawMode()
	{
		switch (_debugDrawGrid)
		{
			case GridDrawDebug.Always:
				return GridDrawMode.Opaque;
			case GridDrawDebug.SelectedOnly:
				return Selection.Contains(gameObject) ? GridDrawMode.Opaque : GridDrawMode.None;
			case GridDrawDebug.UnselectedTransparent:
				return Selection.Contains(gameObject) ? GridDrawMode.Opaque : GridDrawMode.Transparent;
			case GridDrawDebug.AlwaysTransparent:
				return GridDrawMode.Transparent;
			case GridDrawDebug.Never:
				return GridDrawMode.None;
			default:
				Debug.LogWarning("GridDrawDebug not supported!");
				return GridDrawMode.Opaque;
		}
	}
#endif

	void Awake()
	{
		ConstructGrid(GetComponentsInChildren<GridObj>());
	}

	public void OnValidate()
	{
		ConstructGrid(GetComponentsInChildren<GridObj>());
	}

	protected virtual void ConstructGrid(GridObj[] _objects)
	{
		Debug.LogError("ConstructGrid must be overridden in children!");
	}

	protected virtual void ForEachCell(CellDelegate lambda)
	{
		Debug.LogError("ForEachCells must be overridden in children!");
	}
}

public enum GridDrawDebug
{
	Always,

	SelectedOnly,

	UnselectedTransparent,

	AlwaysTransparent,

	Never,
}

public enum GridDrawMode
{
	Opaque,

	Transparent,

	None,
}