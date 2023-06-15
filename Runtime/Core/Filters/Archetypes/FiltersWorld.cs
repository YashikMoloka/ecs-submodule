#if ENABLE_IL2CPP
#define INLINE_METHODS
#endif

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ME.ECS.Collections;
using Unity.IL2CPP.CompilerServices;
using Il2Cpp = Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute;

namespace ME.ECS {

    using FiltersArchetype;
    using Collections.LowLevel;
    using Collections.LowLevel.Unsafe;

    [Il2Cpp(Option.NullChecks, false)]
    [Il2Cpp(Option.ArrayBoundsChecks, false)]
    [Il2Cpp(Option.DivideByZeroChecks, false)]
    public partial class World {

        private MemArrayAllocator<FilterStaticData> filtersStaticData;
        private BufferArray<ListCopyable<ConnectInfoLambda>> filtersStaticDataLambdas;
        
        internal void OnSpawnFilters() { }

        internal void OnRecycleFilters() {

            for (int i = 0; i < this.filtersStaticDataLambdas.Length; ++i) {
                
                if (this.filtersStaticDataLambdas.arr[i] != null) PoolListCopyable<ConnectInfoLambda>.Recycle(ref this.filtersStaticDataLambdas.arr[i]);

            }
            PoolArray<ListCopyable<ConnectInfoLambda>>.Recycle(ref this.filtersStaticDataLambdas);

        }

        public void Register(ref MemoryAllocator allocator, ref FiltersArchetypeStorage storageRef, bool freeze, bool restore) {

            this.RegisterPluginsModuleForEntity();

            if (storageRef.isCreated == false) {

                storageRef = new FiltersArchetypeStorage();
                storageRef.Initialize(ref allocator, World.ENTITIES_CACHE_CAPACITY);
                storageRef.SetFreeze(freeze);

            }

            if (restore == true) {

                this.BeginRestoreEntities();

                // Update entities cache
                var list = PoolListCopyable<Entity>.Spawn(World.ENTITIES_CACHE_CAPACITY);
                if (this.ForEachEntity(list) == true) {

                    for (var i = 0; i < list.Count; ++i) {

                        ref var item = ref list[i];
                        // This call resets FilterData.dataVersions[item.id] to true which might create state desynchronization
                        // in case entity hadn't been updated on the previous tick. FilterData seems to have its state already
                        // stored within the main state, so it's possible that this call is not needed at all.
                        //this.UpdateFiltersOnFilterCreate(item);
                        this.CreateEntityPlugins(item, false);

                    }

                }

                PoolListCopyable<Entity>.Recycle(ref list);

                this.EndRestoreEntities();

            }

        }

        public void SetEntityCapacityInFilters(ref MemoryAllocator allocator, int capacity) {

            // On change capacity
            this.currentState.storage.SetCapacity(ref allocator, capacity);

        }

        public void CreateEntityInFilters(ref MemoryAllocator allocator, in Entity entity) {

            // On create new entity
            this.currentState.storage.SetCapacity(ref allocator, entity.id + 1);

        }

        public void RemoveFromAllFilters(ref MemoryAllocator allocator, in Entity entity) {

            // On destroy entity
            this.currentState.storage.Remove(ref allocator, in entity);

        }

        #if !ENTITIES_GROUP_DISABLED
        public void UpdateFilters(in EntitiesGroup group) {

            // Force to update entity group in filters
            this.currentState.storage.UpdateFilters(this.currentState, ref this.currentState.allocator);

        }
        #endif

        public void UpdateFilters(in Entity entity) {

            // Force to update entity in filters
            this.currentState.storage.UpdateFilters(this.currentState, ref this.currentState.allocator);

        }

        public void AddFilterByStructComponent(ref MemoryAllocator allocator, in Entity entity, int componentId, bool checkLambda) {

            this.currentState.storage.Set(ref allocator, in entity, componentId, checkLambda);

        }

        public void RemoveFilterByStructComponent(ref MemoryAllocator allocator, in Entity entity, int componentId, bool checkLambda) {

            this.currentState.storage.Remove(ref allocator, in entity, componentId, checkLambda);

        }

        public void ValidateFilterByStructComponent(ref MemoryAllocator allocator, in Entity entity, int componentId, bool makeRequest = false) {

            this.currentState.storage.Validate(ref allocator, in entity, componentId, makeRequest);

        }

        public void ValidateFilterByStructComponent<T>(ref MemoryAllocator allocator, in Entity entity, bool makeRequest = false) where T : struct, IComponentBase {

            this.currentState.storage.Validate<T>(ref allocator, in entity, makeRequest);

        }

        public void AddFilterByStructComponent<T>(ref MemoryAllocator allocator, in Entity entity) where T : struct, IComponentBase {

            this.currentState.storage.Set<T>(ref allocator, in entity);

        }

        public void RemoveFilterByStructComponent<T>(ref MemoryAllocator allocator, in Entity entity) where T : struct, IComponentBase {

            this.currentState.storage.Remove<T>(ref allocator, in entity);

        }

        public void RemoveComponentFromFilter(ref MemoryAllocator allocator, in Entity entity) {

            // Remove all components from entity
            this.RemoveFromAllFilters(ref allocator, in entity);

        }

        public FilterData GetFilter(int id) {

            return this.currentState.storage.GetFilter(in this.currentState.allocator, id);

        }

        internal void SetFilterStaticData(int id, FilterInternalData data, ListCopyable<ConnectInfoLambda> dataLambdas) {

            this.filtersStaticData.Resize(ref this.tempAllocator, id + 1);
            this.filtersStaticData[in this.tempAllocator, id] = new FilterStaticData() {
                isCreated = true,
                data = data,
            };
            
            ArrayUtils.Resize(id, ref this.filtersStaticDataLambdas, true);
            this.filtersStaticDataLambdas.arr[id] = dataLambdas;

        }

        internal ref FilterStaticData GetFilterStaticData(int id) {
            
            this.filtersStaticData.Resize(ref this.tempAllocator, id + 1);
            return ref this.filtersStaticData[in this.tempAllocator, id];

        }

        internal ref MemArrayAllocator<FilterStaticData> GetFilterStaticDataBuffer() {
            
            return ref this.filtersStaticData;

        }

        internal ListCopyable<ConnectInfoLambda> GetFilterStaticDataLambdas(int id) {
            
            ArrayUtils.Resize(id, ref this.filtersStaticDataLambdas, true);
            return this.filtersStaticDataLambdas.arr[id];

        }

    }

}
