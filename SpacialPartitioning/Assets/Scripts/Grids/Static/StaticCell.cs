using System;
using System.Collections.Generic;

[Serializable]
public class StaticCell : ICell
{
	public readonly int id = 0;

	public List<GridObj> objects = new List<GridObj>();

	public StaticCell(int _id)
	{
		id = _id;
	}
}
