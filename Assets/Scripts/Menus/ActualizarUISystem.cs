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
    public partial class ActualizarUISystem : SystemBase
    {
        private EntityManager entityManager;

        protected override void OnCreate()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnUpdate()
        {
            // Obtener la entidad del jugador
            Entity playerEntity = SystemAPI.GetSingletonEntity<DisparoData>();
            PlayerDañoData playerDañoData = entityManager.GetComponentData<PlayerDañoData>(playerEntity);

            // Obtener la referencia al componente PlayerUI
            PlayerUI playerUI = entityManager.GetComponentObject<PlayerUI>(playerEntity);

            // Actualizar el texto de la UI
            playerUI.nivelTexto.text = $"Nivel: {playerDañoData.nivelJugador}";
        }
    }
}

