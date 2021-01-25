public class OctreePair
{
	public OctreeObj first = null;
	public OctreeObj second = null;

	public OctreePair()
	{
	}

	public OctreePair(OctreeObj _first, OctreeObj _second)
	{
		first = _first;
		second = _second;
	}

	public static bool Predicate(OctreePair _lhs, OctreePair _rhs)
	{
		return _lhs.first == _rhs.first && _lhs.second == _rhs.second ||
			_lhs.first == _rhs.second && _lhs.second == _rhs.first;
	}
}
