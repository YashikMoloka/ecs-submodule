﻿#if ENABLE_IL2CPP
#define INLINE_METHODS
#endif

namespace ME.ECS {

    using Collections;

    [System.Flags]
    public enum EntityFlag : int {

        None = 0x0,
        /// <summary>
        /// Destroy entity at the end of tick
        /// </summary>
        OneShot = 0x1,
        /// <summary>
        /// Destroy entity at the end of tick if it has no components
        /// </summary>
        DestroyWithoutComponents = 0x2,

    }

    #if ECS_COMPILE_IL2CPP_OPTIONS
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.NullChecks, false),
     Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false),
     Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    #endif
    public struct EntityVersions {

        [ME.ECS.Serializer.SerializeField]
        private NativeBufferArray<ushort> values;
        private static ushort defaultValue;

        public EntityVersions(int capacity) {

            this.values = default;
            this.Validate(capacity);
            
        }

        public override int GetHashCode() {

            var hash = 0;
            for (int i = 0; i < this.values.Length; ++i) {
                hash ^= (int)(this.values.arr[i] + 100000);
            }

            return hash;

        }

        public void Recycle() {

            PoolArrayNative<ushort>.Recycle(ref this.values);

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Validate(int capacity) {

            NativeArrayUtils.Resize(capacity, ref this.values);

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Validate(in Entity entity) {

            var id = entity.id;
            NativeArrayUtils.Resize(id, ref this.values, true);

        }

        public void CopyFrom(EntityVersions other) {

            NativeArrayUtils.Copy(in other.values, ref this.values);

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public ref ushort Get(int entityId) {

            return ref this.values[entityId];

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public ref ushort Get(in Entity entity) {

            var id = entity.id;
            if (id >= this.values.Length) return ref EntityVersions.defaultValue;
            return ref this.values[id];

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Increment(in Entity entity) {

            unchecked {
                ++this.values[entity.id];
            }

            #if ENTITY_VERSION_INCREMENT_ACTIONS
            World world = Worlds.currentWorld;
            world.RaiseEntityVersionIncrementAction(entity, this.values.arr[entity.id]);
            #endif

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Increment(int entityId) {

            unchecked {
                ++this.values[entityId];
            }

            #if ENTITY_VERSION_INCREMENT_ACTIONS
            World world = Worlds.currentWorld;
            world.RaiseEntityVersionIncrementAction(world.GetEntityById(entityId), this.values.arr[entityId]);
            #endif

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Reset(in Entity entity) {

            this.values[entity.id] = default;

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Reset(int entityId) {

            this.Validate(entityId);
            this.values[entityId] = default;

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Reset(int fromId, int toId) {

            NativeArrayUtils.Clear(this.values, fromId, toId - fromId);

        }

    }
    
    #if ECS_COMPILE_IL2CPP_OPTIONS
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.NullChecks, false),
     Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false),
     Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    #endif
    public struct EntityFlags {

        [ME.ECS.Serializer.SerializeField]
        private BufferArray<byte> values;
        private static byte defaultValue;

        public EntityFlags(int capacity) {

            this.values = default;
            this.Validate(capacity);
            
        }

        public override int GetHashCode() {

            var hash = 0;
            for (int i = 0; i < this.values.Length; ++i) {
                hash ^= (this.values.arr[i] + 100000);
            }

            return hash;

        }

        public void Recycle() {

            PoolArray<byte>.Recycle(ref this.values);

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Validate(int capacity) {

            ArrayUtils.Resize(capacity, ref this.values);

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Validate(in Entity entity) {

            var id = entity.id;
            ArrayUtils.Resize(id, ref this.values, true);

        }

        public void CopyFrom(EntityFlags other) {

            ArrayUtils.Copy(in other.values, ref this.values);

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public ref byte Get(int entityId) {

            return ref this.values.arr[entityId];

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public ref byte Get(in Entity entity) {

            var id = entity.id;
            if (id >= this.values.Length) return ref EntityFlags.defaultValue;
            return ref this.values.arr[id];

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Set(int entityId, EntityFlag flags) {

            this.values.arr[entityId] = (byte)flags;

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Reset(in Entity entity) {

            this.values.arr[entity.id] = default;

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Reset(int entityId) {

            this.Validate(entityId);
            this.values.arr[entityId] = default;

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Reset(int fromId, int toId) {

            System.Array.Clear(this.values.arr, fromId, toId - fromId);

        }

    }
    
}