using Cynteract.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ProceduralMesh : ScriptableObject {
    
     MeshRenderer meshRenderer;
     MeshFilter meshFilter;
    // Use this for initialization


    public static Mesh Landscape2D( float stepdistance, float[] heights, int interpolationSteps)
    {
        float[] newHeights = CArrayUtility.InterpolateArray(heights, interpolationSteps);

        return Landscape2D(stepdistance / interpolationSteps, newHeights);
    }



    public static Mesh Tunnel2D(float ceilling, float bottom, float stepDistance, float[] offsets, float[] diameters, int interpolationSteps) {
        float[] newOffsets = CArrayUtility.InterpolateArray(offsets, interpolationSteps);
        float[] newDiameters = CArrayUtility.InterpolateArray(diameters, interpolationSteps);
        return Tunnel2D(ceilling, bottom, stepDistance/(interpolationSteps+1), newOffsets, newDiameters);
    }
    public static Mesh Tunnel2D(float ceilling, float bottom, float stepDistance, float[] offsets, float[] diameters, int interpolationSteps, out Vector3[] spawnPoints)
    {
        float[] newOffsets = CArrayUtility.InterpolateArray(offsets, interpolationSteps);
        float[] newDiameters = CArrayUtility.InterpolateArray(diameters, interpolationSteps);
        spawnPoints = new Vector3[newOffsets.Length];
        for (int i = 0; i < newOffsets.Length; i++)
        {
            spawnPoints[i] = new Vector3 (i* stepDistance / (interpolationSteps + 1), newOffsets[i],0);
        }

        return Tunnel2D(ceilling, bottom, stepDistance / (interpolationSteps + 1), newOffsets, newDiameters);
    }

    public static void Tunnel2D(float ceilling, float bottom, float stepDistance, float[] offsets, float[] diameters, int interpolationSteps , float RandomDeviation,out Mesh upperPart, out Mesh lowerPart, out Vector3[] spawnPoints)
    {
        float[] upperOffsets = CArrayUtility.MapAddRandom(offsets, 0, RandomDeviation,1,1, ceilling, diameters );
        float[] lowerOffsets = CArrayUtility.MapAddRandom(offsets, -RandomDeviation, 0,1,1, bottom, diameters);


        float[] newOffsets = CArrayUtility.InterpolateArray(offsets, interpolationSteps);
        float[] newUpperOffsets = CArrayUtility.InterpolateArray(upperOffsets, interpolationSteps);
        float[] newLowerOffsets = CArrayUtility.InterpolateArray(lowerOffsets, interpolationSteps);

        float[] newDiameters = CArrayUtility.InterpolateArray(diameters, interpolationSteps);
        spawnPoints = new Vector3[newOffsets.Length];
        for (int i = 0; i < newOffsets.Length; i++)
        {
            spawnPoints[i] = new Vector3(i * stepDistance / (interpolationSteps + 1), newOffsets[i], 0);
        }

        lowerPart = bottomMesh(ceilling, bottom, stepDistance / (interpolationSteps + 1), newLowerOffsets, newDiameters);
        upperPart = ceillingMesh(ceilling, bottom, stepDistance / (interpolationSteps + 1), newUpperOffsets, newDiameters);
    }
    public static void Tunnel2D(float ceilling, float bottom, float stepDistance, float[] offsets, float[] diameters, out Mesh upperPart, out Mesh lowerPart)
    {
        lowerPart = bottomMesh(ceilling, bottom, stepDistance, offsets, diameters);
        upperPart= ceillingMesh(ceilling, bottom, stepDistance, offsets, diameters);
    }

    private static Mesh ceillingMesh(float ceilling, float bottom, float stepDistance, float[] offsets, float[] diameters)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        float[] positions = new float[offsets.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = (ceilling + bottom) / 2 + offsets[i];
        }
        for (int i = 0; i < positions.Length; i++)
        {
            verts.Add(new Vector3(i * stepDistance, ceilling, 0));
            verts.Add(new Vector3(i * stepDistance, positions[i] + diameters[i] / 2, 0));
            if (i >= 1)
            {
                int index = i * 2 + 1;
                tris.AddRange(new List<int> { index  - 3  , index-1,     index,
                                              index  - 3  , index      ,  index-2
                }
                );
            }
        }

        mesh.SetVertices(verts);
        mesh.normals = new Vector3[verts.Count];
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        return mesh;
    }

    private static Mesh bottomMesh(float ceilling, float bottom, float stepDistance, float[] offsets, float[] diameters)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        float[] positions = new float[offsets.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = (ceilling + bottom) / 2 + offsets[i];
        }
        for (int i = 0; i < positions.Length; i++)
        {
            verts.Add(new Vector3(i * stepDistance, bottom, 0));
            verts.Add(new Vector3(i * stepDistance, positions[i] - diameters[i] / 2, 0));
            if (i >= 1)
            {
                int index = i * 2 + 1;
                tris.AddRange(new List<int> { index  - 3  , index   , index-1,
                                              index  - 3  , index-2 , index
                }
                );
            }
        }
        mesh.SetVertices(verts);
        mesh.normals = new Vector3[verts.Count];
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        return mesh;
    }

    public static Mesh Tunnel2D(float ceilling, float bottom, float stepDistance,float[] offsets, float[] diameters)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        float[] positions = new float[offsets.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = (ceilling + bottom) / 2 + offsets[i];
        }
        for (int i = 0; i < positions.Length; i++)
        {
            verts.Add(new Vector3(i * stepDistance, bottom, 0));
            verts.Add(new Vector3(i * stepDistance, positions[i]-diameters[i]/2, 0));
            if (i >= 1)
            {
                int index = i * 2 + 1;
                tris.AddRange(new List<int> { index  - 3  , index   , index-1,
                                              index  - 3  , index-2 , index
                }
                );
            }
        }
        int count = verts.Count;
        for (int i = 0; i < positions.Length; i++)
        {
            verts.Add(new Vector3(i * stepDistance, ceilling, 0));
            verts.Add(new Vector3(i * stepDistance, positions[i] + diameters[i]/2, 0));
            if (i >= 1)
            {
                int index = i * 2 + 1+count;
                tris.AddRange(new List<int> { index  - 3  , index-1,     index,
                                              index  - 3  , index      ,  index-2 
                }
                );
            }
        }
        
        mesh.SetVertices(verts);
        mesh.normals = new Vector3[verts.Count];
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        return mesh;

    }
    public static Mesh Landscape2D(float stepdistance, float[] heights)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        for (int i = 0; i < heights.Length; i++)
        {
            verts.Add(new Vector3(i * stepdistance, 0, 0));
            verts.Add(new Vector3(i * stepdistance, heights[i], 0));
            if (i >= 1)
            {
                int index= i*2+1;
                tris.AddRange(new List<int> { index  - 3  , index   , index-1,
                                              index  - 3  , index-2 , index
                }
                );
            }
        }
        mesh.SetVertices(verts);
        mesh.normals = new Vector3[heights.Length* 2];
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        return mesh;

    }

    public static Mesh Quad() {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[4];
        mesh.triangles = new int[6];
        mesh.normals = new Vector3[mesh.vertices.Length];

        mesh.SetVertices(new List<Vector3>() { Vector3.zero , new Vector3(0, 1, 0), new Vector3(0,0,1), new Vector3(0,1,1)});
        mesh.SetTriangles(new int[] { 0, 3, 2, 3, 0, 1}, 0);

        mesh.RecalculateNormals();
        return mesh;

    }
    public static Mesh Quad(Vector3 lU, Vector3 lo, Vector3 ru, Vector3 ro) {

        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[4];
        mesh.triangles = new int[6];
        mesh.normals = new Vector3[mesh.vertices.Length];

        mesh.SetVertices(new List<Vector3>() {lU,lo,ru, ro });
        mesh.SetTriangles(new int[] { 0, 3, 2, 3, 0, 1 }, 0);

        mesh.RecalculateNormals();
        return mesh;


    }
    public static Mesh Cube(Vector3 lu, Vector3 lo, Vector3 ru, Vector3 ro, Vector3 luh, Vector3 loh, Vector3 ruh, Vector3 roh ) {
        Mesh mesh = Merge(
                Quad(lo,loh,ro,roh),//up
                Quad(lu,lo,ru,ro),//front
                Quad(ru, ro, ruh, roh),//right
                Quad(ruh, roh, luh, loh),//back
                Quad(luh, loh, lu,lo),//left
                Quad(luh, lu, ruh, ru)//bottom
            );
        return mesh;
            
    }
    public static Mesh Circle(Vector3 center, Vector3 normal, float radius, int steps) {
        List<Vector3> verts = new List<Vector3>();
        //verts.Add(center);#
        Vector3 orthoNormal;
        if (normal.z!=0)
        {
            orthoNormal=new Vector3(1, 0, normal.x / (-normal.z));
        }
        else if (normal.y != 0)
        {
            orthoNormal = new Vector3(1, -normal.x/normal.y);
        }
        else if (normal.x!=0)
        {
            orthoNormal = new Vector3( -normal.y / normal.x,1,0);
        }
        else
        {
            orthoNormal=Vector3.up;
        }
        for (int i = 0; i <steps+1; i++)
        {
            verts.Add(center + radius * (Quaternion.AngleAxis((i/(float)steps)*360,normal) *orthoNormal) );
        }
        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        return mesh;
    }
    public static Mesh StraightTunnel(Vector3 start, Vector3 normal, float radius, int steps, int segments, float segmentDistance) {
        Mesh[] circles=new Mesh[segments];
        for (int i = 0; i < segments; i++)
        {
            circles[i] = Circle(start + i * segmentDistance * normal, normal, radius, steps);
            
        }
        Mesh mesh = Merge(circles);
        List<int> tris = new List<int>();
        for (int j = 1; j < segments; j++)
        {
            for (int i = 1; i < steps+segments; i++)
            {
                tris.AddRange(new List<int> { (i - 1) + (j - 1) * steps,  (i - 1) + j * steps, i + (j - 1) * steps,i + (j - 1) * steps , (i - 1) + j * steps, i + j * steps });

            }
        }

        mesh.normals = new Vector3[mesh.vertices.Length];

        mesh.SetTriangles(tris,0);
        mesh.RecalculateNormals();
        return mesh;
    }
    public static Mesh TunnelWithIndividualCircles( float[] radius, int steps, Vector3[] segmentPositions, Vector3[] segmentNormals)
    {
        Mesh[] circles = new Mesh[segmentPositions.Length];
        for (int i = 0; i < segmentPositions.Length; i++)
        {
            circles[i] = Circle(segmentPositions[i], segmentNormals[i], radius[i], steps);

        }
        Mesh mesh = Merge(circles);
        List<int> tris = new List<int>();
        for (int j = 1; j < segmentPositions.Length; j++)
        {
            for (int i = 1; i < steps + segmentPositions.Length; i++)
            {
                tris.AddRange(new List<int> { (i - 1) + (j - 1) * steps, (i - 1) + j * steps, i + (j - 1) * steps, i + (j - 1) * steps, (i - 1) + j * steps, i + j * steps });

            }
        }

        mesh.normals = new Vector3[mesh.vertices.Length];
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        return mesh;

    }

    public static Mesh RandomTunnel(Vector3 start,int segments, float minRadius, float maxRadius, float minSegmentDistance, float MaxSegmentDistance,int steps, Vector3 direction) {
        float[] radius = new float[segments];
        Vector3[] segmentPositions = new Vector3[segments];
        Vector3[] segmentNormals = new Vector3[segments];
        segmentPositions[0] = start;
        for (int i = 1; i < segments; i++)
        {
            radius[i] = Random.Range(minRadius, minRadius);
            segmentPositions[i] = segmentPositions[i - 1] + Random.Range(minSegmentDistance, MaxSegmentDistance) * direction;
            segmentNormals[i] = Quaternion.Euler(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))*direction;
        }
        return TunnelWithIndividualCircles(radius, steps, segmentPositions, segmentNormals);
    }
    public static Mesh FilledCircle(Vector3 center, Vector3 normal, float radius, int steps) {
        List<Vector3> verts = new List<Vector3>();
        verts.Add(center);
        Mesh mesh = Circle( center,  normal,  radius,  steps);
        verts.AddRange(mesh.vertices);
        List<int> tris = new List<int>();
        for (int i = 2; i < steps+2; i++)
        {
            tris.AddRange(new List<int>() { 0, i, i - 1 });
        }
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris,0);
        mesh.RecalculateNormals();
        return mesh;
    }
    public static Mesh Cube(Vector3 center, Quaternion rotation, Vector3 scale) {
        return Cube(
            center + rotation * new Vector3(-scale.x, -scale.y, -scale.z),
            center + rotation * new Vector3(-scale.x,  scale.y, -scale.z),
            center + rotation * new Vector3( scale.x, -scale.y, -scale.z),
            center + rotation * new Vector3( scale.x,  scale.y, -scale.z),
            center + rotation * new Vector3(-scale.x, -scale.y,  scale.z),
            center + rotation * new Vector3(-scale.x,  scale.y,  scale.z),
            center + rotation * new Vector3( scale.x, -scale.y,  scale.z),
            center + rotation * new Vector3( scale.x,  scale.y,  scale.z)
            );

    }

    public static Mesh Grid(int x, int y, float distanceX, float distanceY, Vector3 lu, float[] heights) {

        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                verts.Add(lu + new Vector3(i * distanceY, heights[i * j],j * distanceX) );
                if (i > 0 && j > 0) {
                    tris.AddRange(new List<int>
                    {
                        (i-1)*x+j-1,     (i-1)*x+(j), i*x+j,
                        (i-1)*x+j-1,  i*x+j, i*x+j-1
                    }
                        
                        
                        
                        );

                }
            }
        }
        mesh.SetVertices(verts);
        mesh.normals = new Vector3[x * y];
        mesh.SetTriangles(tris,0);
        mesh.RecalculateNormals();
        return mesh;
    }
    public static Mesh Merge(params Mesh[] meshes) {
        int offset=0;
        Mesh mesh = new Mesh();
        List<Vector3> tempVerts = new List<Vector3>();

        List<int> tempTris = new List<int>();
        List<Vector3> tempNormals = new List<Vector3>();
        for (int i = 0; i < meshes.Length; i++)
            
        {
            tempVerts.AddRange(meshes[i].vertices);
            int[] offsetTris = meshes[i].triangles;
            for (int j = 0; j < offsetTris.Length; j++)
            {
                offsetTris[j] += offset;
            }
            tempTris.AddRange(offsetTris);
            for (int k = 0; k < offsetTris.Length; k++)
            {
            }

            tempNormals.AddRange(meshes[i].normals);
            offset = tempVerts.Count;
        }
        mesh.SetVertices(tempVerts);
        mesh.SetTriangles(tempTris,0);
        mesh.SetNormals(tempNormals);
        return mesh;

    }
}
