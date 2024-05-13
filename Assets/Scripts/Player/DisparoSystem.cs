using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;
using Unity.Collections;


[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct PlayerSystem : ISystem
{
    private EntityManager _entityManager;
    private Entity _playerEntity;
    private Entity _inputEntity;

    private DisparoData _playerComponent;
    private InputMono _inputComponent;

    public void OnUpdate(ref SystemState state)
    {
        //references
        _entityManager = state.EntityManager;
        _playerEntity = SystemAPI.GetSingletonEntity<DisparoData>();
        _inputEntity = SystemAPI.GetSingletonEntity<InputMono>();

        //components
        _playerComponent = _entityManager.GetComponentData<DisparoData>(_playerEntity);
        _inputComponent = _entityManager.GetComponentData<InputMono>(_inputEntity);

        Move(ref state);
        Disparar(ref state);
    }

    private void Move(ref SystemState state)
    {
        // Mover al jugador
        LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);
       

        _entityManager.SetComponentData(_playerEntity, playerTransform);
    }

    private void Disparar(ref SystemState state)
    {

        if (_inputComponent.disparoIniciar)
        {
            for (int i = 0; i < c_PlayerComponent.NumOfBulletsToSpawn; i++)
            {
                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);

                Entity bulletEntity = _entityManager.Instantiate(c_PlayerComponent.BulletPrefab);

                ECB.AddComponent(bulletEntity, new BulletComponent
                {
                    Speed = 25f
                });

                ECB.AddComponent(bulletEntity, new BulletLifeTimeComponent
                {
                    RemainingLifetime = 1.5f
                });

                LocalToWorld bulletTransform = _entityManager.GetComponentData<LocalToWorld>(bulletEntity);

                bulletTransform.Rotation = playerTransform.Rotation;

                float randomOffset = UnityEngine.Random.Range(-c_PlayerComponent.BulletSpread, c_PlayerComponent.BulletSpread);

                bulletTransform.Position += playerTransform.Position + (playerTransform.Right() * 1.65f) + (bulletTransform.Up() * randomOffset);

                ECB.SetComponent(bulletEntity, bulletTransform);

                ECB.Playback(_entityManager);
            }
        }

    }
}
