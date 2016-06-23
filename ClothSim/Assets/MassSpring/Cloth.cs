using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MassSpring
{
    /// see http://cg.alexandra.dk/?p=147 .i study base this article.
    public class Cloth : MonoBehaviour
    {

        int mBoneXCount = 10;
        int mBoneYCount = 3;

        List<Transform> mBones = new List<Transform>();
        List<ParticleBone> mParticleBones = new List<ParticleBone>();

        List<Constraint> mConstraints = new List<Constraint>();

        List<Vector3> vertexArray = new List<Vector3>();
        List<Vector3> uvArray = new List<Vector3>();
        List<int> triangleArray = new List<int>();

        Material material;

        // Use this for initialization
        void Start()
        {
            material = new Material(Shader.Find("Diffuse"));
            BuildBone();
            BuildMesh();
            BuildParticle();
            BuildConstraint();
        }

        void Update()
        {
            for (int step = 0; step < 15; step++)
            {
                for (int i = 0; i < mConstraints.Count; ++i)
                {
                    mConstraints[i].satisfyConstraint();
                }
            }

            for (int i = 0; i < mParticleBones.Count; ++i)
            {
                mParticleBones[i].timeStep();
            }

            BuildMesh();
        }

        void BuildParticle()
        {
            for (int i = 0; i < mBones.Count; ++i)
            {
                ParticleBone particle = mBones[i].gameObject.AddComponent<ParticleBone>();
                mParticleBones.Add(particle);
            }
        }

        void BuildConstraint()
        {
            for (int x = 0; x < mBoneXCount; ++x)
            {
                for (int y = 0; y < mBoneYCount ; ++y)
                {
                    if (x < mBoneXCount - 1) makeConstraint(getParticle(x, y), getParticle(x + 1, y));
                    if (y < mBoneYCount - 1) makeConstraint(getParticle(x, y), getParticle(x, y + 1));
                    if (x < mBoneXCount - 1 && y < mBoneYCount - 1) makeConstraint(getParticle(x, y), getParticle(x + 1, y + 1));
                    if (x < mBoneXCount - 1 && y < mBoneYCount - 1) makeConstraint(getParticle(x + 1, y), getParticle(x, y + 1));
                }
            }

            for (int x = 0; x < mBoneXCount; x++)
            {
                for (int y = 0; y < mBoneYCount; y++)
                {
                    if (x < mBoneXCount - 2) makeConstraint(getParticle(x, y), getParticle(x + 2, y));
                    if (y < mBoneYCount - 2) makeConstraint(getParticle(x, y), getParticle(x, y + 2));
                    if (x < mBoneXCount - 2 && y < mBoneYCount - 2) makeConstraint(getParticle(x, y), getParticle(x + 2, y + 2));
                    if (x < mBoneXCount - 2 && y < mBoneYCount - 2) makeConstraint(getParticle(x + 2, y), getParticle(x, y + 2));
                }
            }

            for (int i = 0; i < 3; i++)
            {
               // getParticle(0 + i, 0).offsetPos(new Vector3(0.5f, 0.0f, 0.0f)); // moving the particle a bit towards the center, to make it hang more natural - because I like it ;)
                getParticle(0 + i, 0).makeUnmovable();

                //getParticle(0 + i, 0).offsetPos(new Vector3(-0.5f, 0.0f, 0.0f)); // moving the particle a bit towards the center, to make it hang more natural - because I like it ;)
                getParticle(mBoneXCount - 1 - i, 0).makeUnmovable();
            }



        }
        void makeConstraint(ParticleBone p1, ParticleBone p2)
        {
            mConstraints.Add(new Constraint(p1, p2));
        }

        ParticleBone getParticle(int xIndex, int yIndex)
        {
            return mParticleBones[BoneIndex(xIndex,yIndex)];
        }

        void BuildBone()
        {
            mBones.Clear();
            for (int i = 0; i < mBoneXCount; ++i)
            {
                for (int j = 0; j < mBoneYCount; ++j)
                {
                    Transform bone = new GameObject(string.Format("{0}_{1}", i, j)).transform;
                    bone.transform.parent = this.transform;
                    bone.transform.localRotation = Quaternion.identity;
                    bone.transform.localScale = Vector3.one;
                    bone.transform.localPosition = new Vector3(i, j, 0);
                    mBones.Add(bone);

                }
            }


        }


        int BoneIndex(int xIndex, int yIndex)
        {
            return yIndex + xIndex * mBoneYCount;
        }


        void BuildMesh()
        {
            SkinnedMeshRenderer rend = GetComponent<SkinnedMeshRenderer>();
            if (rend == null)
            {
                rend = gameObject.AddComponent<SkinnedMeshRenderer>();
                rend.material = material;
            }

            Mesh mesh = rend.sharedMesh;
            if (mesh == null)
            {
                mesh = new Mesh();
                rend.sharedMesh = mesh;
            }

            vertexArray.Clear();
            uvArray.Clear();
            triangleArray.Clear();

            for (int i = 0; i < mBoneXCount - 1; ++i)
            {
                for (int j = 0; j < mBoneYCount - 1; ++j)
                {
                    Transform bone0 = mBones[BoneIndex(i, j)]; 
                    Transform bone1 = mBones[BoneIndex(i+1, j)];
                    Transform bone2 = mBones[BoneIndex(i, j+1)];
                    Transform bone3 = mBones[BoneIndex(i+1, j+1)];

                    int index1 = vertexArray.Count;
                    vertexArray.Add(bone0.position);
                    uvArray.Add(new Vector2(0, 0));

                    int index2 = vertexArray.Count;
                    vertexArray.Add(bone1.position);
                    uvArray.Add(new Vector2(1, 0));

                    int index3 = vertexArray.Count;
                    vertexArray.Add(bone2.position);
                    uvArray.Add(new Vector2(0, 1));

                    int index4 = vertexArray.Count;
                    vertexArray.Add(bone3.position);
                    uvArray.Add(new Vector2(1, 1));

                    triangleArray.Add(index1);
                    triangleArray.Add(index2);
                    triangleArray.Add(index3);

                    triangleArray.Add(index2);
                    triangleArray.Add(index4);
                    triangleArray.Add(index3);
  
                }
            }

            mesh.SetVertices(vertexArray);
            mesh.SetUVs(0, uvArray);
            mesh.SetTriangles(triangleArray, 0);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            mesh.UploadMeshData(false);
        }
    }
}

