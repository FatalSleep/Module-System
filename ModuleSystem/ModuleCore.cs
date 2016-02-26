using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ModuleSystem {
    public class ModuleCore {
        #region Properties
        public string AssemblyDirectory { get; internal set; }

        public string AssemblyName { get; internal set; }

        public Assembly LoadedAssembly { get; internal set; }

        public ReadOnlyCollection<Type> VerifiedModules { get; internal set; }

        public List<Module> LoadedModules { get; internal set; }
        #endregion

        #region Constructor
        public ModuleCore(string assemblyName, string assemblyDirectory, Assembly loadedAssembly, ReadOnlyCollection<Type> verifiedModules) {
            AssemblyName = assemblyName;
            AssemblyDirectory = assemblyDirectory;
            LoadedAssembly = loadedAssembly;
            VerifiedModules = verifiedModules;
        }
        #endregion

        #region Load Modules
        public void LoadModule(Type module) {
            if(!module.IsSubclassOf(typeof(Module)) || !VerifiedModules.Contains(module))
                return;

            Module loadedModule = (Module) Activator.CreateInstance(module);
            LoadedModules.Add(loadedModule);
        }

        public void LoadModules() {
            foreach(Type module in VerifiedModules)
                LoadedModules.Add((Module)Activator.CreateInstance(module));
        }
        #endregion
    }
}
