#if ENABLE_IL2CPP
#define INLINE_METHODS
#endif

namespace ME.ECS.Collections {

    #if ECS_COMPILE_IL2CPP_OPTIONS
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    #endif
    public struct DataObjectDefaultProvider<T> : IDataObjectProvider<T> where T : struct {

        public void Clone(T from, ref T to) {

            to = from;

        }

        public void Recycle(ref T value) {

            value = default;

        }

    }

    public interface IDataObjectProvider<T> {

        void Clone(T from, ref T to);

        void Recycle(ref T value);

    }

    public interface IDataObject<T> {

        ref readonly T Read();
        ref T Get();
        void Set(T value);
        void Dispose();

    }

    #if ECS_COMPILE_IL2CPP_OPTIONS
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    #endif
    [GeneratorIgnoreManagedType]
    public struct DataObject<T, TProvider> : IDataObject<T> where TProvider : struct, IDataObjectProvider<T> {

        private class DisposeSentinel : System.IDisposable {

            public T data;
            public Tick tick;

            ~DisposeSentinel() {

                this.Dispose();

            }

            #if INLINE_METHODS
            [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            #endif
            public void Dispose() {

                this.tick = Tick.Invalid;
                PoolClass<DisposeSentinel>.Recycle(this);
                default(TProvider).Recycle(ref this.data);

            }

        }

        private DisposeSentinel disposeSentinel;
        private bool isCreated;

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public DataObject(T data) {

            this.disposeSentinel = PoolClass<DisposeSentinel>.Spawn();
            this.disposeSentinel.data = data;
            this.disposeSentinel.tick = Worlds.currentWorld.GetLastSavedTick();
            this.isCreated = true;

        }

        public override int GetHashCode() {

            return this.disposeSentinel.data.GetHashCode();

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public ref readonly T Read() {

            if (this.isCreated == false) {
                throw new System.Exception($"Try to read collection that has been already disposed. Tick: {Worlds.currentWorld.GetCurrentTick()}");
            }

            return ref this.disposeSentinel.data;

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public ref T Get() {

            if (this.isCreated == false) {
                throw new System.Exception($"Try to read collection that has been already disposed. Tick: {Worlds.currentWorld.GetCurrentTick()}");
            }

            if (this.disposeSentinel.tick != Worlds.currentWorld.GetLastSavedTick()) {
                this.CloneInternalArray();
            }

            return ref this.disposeSentinel.data;

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Set(T value) {

            if (this.isCreated == false) {
                throw new System.Exception($"Try to read collection that has been already disposed. Tick: {Worlds.currentWorld.GetCurrentTick()}");
            }

            if (this.disposeSentinel.tick != Worlds.currentWorld.GetLastSavedTick()) {
                this.CloneInternalArray();
            } else {
                default(TProvider).Recycle(ref this.disposeSentinel.data);
            }
            this.disposeSentinel.data = value;

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Dispose() {

            this.disposeSentinel.Dispose();
            this.isCreated = false;

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        private void CloneInternalArray() {

            this.disposeSentinel.tick = Worlds.currentWorld.GetLastSavedTick();
            var previousData = this.disposeSentinel.data;
            this.disposeSentinel = PoolClass<DisposeSentinel>.Spawn();
            default(TProvider).Clone(previousData, ref this.disposeSentinel.data);

        }

    }

    #if ECS_COMPILE_IL2CPP_OPTIONS
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    #endif
    [GeneratorIgnoreManagedType]
    public struct DataObject<T> : IDataObject<T> where T : struct {

        private DataObject<T, DataObjectDefaultProvider<T>> dataObject;
        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public DataObject(T data) {

            this.dataObject = new DataObject<T, DataObjectDefaultProvider<T>>(data);

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public ref readonly T Read() {

            return ref this.dataObject.Read();

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public ref T Get() {

            return ref this.dataObject.Get();

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Set(T value) {

            this.dataObject.Set(value);

        }

        #if INLINE_METHODS
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        #endif
        public void Dispose() {

            this.dataObject.Dispose();

        }

    }

}