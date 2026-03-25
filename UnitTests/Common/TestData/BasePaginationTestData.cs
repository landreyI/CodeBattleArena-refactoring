using System.Collections;

namespace UnitTests.Common.TestData
{
    public class BasePaginationTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // Параметры: { page, size, totalInDb, expectedCount }

            // Сценарий 1: Первая полная страница
            yield return new object[] { 1, 10, 100, 10 };

            // Сценарий 2: Последняя неполная страница (всего 15, просим 2-ю по 10)
            yield return new object[] { 2, 10, 15, 5 };

            // Сценарий 3: Пустая база данных
            yield return new object[] { 1, 10, 0, 0 };

            // Сценарий 4: Запрос страницы, которой не существует (ушли за пределы)
            yield return new object[] { 5, 10, 25, 0 };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
