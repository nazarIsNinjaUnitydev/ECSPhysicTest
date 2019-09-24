using Unity.Entities;
using UnityEngine;

namespace Components
 {
     public class MoveComponentMono : MonoBehaviour, IConvertGameObjectToEntity
     {
         public float Speed;
         public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
         {
             dstManager.AddComponentData(entity, new MoveComponent()
             {
                 Speed = this.Speed
             });
             dstManager.AddComponentData(entity, new TriggerComponent()
             {
                 Trigger = false
             });
             
         }
     }
 }