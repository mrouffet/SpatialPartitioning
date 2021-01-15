using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class StaticGrid : IGrid
{
	[SerializeField]
	Vector3Int _dimensions = new Vector3Int(10, 10, 10);

	[SerializeField]
	Vector3 _cellExtents = Vector3.one;

	public StaticCell[] cells { get; private set; }

	public Vector3 extents { get; private set; }

	public Vector3 minBound { get { return transform.position - extents; } }
	public Vector3 maxBound { get { return transform.position + extents; } }

#if UNITY_EDITOR
	[SerializeField]
	bool _debugDrawCenter = true;

	[SerializeField]
	bool _debugDrawBounds = true;

	[SerializeField]
	int _debugSelectedCell = -1;

	Color GetDebugColor(int index, GridDrawMode drawMode)
	{
		Color[] colors = { Color.grey, Color.green, Color.yellow, Color.red, Color.magenta };

		Color color = colors[index];

		if (drawMode == GridDrawMode.Transparent)
			color.a = 0.1f;

		return color;
	}
#endif

	Vector3 GetHalfDims()
	{
		return (Vector3)(_dimensions) / 2.0f;
	}

	int GetCellID(Vector3Int cellCoords)
	{
		// Cell id is its index in grid.

		if (cellCoords.x < 0 || cellCoords.x > _dimensions.x ||
			cellCoords.y < 0 || cellCoords.y > _dimensions.y ||
			cellCoords.z < 0 || cellCoords.z > _dimensions.z)
			Debug.LogError("Invalid cell coordinates: (" + cellCoords.x + ',' + cellCoords.y + ',' + cellCoords.z + ")");

		return cellCoords.x + cellCoords.y * _dimensions.x + cellCoords.z * _dimensions.x * _dimensions.y;
	}

	StaticCell GetCell(Vector3Int cellCoords)
	{
		int cellID = GetCellID(cellCoords);

		return cells[cellID];
	}

	Vector3Int GetCellCoordsFromPosition(Vector3 position)
	{
		Vector3 relativePos = position - minBound;

		Vector3Int truncPos = new Vector3Int(
			(int)Math.Truncate(relativePos.x / _cellExtents.x),
			(int)Math.Truncate(relativePos.y / _cellExtents.y),
			(int)Math.Truncate(relativePos.z / _cellExtents.z)
		);

		return truncPos;
	}

	StaticCell GetCellFromPosition(Vector3 position)
	{
		return GetCell(GetCellCoordsFromPosition(position));
	}

	Vector3Int GetCellCoords(StaticCell cell)
	{
		Vector3Int coords = Vector3Int.zero;

		int temp = cell.id / _dimensions.x;

		coords.x = cell.id % _dimensions.x;
		coords.y = temp % _dimensions.y;
		coords.z = temp / _dimensions.y;

		return coords;
	}

	Vector3 GetCellPosition(StaticCell cell)
	{
		Vector3 indexPos = GetCellCoords(cell);

		indexPos.x -= _dimensions.x / 2;
		indexPos.y -= _dimensions.y / 2;
		indexPos.z -= _dimensions.z / 2;

		Vector3 relativePos = Vector3.Scale(indexPos, _cellExtents);

		if (_dimensions.x % 2 == 0)
			relativePos.x += 0.5f * _cellExtents.x;

		if (_dimensions.y % 2 == 0)
			relativePos.y += 0.5f * _cellExtents.y;

		if (_dimensions.z % 2 == 0)
			relativePos.z += 0.5f * _cellExtents.z;

		return transform.position + relativePos;
	}

	protected sealed override void ConstructGrid(GridObj[] _objects)
	{
		// Compute bounds.
		extents = Vector3.Scale((Vector3)(_dimensions) / 2.0f, _cellExtents);


		// Build cells.
		cells = new StaticCell[_dimensions.x * _dimensions.y * _dimensions.z];

		for (int i = 0; i < cells.Length; ++i)
			cells[i] = new StaticCell(i);


		// Add object to grid.
		for (int i = 0; i < _objects.Length; ++i)
		{
			StaticCell cell = GetCellFromPosition(_objects[i].transform.position);
			cell.objects.Add(_objects[i]);
		}
	}

	public sealed override List<ICell> QueryNeighbors(ICell cell)
	{
		List<ICell> result = new List<ICell>();

		Vector3Int coords = GetCellCoords(cell as StaticCell);


		if(coords.x - 1 > 0)
		{
			if (coords.y - 1 > 0)
			{
				if (coords.z - 1 > 0)
				{

				}
			}
		}


		return result;
	}

	public sealed override void ForEachCell(CellDelegate lambda)
	{
		for (uint i = 0u; i < cells.Length; ++i)
			lambda.Invoke(cells[i]);
	}

	private void OnDrawGizmos()
	{
		GridDrawMode drawMode = GetDrawMode();

		if (drawMode == GridDrawMode.None)
			return;

		// Draw cells.
		{
			List<StaticCell> filledCells = new List<StaticCell>();

			Gizmos.color = GetDebugColor(0, drawMode);

			ForEachCell((ICell cell) =>
			{
				StaticCell sCell = cell as StaticCell;

				if (sCell.objects.Count > 0)
					filledCells.Add(sCell);
				else
					Gizmos.DrawWireCube(GetCellPosition(sCell), _cellExtents);
			});


			// Draw filled cells.

			Gizmos.color = GetDebugColor(1, drawMode);

			foreach (var cell in filledCells)
				Gizmos.DrawWireCube(GetCellPosition(cell), _cellExtents);


			// Draw debug selected cell.
			if (_debugSelectedCell >= 0 && _debugSelectedCell < cells.Length)
			{
				Gizmos.color = GetDebugColor(2, drawMode);

				Gizmos.DrawWireCube(GetCellPosition(cells[_debugSelectedCell]), _cellExtents);
			}
		}

		// Draw center and bounds.
		if (_debugDrawCenter)
		{
			Gizmos.color = GetDebugColor(3, drawMode);
			Gizmos.DrawCube(transform.position, _cellExtents / 10.0f);
		}

		if (_debugDrawBounds)
		{
			Gizmos.color = GetDebugColor(4, drawMode);
			Gizmos.DrawLine(transform.position, minBound);
			Gizmos.DrawLine(transform.position, maxBound);
		}
	}
}
