namespace hw1.Cache
{
    public static class Sorting
    {
        // --- Алгоритм сортування (Insertion Sort) ---
        // (не використовуючи Linq.OrderBy або List.Sort)
        // Приймає 'comparison' делегат, щоб зробити сортування узагальненим
        public static void InsertionSort<T>(List<T> list, Comparison<T> comparison)
        {
            // Проходимо по масиву, починаючи з другого елемента (індекс 1)
            for (int i = 1; i < list.Count; i++)
            {
                T currentItem = list[i];
                int j = i - 1;

                // Зміщуємо елементи, які більші за currentItem, вправо
                // comparison(list[j], currentItem) > 0 означає, що list[j] > currentItem
                while (j >= 0 && comparison(list[j], currentItem) > 0)
                {
                    list[j + 1] = list[j];
                    j = j - 1;
                }
                
                // Вставляємо currentItem на його правильну позицію
                list[j + 1] = currentItem;
            }
        }
    }
}