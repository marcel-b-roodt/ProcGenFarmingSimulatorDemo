using Godot;
using System.Collections;

public static class NodeUtils
{
	public static Node2D GetClosestNode<T>(Vector2 position, IEnumerable nodes)
	{
		Node2D closestNode = null;
		foreach (Node2D node in nodes)
		{
			if (!(node is T))
				continue;

			if (closestNode == null)
				closestNode = node;
			else
			{
				if (position.DistanceSquaredTo(node.Position) > position.DistanceSquaredTo(closestNode.Position))
					closestNode = node;
			}
		}

		return closestNode;
	}
}
