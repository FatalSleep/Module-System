using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace ModuleSystem {
    public class ModuleLoader {
        #region Properties
        public Dictionary<string,ModuleCore> ModuleCores { get; internal set; }

        public HashSet<string> BlackListedModules { get; internal set; }

        public string ImportDirectory { get; internal set; }

        public bool RecursiveImport { get; internal set; }
        #endregion

        #region Constructor
        public ModuleLoader(string directory, bool recursiveImport) {
            ImportDirectory = directory;
            RecursiveImport = recursiveImport;

            ModuleCores = new Dictionary<string, ModuleCore>();
            BlackListedModules = new HashSet<string>();
        }
        #endregion

        #region Module Loader
        private bool ImportDirectoryIsValid() {
            try {
                bool DirectoryExists = Directory.Exists(ImportDirectory);
                System.IO.Path.GetFullPath(ImportDirectory);
                return DirectoryExists;
            } catch(Exception) {
                return false;
            }
        }

        public void ImportModules( bool applyBlackList ) {
            if(!ImportDirectoryIsValid())
                return;

            foreach(string file in Directory.GetFiles(ImportDirectory, "*.dll", (RecursiveImport) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)) {
                string assemblyName = AssemblyName.GetAssemblyName(file).FullName;

                if(assemblyName == Assembly.GetExecutingAssembly().FullName)
                    continue;
                if(applyBlackList && BlackListedModules.Contains(assemblyName))
                    continue;

                if(!ModuleCores.ContainsKey(assemblyName)) {
                    Assembly importedAssembly = Assembly.LoadFrom(file);
                    List<Type> verifiedModules = new List<Type>();

                    foreach(Type module in importedAssembly.GetTypes())
                        if(VerifyModule(module))
                            verifiedModules.Add(module);

                    ModuleCore core = new ModuleCore(assemblyName, Path.GetFullPath(file), importedAssembly, verifiedModules.AsReadOnly());
                    ModuleCores.Add(assemblyName, core);
                }
            }
        }

        protected bool VerifyModule(Type module) =>
            module.IsClass && module.BaseType == typeof(Module);
        #endregion

        #region Internal Exceptions
        internal class InvalidDirectoryException : Exception {
            public InvalidDirectoryException() { }
            public InvalidDirectoryException(string Directory) : base("Directory: " + Directory) { }
        }
        #endregion
    }
}
