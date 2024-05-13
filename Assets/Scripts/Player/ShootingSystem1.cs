using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;


[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct DisparoSystem : ISystem
{

    private BuildPhysicsWorld physicsWorldSystem;

    [BurstCompile]
    public void OnCreate(ref SystemState state) 
    {
       
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }




    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var zombiesEntities in SystemAPI.Query<ZombiesOleadasAspect>().WithAll<ZombiesTag>()) { }
        {

            RaycastInput input = new RaycastInput
            {

                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u, // Collides with everything
                    CollidesWith = ~0u,
                    GroupIndex = 0
                }
            };

            
        }

    
    }


}

