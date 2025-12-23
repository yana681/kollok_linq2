using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kollok_linq2
{
    public enum WeekDay
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6
    }
    internal class Lesson
    {
        public string Label { get; set; }        // Название предмета
        public int Number { get; set; }          // Номер пары
        public int WeekNumber { get; set; }      // 0 - каждую неделю, 1 - первая, 2 - вторая
        public WeekDay WeekDay { get; set; }     // День недели
        public string GroupNumber { get; set; }  // Номер группы

        // Конструктор с параметрами
        public Lesson(string label, int number, int weekNumber, WeekDay weekDay, string groupNumber)
        {
            Label = label;
            Number = number;
            WeekNumber = weekNumber;
            WeekDay = weekDay;
            GroupNumber = groupNumber;
        }

        // Метод для получения информации о неделе
        public string GetWeekInfo()
        {
            switch (WeekNumber)
            {
                case 0:
                    return "Каждую неделю";
                case 1:
                    return "Первая неделя";
                case 2:
                    return "Вторая неделя";
                default:
                    return "Неизвестно";
            }
        }

        // Переопределение ToString для удобного отображения
        public override string ToString()
        {
            return $"{Label} - {Number} пара, {WeekDay}, группа {GroupNumber} ({GetWeekInfo()})";
        }
    }
}
