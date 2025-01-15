using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderVisualizer : MonoBehaviour
{
    // Color configurable para el Gizmo
    [SerializeField] private Color gizmoColor = Color.green;

    [SerializeField] private BoxCollider boxCollider;

    // Se llama automáticamente para dibujar los Gizmos en la escena
    void OnDrawGizmos()
    {
        // Guardamos el color actual de Gizmos
        Color previousColor = Gizmos.color;

        // Establecemos el color del Gizmo
        Gizmos.color = gizmoColor;

        // Obtenemos la posición, rotación y escala del objeto
        Transform objectTransform = boxCollider.transform;

        // Aplicamos la rotación y la escala al Gizmo
        Gizmos.matrix = Matrix4x4.TRS(objectTransform.position + boxCollider.center, objectTransform.rotation, objectTransform.lossyScale);

        // Dibujamos el cubo con la posición, el tamaño y la transformación correcta
        Gizmos.DrawCube(Vector3.zero, boxCollider.size);

        // Restauramos el color original de Gizmos
        Gizmos.color = previousColor;

        // Restauramos la matriz de Gizmos a la identidad para evitar interferencias con otros Gizmos
        Gizmos.matrix = Matrix4x4.identity;
    }
}