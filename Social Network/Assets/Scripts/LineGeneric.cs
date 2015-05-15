using UnityEngine;
using System.Collections;

public static class LineGeneric {
	
	public static GameObject CreateLineMesh(Vector3 p1, Vector3 p2, float width, float distFromEnds, float distDepth)
	{
		GameObject lineMesh = new GameObject("lineMesh");
		Mesh newMesh = new Mesh();
		lineMesh.AddComponent<MeshFilter>();
		lineMesh.AddComponent<MeshRenderer>();
		
		Vector3 widthDirection = (Vector3.Cross (p1 - p2, Vector3.back).normalized) * width;
		Vector3 depthDirection = Vector3.forward;
		
		// Set positions of verts
		Vector3 backBottom = (p1 - distFromEnds*(p1 - p2)) + widthDirection + depthDirection*distDepth;
		Vector3 backTop = (p1 - distFromEnds*(p1 - p2)) - widthDirection + depthDirection*distDepth;
		Vector3 frontBottom = (p2 - distFromEnds*(p2 - p1)) + widthDirection + depthDirection*distDepth;
		Vector3 frontTop = (p2 - distFromEnds*(p2 - p1)) - widthDirection + depthDirection*distDepth;
		
		Vector3[] verts = new Vector3[4] { backBottom, backTop, frontBottom, frontTop };
		newMesh.vertices = verts;
		
		// Set the UVs
		Vector2[] uvs = new Vector2[newMesh.vertices.Length];
		for (int i = 0; i < uvs.Length; i++)
		{
			uvs[i] = new Vector2(newMesh.vertices[i].x, newMesh.vertices[i].z);
		}
		newMesh.uv = uvs;
		
		int[] triVerts = new int[6]	{ 0,1,2,2,1,3 };
		
		newMesh.triangles = triVerts;
		newMesh.RecalculateNormals();
		
		lineMesh.GetComponent<MeshFilter>().mesh = newMesh;
		
		return lineMesh;
	}

}
