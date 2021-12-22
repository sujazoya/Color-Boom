using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropTriangleObstacle : MonoBehaviour {

    public MeshRenderer[] triangleParts;
    public MeshRenderer[] innerObstacleParts;



    public void setTriangleColors()
    {

        if (GameController.Instance == null) return;

        int exitPart = UnityEngine.Random.Range(0, 3);
        Material exitPartMat = GameController.Instance.selectedMaterial;
        triangleParts[exitPart].material = exitPartMat;


        List<Material> otherMats = new List<Material>();
        foreach (Material mat in GameController.Instance.gameMaterials)
        {
            if (!mat.name.ToLower().Equals(exitPartMat.name.ToLower()))
            {
                otherMats.Add(mat);
            }
        }

     
        for (int i = 0; i < 3; i++)
        {
            if (i != exitPart)
            {
                int selMat = UnityEngine.Random.Range(0, otherMats.Count);
                triangleParts[i].material = otherMats[selMat];
                otherMats.RemoveAt(selMat);
            }
        }

    }

}
