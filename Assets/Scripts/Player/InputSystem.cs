using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;



namespace Zombies
{
    // SYSTEM BASE PARA INPUT: es un sistema que no usa el BurstCompiler, pero para input da igual
    public partial class InputSystem : SystemBase
    {
        private InputPlayerECS inputPlayerECS;

        protected override void OnCreate()
        {
            // Si no hay InputMono, crear una entidad de este
            if (!SystemAPI.TryGetSingleton(out InputMono input))
            {
                EntityManager.CreateEntity(typeof(InputMono));
            }

            inputPlayerECS = new InputPlayerECS();
            inputPlayerECS.Enable();
        }

        protected override void OnUpdate()
        {
            bool disparo = inputPlayerECS.Player.Disparar.ReadValue<bool>();

            SystemAPI.SetSingleton(new InputMono
            {
                disparoIniciar = disparo
            });
        }
    }



}
