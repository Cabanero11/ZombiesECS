using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;
using Unity.Collections;
using Unity.VisualScripting;

namespace Zombies
{

    public partial class CamaraSystem : SystemBase
    {
        private EntityManager entityManager;


        protected override void OnCreate()
        {
            
        }

        protected override void OnUpdate()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var jugadorEntidad = SystemAPI.GetSingletonEntity<DisparoData>();

            LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(jugadorEntidad);

            var cameraSingleton = CameraSingleton.Instance;

            if (cameraSingleton == null) 
                return;


            // Ajustar la posición de la cámara para estar a la altura del jugador
            Vector3 cameraPosition = playerTransform.Position + new float3(0f, cameraSingleton.AlturaSobreJugador, -cameraSingleton.DistanciaDetrasJugador);
            cameraSingleton.transform.SetPositionAndRotation(cameraPosition, playerTransform.Rotation);
        }
    }
}


