#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ScriptableObjectTemplateCreator : Editor
{
    [MenuItem("Assets/Create/ScriptableObject Script", false, 80)]
    private static void CreateScriptableObjectScript()
    {
        // Obtén la ruta del directorio seleccionado
        string folderPath = GetSelectedFolderPath();

        // Si no se seleccionó una carpeta, usa "Assets" como predeterminada
        if (string.IsNullOrEmpty(folderPath))
        {
            folderPath = "Assets";
        }

        // Abre el cuadro de diálogo para darle nombre al archivo
        string path = EditorUtility.SaveFilePanelInProject(
            "Create ScriptableObject Script",
            "NewScriptableObject",
            "cs",
            "Choose a name for the new ScriptableObject script.",
            folderPath
        );

        if (string.IsNullOrEmpty(path))
            return;

        // Obtén el nombre del archivo sin la extensión
        string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

        // Contenido base del script
        string scriptContent = $@"
using UnityEngine;

[CreateAssetMenu(fileName = ""{fileName}"", menuName = ""Custom/ScriptableObject"")]
public class {fileName} : ScriptableObject
{{
    //Add your fields here
}}";

        // Escribe el contenido al archivo
        System.IO.File.WriteAllText(path, scriptContent);

        // Importa el archivo al proyecto
        AssetDatabase.Refresh();

        // Selecciona el archivo recién creado
        var newScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        Selection.activeObject = newScript;
    }

    // Método para obtener la ruta de la carpeta seleccionada
    private static string GetSelectedFolderPath()
    {
        // Revisa si la selección en Unity es una carpeta
        if (Selection.activeObject != null)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(path))
            {
                return path;
            }
        }

        // Si no es una carpeta, retorna null
        return null;
    }
}
#endif
