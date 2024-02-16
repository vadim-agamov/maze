using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public static class WebGLPostBuildProcessor
    {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target == BuildTarget.WebGL)
            {
                //get folder name
                var folderName = Path.GetFileName(pathToBuiltProject);
                var frameworkJsPath = Path.Combine(pathToBuiltProject, $"Build/{folderName}.framework.js");
                if (File.Exists(frameworkJsPath))
                {
                    var content = File.ReadAllText(frameworkJsPath);
 
                    if (content.Contains("if(!screen.orientation){return}"))
                    {
                        Debug.Log("Replacing screen.orientation");
                    }
                    else
                    {
                        Debug.LogWarning("We can remove this post-build script if the issue is fixed in Unity 2022.3.17f1 or higher");
                    }
 
                    content = content.Replace("if(!screen.orientation){return}", "if(true){return}");
                    File.WriteAllText(frameworkJsPath, content);
                }
            }
        }
    }
}