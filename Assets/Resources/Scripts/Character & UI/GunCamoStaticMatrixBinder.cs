using UnityEngine;

public class GunCamoStaticMatrixBinder : MonoBehaviour
{
    public Renderer[] camoRenderers;

    void Awake()
    {
        // Use IDENTITY matrix for fully static space
        Matrix4x4 M = Matrix4x4.identity;

        foreach (var r in camoRenderers)
        {
            Material[] mats = r.materials; // forces instancing

            foreach (var mat in mats)
            {
                mat.SetVector("_StaticRow0", M.GetRow(0));
                mat.SetVector("_StaticRow1", M.GetRow(1));
                mat.SetVector("_StaticRow2", M.GetRow(2));
                mat.SetVector("_StaticRow3", M.GetRow(3));
            }

            r.materials = mats; // reassign instances
        }
    }
}
