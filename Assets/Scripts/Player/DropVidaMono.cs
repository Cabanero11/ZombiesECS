using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.VisualScripting;
using UnityEngine;


namespace Zombies
{
    public class DropVidaMono : MonoBehaviour
    {
        public GameObject dropVida;
        public float vidaRecuperada;

    }

    // Añado los valores de DisparoData a la entidad de PlayerEntity con el Baker
    public class DropVidaBaker : Baker<DropVidaMono>
    {
        public override void Bake(DropVidaMono authoring)
        {
            var dropVidaEntidad = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(dropVidaEntidad, new DropVidaData
            {
                dropVida = GetEntity(authoring.dropVida, TransformUsageFlags.Dynamic),
            });
        }


    }

}

