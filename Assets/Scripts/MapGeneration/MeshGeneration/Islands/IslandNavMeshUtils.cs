using System;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public static class IslandNavMeshUtils
{
    public static IEnumerator BuildNavMeshAsync(Transform transform, NavMeshSurface surface, Action callback)
	{
        // remove all data otherwise UpdateNavMesh doesn't create/update the navmesh (I don't know why)
		surface.navMeshData = null;

		NavMeshData data = new NavMeshData();
		data.position = transform.position; // the navmesh gets generated way above the actual mesh so set the position to avoid it
		data.name = transform.name;

		AsyncOperation operation = surface.UpdateNavMesh(data);

		while (!operation.isDone)
		{
			yield return null;
		}
		
		surface.navMeshData = data;
		surface.AddData();

		yield return null;

		callback.Invoke();
	}
}