using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Collider = Unity.Physics.Collider;
using Unity.Transforms;
using Unity.Profiling;
using Unity.Burst;
using System;
using Components;
using UnityEditor;
using Unity.Collections.LowLevel.Unsafe;

//public struct TriggerGravityFactor : IComponentData
//{
//    public float GravityFactor;
//    public float DampingFactor;
//}
//
//public class TriggerGravityFactorBehaviour : MonoBehaviour, IConvertGameObjectToEntity
//{
//    public float GravityFactor = 0f;
//    public float DampingFactor = 0.9f;
//
//    void OnEnable() { }
//
//    void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        if (enabled)
//        {
//            dstManager.AddComponentData(entity, new TriggerGravityFactor()
//            {
//                GravityFactor = GravityFactor,
//                DampingFactor = DampingFactor,
//            });
//        }
//    }
//}


// This system sets the PhysicsGravityFactor of any dynamic body that enters a Trigger Volume.
// A Trigger Volume is defined by a PhysicsShapeAuthoring with the `Is Trigger` flag ticked and a
// TriggerGravityFactor behaviour added.
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class TriggerGravityFactorSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;

    protected override void OnCreate()
    {
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    struct TriggerGravityFactorJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<TriggerComponent> TriggerGravityFactorGroup;
        public ComponentDataFromEntity<PhysicsGravityFactor> PhysicsGravityFactorGroup;
        public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;
       

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.Entities.EntityA;
            Entity entityB = triggerEvent.Entities.EntityB;

            bool isBodyATrigger = !TriggerGravityFactorGroup.Exists(entityA);
            bool isBodyBTrigger = !TriggerGravityFactorGroup.Exists(entityB);

            // Ignoring Triggers overlapping other Triggers
            if (isBodyATrigger && isBodyBTrigger)
                return;

            bool isBodyADynamic = PhysicsVelocityGroup.Exists(entityA);
            bool isBodyBDynamic = PhysicsVelocityGroup.Exists(entityB);

            // Ignoring overlapping static bodies
            if ((isBodyATrigger && !isBodyBDynamic) ||
                (isBodyBTrigger && !isBodyADynamic))
                return;

            var triggerEntity = isBodyATrigger ? entityA : entityB;
            var dynamicEntity = isBodyATrigger ? entityB : entityA;
            
            var component = TriggerGravityFactorGroup[dynamicEntity];
            component.Trigger = true;
            TriggerGravityFactorGroup[dynamicEntity] = component; 
            
             
            
//            var triggerGravityComponent = TriggerGravityFactorGroup[triggerEntity];
//            // tweak PhysicsGravityFactor
//            {
//                var component = PhysicsGravityFactorGroup[dynamicEntity];
//                component.Value = triggerGravityComponent.GravityFactor;
//                PhysicsGravityFactorGroup[dynamicEntity] = component;
//            }
//            // damp velocity
//            {
//                var component = PhysicsVelocityGroup[dynamicEntity];
//                component.Linear *= triggerGravityComponent.DampingFactor;
//                PhysicsVelocityGroup[dynamicEntity] = component;
//            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle jobHandle = new TriggerGravityFactorJob
        {
            TriggerGravityFactorGroup = GetComponentDataFromEntity<TriggerComponent>() ,
            PhysicsGravityFactorGroup = GetComponentDataFromEntity<PhysicsGravityFactor>(),
            PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
        }.Schedule(m_StepPhysicsWorldSystem.Simulation, 
                    ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);
        
     


        return jobHandle;
    }
}