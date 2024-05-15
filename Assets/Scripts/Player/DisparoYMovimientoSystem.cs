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
public partial struct DisparoYMovimientoSystem : ISystem
{
    private EntityManager _entityManager;
    private Entity _playerEntity;
    private Entity _inputEntity;

    private DisparoData _playerComponent;
    private InputMono _inputComponent;

    public float sensibilityX;
    public float sensibilityY;
    float xRotation, yRotation;
    float velocidadMovimiento;


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
        // Obtener el transform del jugador
        LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

        sensibilityX = 10f;
        sensibilityY = 4f;

        float mouseX = Input.GetAxisRaw("Mouse X") * SystemAPI.Time.DeltaTime * sensibilityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * SystemAPI.Time.DeltaTime * sensibilityY;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Limitar la rotación en el eje X entre -90 y 90 grados
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Obtener la entrada del teclado para movimiento
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calcular la dirección de movimiento utilizando los ejes Right y Forward del transform del jugador
        float3 moveDirection = math.mul(quaternion.Euler(0, yRotation, 0), new float3(horizontalInput, 0, verticalInput));

        // Verificar si la dirección de movimiento es un vector válido
        if (math.lengthsq(moveDirection) > 0f)
        {
            // Normalizar la dirección de movimiento para mantener una velocidad constante en todas las direcciones
            moveDirection = math.normalize(moveDirection);
        }

        // Calcular el desplazamiento basado en la dirección de movimiento y la velocidad
        // Detectar si la tecla LeftShift está siendo presionada
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Ajustar la velocidad de movimiento
        velocidadMovimiento = isSprinting ? 25f : 15f;


        float3 movimiento = moveDirection * velocidadMovimiento * SystemAPI.Time.DeltaTime;

        // Actualizar la posición del jugador sumando el desplazamiento
        playerTransform.Position += movimiento;

        // Actualizar la rotación del jugador
        playerTransform.Rotation = quaternion.Euler(0, yRotation, 0);

        // Actualizar el componente de transformación de la entidad del jugador
        _entityManager.SetComponentData(_playerEntity, playerTransform);

        // Actualizar la posición y rotación de la cámara
        var cameraSingleton = CameraSingleton.Instance;
        if (cameraSingleton != null)
        {
            // La cámara sigue exactamente la posición y rotación del jugador
            cameraSingleton.transform.position = playerTransform.Position;
            cameraSingleton.transform.rotation = quaternion.Euler(xRotation, yRotation, 0);
        }

        

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
                    velocidadBala = 44f,
                    dañoBala = 5f
                });

                entityCommandBuffer.AddComponent(bulletEntity, new BalasTiempoMono
                {
                    balasTiempoDesaparicion = 4.0f
                });

                LocalTransform balasTransform = _entityManager.GetComponentData<LocalTransform>(bulletEntity);
                LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

                float offsetX = 1.3f;
                float offsetY = 1.3f;


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
