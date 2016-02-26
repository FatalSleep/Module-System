using System;
using ModuleSystem.Interface;

namespace ModuleSystem {
    public abstract class Module : IAssembleModule, IDisposable {
        #region Properties
        public ModuleCore Info { get; internal set; }
        #endregion

        #region Event Definitions
        public delegate void ModuleLoadedEventHandler();
        public delegate void ModuleUnloadedEventHandler();
        public event ModuleLoadedEventHandler OnModuleLoaded;
        public event ModuleUnloadedEventHandler OnModuleUnloaded;
        #endregion

        #region Constructor / Destructor
        Module(ModuleCore info) {
            if(info == null)
                throw new NullReferenceException("Attempted to create module from <null> assembly.");

            Info = info;
        }

        ~Module() {
            Dispose(false);
        }
        #endregion

        #region Dispose Pattern
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged) { }
        #endregion

        #region Module Assembler Events
        public virtual void OnAssemble() =>
            OnModuleLoaded?.Invoke();

        public virtual void OnDisassemble() =>
            OnModuleUnloaded?.Invoke();
        #endregion
    }
}
