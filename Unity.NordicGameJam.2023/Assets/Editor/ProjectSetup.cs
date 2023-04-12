using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using static System.IO.Directory;
using static System.IO.Path;

namespace amheklerior.editor
{
    public static class ProjectSetup
    {
        public static readonly string RUNTIME = "Runtime";
        public static readonly string THIRD_PARTY = "ThirdParty";
        public static readonly string SANDBOX = "Sandbox";
        public static readonly string EDITOR = "Editor";
        public static readonly string ROOT = Application.dataPath;

        [MenuItem("Tools/Setup/Create Folder Structure")]
        public static void SetupFolderStructure()
        {
            Debug.Log("[Info] Project scaffolding...");
            CreateRuntimeFolder();
            CreateEditorFolder();
            CreateThirdPartyFolder();
            CreateSandboxFolder();
            CreateAssemblies();
            AssetDatabase.Refresh();
            Debug.Log("[Info] Project scaffolding completed!");
        }


        private static void CreateRuntimeFolder()
        {
            var fullpath = Combine(ROOT, RUNTIME);
            CreateFolder(ROOT, RUNTIME, "Art", "Audio", "Code", "Prefabs", "Scenes", "Fonts");
            CreateReadmeFile(Combine(fullpath, "Art"), "Put in here all your art assets, such as icons, textures, sprites, materials, models, etc.");
            CreateReadmeFile(Combine(fullpath, "Audio"), "Put in here your music and sounds.");
            CreateReadmeFile(Combine(fullpath, "Code"), "Put in here all your scripts.");
            CreateReadmeFile(Combine(fullpath, "Prefabs"), "Put in here all your game and UI prefabs.");
            CreateReadmeFile(Combine(fullpath, "Scenes"), "Put in here all your game and UI scenes.");
            CreateReadmeFile(Combine(fullpath, "Fonts"), "Put in here all Fonts");
        }

        private static void CreateEditorFolder()
        {
            CreateFolder(ROOT, EDITOR);
            CreateReadmeFile(Combine(ROOT, EDITOR), "Put your editor tools in here.");
        }

        private static void CreateThirdPartyFolder()
        {
            CreateFolder(ROOT, THIRD_PARTY);
            CreateReadmeFile(Combine(ROOT, THIRD_PARTY), "Put all third party libraries and assets in here.");
        }

        private static void CreateSandboxFolder()
        {
            CreateFolder(ROOT, SANDBOX);
            CreateReadmeFile(
                Combine(ROOT, SANDBOX),
                "Use this folder as your playground field, in which you can run experiments. " +
                "In a team scenario, evaluate creating a folder with your name inside it."
            );
        }

        private static void CreateAssemblies()
        {
            CreateAssemblyDefinition(Combine(ROOT, THIRD_PARTY), THIRD_PARTY);
            CreateAssemblyDefinition(Combine(ROOT, SANDBOX), SANDBOX);
            CreateAssemblyDefinition(Combine(ROOT, RUNTIME), RUNTIME, new string[] { $"GUID:{GetThirdPartyAsmdefGUID()}" });
        }

        #region Internals 

        private static void CreateFolder(string anchor, string folder, params string[] subFolders)
        {
            var path = Combine(anchor, folder);
            if (Exists(path)) return;
            CreateDirectory(path);
            foreach (var subFolder in subFolders) CreateFolder(path, subFolder);
        }

        private static void CreateAssemblyDefinition(string path, string name, string[] deps = null)
        {
            var asmdef = new AssemblyDefinition { name = name, references = deps ?? Array.Empty<string>() };
            var asmdefJson = JsonUtility.ToJson(asmdef, true);
            var asmdefPath = Combine(path, $"{name}.asmdef");
            var stream = File.CreateText(asmdefPath);
            stream.Write(asmdefJson);
            stream.Close();
            AssetDatabase.Refresh();
        }

        private static void CreateReadmeFile(string path, string content)
        {
            var file = Combine(path, "readme.txt");
            if (File.Exists(file)) return;
            File.WriteAllText(file, content);
        }

        private static string GetThirdPartyAsmdefGUID() => AssetDatabase.AssetPathToGUID(Combine(
            Combine("Assets", THIRD_PARTY),
            $"{THIRD_PARTY}.asmdef"));

        protected struct AssemblyDefinition
        {
            public string name;
            public string[] references;
        }

        #endregion

    }
}