using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using Zombies;
using Unity.Collections;
using UnityEngine.EventSystems;


[BurstCompile]
public partial struct DisparoSystem : ISystem
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

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");


        Move(ref state);
        Disparar(ref state);
    }

    private void Move(ref SystemState state)
    {
        // Mover al jugador
        LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calcular la dirección de movimiento utilizando los ejes Right y Forward del transform del jugador
        float3 moveDirection = playerTransform.Right() * horizontalInput + playerTransform.Forward() * verticalInput;

        // Verificar si la dirección de movimiento es un vector válido
        if (math.lengthsq(moveDirection) > 0f)
        {
            // Normalizar la dirección de movimiento para mantener una velocidad constante en todas las direcciones
            moveDirection = math.normalize(moveDirection);
        }
        else
        {
            // Si la dirección de movimiento es cero, no hacemos ningún movimiento
            return;
        }

        // Calcular el desplazamiento basado en la dirección de movimiento y la velocidad
        float3 movimiento = moveDirection * 5f * Time.deltaTime;

        // Actualizar la posición del jugador sumando el desplazamiento
        playerTransform.Position += movimiento;

        // Actualizar el componente de transformación de la entidad del jugador
        _entityManager.SetComponentData(_playerEntity, playerTransform);

    }


    private void Disparar(ref SystemState state)
    {

        if (_inputComponent.disparoIniciar)
        {
            for (int i = 0; i < _playerComponent.numeroBalasPorDisparo; i++)
            {
                EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

                Entity bulletEntity = _entityManager.Instantiate(_playerComponent.balaPrefab);


                entityCommandBuffer.AddComponent(bulletEntity, new BalasData
                {
                    velocidadBala = 30f,
                    dañoBala = 10f
                });

                entityCommandBuffer.AddComponent(bulletEntity, new BalasTiempoMono
                {
                    balasTiempoDesaparicion = 4.0f
                });

                LocalTransform balasTransform = _entityManager.GetComponentData<LocalTransform>(bulletEntity);
                LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

                // Obtener la posición y dirección del punto de disparo del jugador
                LocalTransform puntoDisparoTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

                float offsetY = 1.2f;

                // Colocar la bala en la posición del punto de disparo y dirigirla hacia adelante
                balasTransform.Position.x = playerTransform.Position.x;
                balasTransform.Position.y = playerTransform.Position.y + offsetY;
                balasTransform.Position.z = playerTransform.Position.z;

                balasTransform.Rotation = playerTransform.Rotation;

                // Agregar un desplazamiento aleatorio en el plano XY
                float2 randomOffset = UnityEngine.Random.insideUnitCircle * _playerComponent.balasSpread;
                float3 offset = new(randomOffset.x, randomOffset.y, 0);

                // Sumar el desplazamiento al punto de disparo para dispersar las balas
                balasTransform.Position += math.mul(playerTransform.Rotation, offset);

                entityCommandBuffer.SetComponent(bulletEntity, balasTransform);

                entityCommandBuffer.Playback(_entityManager);
            }
        }

    }
}
