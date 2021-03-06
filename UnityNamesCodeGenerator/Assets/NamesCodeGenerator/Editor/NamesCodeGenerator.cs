﻿using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace NamesCodeGenerator
{
    public static class NamesCodeGenerator
    {
        const string headerComment = "// Generated code by NamesCodeGenerator";

        public static void GenerateConstStaticClasses(string outputPath, string namespaceName)
        {
            CodeSerializer.ResetDirectory(outputPath);

            GenerateConstStaticClass(outputPath, namespaceName, "TagName", NameGetter.GetTags());
            GenerateConstStaticClass(outputPath, namespaceName, "LayerName", NameGetter.GetLayers().Select(l => l.Name).ToArray());
            GenerateConstStaticClass(outputPath, namespaceName, "SceneName", NameGetter.GetScenes().Select(s => s.Name).ToArray());
            GenerateConstStaticClass(outputPath, namespaceName, "SortingLayerName", NameGetter.GetSortingLayers().Select(s => s.Name).ToArray());

            AssetDatabase.Refresh();
        }

        public static void GenerateNamesCodes(string outputPath, string namespaceName, bool isGenerateExtensions)
        {
            CodeSerializer.ResetDirectory(outputPath);

            GenerateStructAndParent(outputPath, namespaceName, "Tags", "TagName", NameGetter.GetTags());
            GenerateStructAndParent(outputPath, namespaceName, "Layers", "LayerName", new[] { Member.StringName, Member.IntIndex }, NameGetter.GetLayers());
            GenerateStructAndParent(outputPath, namespaceName, "Scenes", "SceneName", new[] { Member.StringName, Member.IntIndex }, NameGetter.GetScenes());
            GenerateStructAndParent(outputPath, namespaceName, "SortingLayers", "SortingLayerName", new[] { Member.StringName, Member.IntId }, NameGetter.GetSortingLayers());

            if (isGenerateExtensions)
                ExtensionCodeGenerator.GenerateExtensionCodes(outputPath, namespaceName);

            AssetDatabase.Refresh();
        }

        static void GenerateConstStaticClass(string outputPath, string namespaceName, string className, string[] names)
        {
            var codeBuilder = new CodeBuilder.StaticClassCodeBuilder(headerComment);
            if (namespaceName != null)
                codeBuilder.AddNamespace(namespaceName);
            codeBuilder.AddClass(className);
            codeBuilder.AddConstParameters(names);
            CodeSerializer.WriteCodeFile(outputPath, codeBuilder.Build(), className, namespaceName);
        }

        static string GenerateStructCode(string namespaceName, string structName, IEnumerable<Member> members)
        {
            var codeBulider = new CodeBuilder.StructWithReadonlyPropertiesCodeBuilder(headerComment);
            if (namespaceName != null)
                codeBulider.AddNamespace(namespaceName);
            codeBulider.AddStuct(structName);
            codeBulider.AddMembers(members);
            return codeBulider.Build();
        }

        static void GenerateStructAndParent(string outputPath, string namespaceName, string parentName, string structName, string[] names)
        {
            var structCode = GenerateStructCode(namespaceName, structName, new[] { Member.StringName });
            CodeSerializer.WriteCodeFile(outputPath, structCode, structName, namespaceName);

            var parentCode = GenerateStaticClassCode(namespaceName, parentName, structName, names);
            CodeSerializer.WriteCodeFile(outputPath, parentCode, parentName, namespaceName);
        }

        static string GenerateStaticClassCode(string namespaceName, string className, string structName, string[] names)
        {
            var parentCodeBuilder = new CodeBuilder.StaticClassCodeBuilder(headerComment);
            if (namespaceName != null)
                parentCodeBuilder.AddNamespace(namespaceName);
            parentCodeBuilder.AddClass(className);
            parentCodeBuilder.AddObjectParameters(structName, names);
            return parentCodeBuilder.Build();
        }

        static string GenerateStaticClassCode(string namespaceName, string className, string structName, IEnumerable<NameWithNumber> nameWithNumbers)
        {
            var parentCodeBuilder = new CodeBuilder.StaticClassCodeBuilder(headerComment);
            if (namespaceName != null)
                parentCodeBuilder.AddNamespace(namespaceName);
            parentCodeBuilder.AddClass(className);
            parentCodeBuilder.AddObjectParameters(structName, nameWithNumbers);
            return parentCodeBuilder.Build();
        }

        static void GenerateStructAndParent(string outputPath, string namespaceName, string parentName, string structName, Member[] members, IEnumerable<NameWithNumber> nameWithNumbers)
        {
            var structCode = GenerateStructCode(namespaceName, structName, members);
            CodeSerializer.WriteCodeFile(outputPath, structCode, structName, namespaceName);

            var parentCode = GenerateStaticClassCode(namespaceName, parentName, structName, nameWithNumbers);
            CodeSerializer.WriteCodeFile(outputPath, parentCode, parentName, namespaceName);
        }
    }
}
