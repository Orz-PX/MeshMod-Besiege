using UnityEngine;
using System.Collections;
using Modding;
using Modding.Levels;

public class MeshEntityScript :MonoBehaviour
{

    public MeshEntityData meshEntityData;
    public Entity entity;

    MSlider redSlider;
    MSlider greenSlider;
    MSlider blueSlider;
    MSlider alphaSlider;

    Material material;

    void Start()
    {
        if (!StatMaster.levelSimulating)
        {
    
            if (meshEntityData == null)
            {
                meshEntityData = new MeshEntityData(entity.Id, new Color(255,255,255,255));
            }

            redSlider= entity.InternalObject.EntityBehaviour.AddSlider("Red", "Red", meshEntityData.Color.r, 0, 255);           
            greenSlider = entity.InternalObject.EntityBehaviour.AddSlider("Green", "Green", meshEntityData.Color.g, 0, 255);          
            blueSlider = entity.InternalObject.EntityBehaviour.AddSlider("Blue", "Blue", meshEntityData.Color.b, 0, 255);           
            alphaSlider = entity.InternalObject.EntityBehaviour.AddSlider("Alpha", "Alpha", meshEntityData.Color.a, 0, 255);

            redSlider.ValueChanged += (value) => { ChandedPropertise(); };
            greenSlider.ValueChanged += (value) => { ChandedPropertise(); };
            blueSlider.ValueChanged += (value) => { ChandedPropertise(); };
            alphaSlider.ValueChanged += (value) => { ChandedPropertise(); };

            material = entity.InternalObject.visualController.renderers[0].material;
            
            ChandedPropertise();
        }
      

    }

    void ChandedPropertise()
    {
        Color color = new Color(redSlider.Value / 255f, greenSlider.Value / 255f, blueSlider.Value / 255f, alphaSlider.Value / 255f);
        meshEntityData.Color = material.color = color;
        if (color.a >= 1)
        {
            material.shader = Shader.Find("Diffuse");
        }
        else
        {
            material.shader = Shader.Find("Transparent/Diffuse");
        }
    }

    void Update()
    {

    }
}
