public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
    public static void SwapElements(int[] arr, int i, int j)
    {
        int temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }

    public static int DivideArray(int[] arr, int low, int high)
    {
        // Seleccionar el punto de pivote
        int pivot = arr[high];
        // Índice del elemento más pequeño e indica la posición
        // correcta del pivote encontrado hasta el momento
        int i = (low - 1);
        for (int j = low; j <= high - 1; ++j)
        {
            // Si el elemento actual es más pequeño que el pivote
            if (arr[j] < pivot)
            {
                // Incrementa el índice del elemento más pequeño
                i++;
                SwapElements(arr, i, j);
            }
        }
        SwapElements(arr, i + 1, high);
        return (i + 1);
    }

    public static void QuickSort(int[] array, int low, int high)
    {
        if (low < high)
        {
            // pi es el índice de división,
            // arr[p] ahora está en el lugar correcto
            int pi = DivideArray(array, low, high);
            // Ordenar elementos por separado antes de
            // la división y después de la partición
            QuickSort(array, low, pi - 1);
            QuickSort(array, pi + 1, high);
        }
    }

}