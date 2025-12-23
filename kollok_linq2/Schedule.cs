using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace kollok_linq2
{
    internal class Schedule
    {
        public string Name { get; set; }
        public List<Lesson> Lessons { get; set; }
        public Schedule(string name)
        {
            Name = name;
            Lessons = new List<Lesson>();
        }
        public void Add(Lesson lesson)
        {
            Lessons.Add(lesson);
        }
        public void Add(string label, int number, int weekNumber, WeekDay weekDay, string groupNumber)
        {
            Lessons.Add(new Lesson(label, number, weekNumber, weekDay, groupNumber));
        }

        // 6.1) Учебные пары в указанный день недели
        public List<Lesson> GetLessonsByDay(WeekDay day)
        {
            return Lessons
                .Where(lesson => lesson.WeekDay == day)
                .OrderBy(lesson => lesson.Number)
                .ToList();
        }

        // 6.2) Расписание по группам, отсортированное по дням неделям
        public Dictionary<string, List<Lesson>> GetScheduleByGroups()
        {
            return Lessons
                .GroupBy(lesson => lesson.GroupNumber)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .OrderBy(lesson => lesson.WeekDay)
                        .ThenBy(lesson => lesson.Number)
                        .ToList()
                );
        }

        // 6.3) Список всех пар сгруппированный по названию предмета
        public Dictionary<string, List<Lesson>> GetLessonsBySubject()
        {
            return Lessons
                .GroupBy(lesson => lesson.Label)
                .ToDictionary(
                    group => group.Key,  
                    group => group
                        .OrderBy(lesson => lesson.WeekDay) 
                        .ThenBy(lesson => lesson.Number)
                        .ToList()
                );
        }
        public void SaveToFile(string fileName = "schedule.txt")
        {
            List<string> lines = new List<string>();
            lines.Add(Name);
            foreach (Lesson lesson in Lessons)
            {
                string line = $"{lesson.Label};{lesson.Number};{lesson.WeekNumber};{lesson.WeekDay};{lesson.GroupNumber}";
                lines.Add(line);
            }
            File.WriteAllLines(fileName, lines);
        }

        public static Schedule LoadFromFile(string fileName = "schedule.txt")
        {
            if (!File.Exists(fileName))
            {
                return new Schedule("Факультет");
            }
            string[] lines = File.ReadAllLines(fileName);
            string facultyName = lines[0];
            Schedule schedule = new Schedule(facultyName);
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] parts = line.Split(';');
                Lesson lesson = new Lesson(
                    parts[0],
                    int.Parse(parts[1]), 
                    int.Parse(parts[2]),
                    (WeekDay)Enum.Parse(typeof(WeekDay), parts[3]),
                    parts[4]
                );

                schedule.Add(lesson);
            }

            return schedule;
        }
    }
}
