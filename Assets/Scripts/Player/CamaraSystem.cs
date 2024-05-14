using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;
using Unity.Collections;


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

            // Obtiene la posición detrás del jugador
            var vectorNegativo = new float3(-1f, -1f, -1f);

            float3 forward = math.mul(playerTransform.Rotation, new float3(0, 0, 1));
            float3 direccionDetrasJugador = -forward * cameraSingleton.DistanciaDetrasJugador;
            float3 posicionCamara = playerTransform.Position + direccionDetrasJugador + new float3(0, cameraSingleton.AlturaSobreJugador, 0);

            cameraSingleton.transform.position = posicionCamara;
            cameraSingleton.transform.LookAt(playerTransform.Position, Vector3.up);
        }

        // Espero que funcione asi xd
        public EntityManager ObtenerEntityManager(ref SystemState state)
        {
            entityManager = state.EntityManager;

            return entityManager;

        }

    }
}

