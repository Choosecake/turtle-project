using Random = System.Random;

public static class ArrayExtensions
{
    public static void Shuffle<T>(this T[] array)
    {
        var random = new Random();

        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);

            (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
            // T temp = array[i];
            // array[i] = array[randomIndex];
            // array[randomIndex] = temp;
        }
    }
}