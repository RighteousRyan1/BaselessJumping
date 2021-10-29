using System;
using System.Collections.Generic;

namespace BaselessJumping.GameContent
{
    public class Quest
    {
        public string Name { get; }
        public bool Completed { get; private set; }
        public List<QuestTask> Objectives { get; set; } = new();

        public int TotalTasks { get; }
        /// <summary> Create a new list of tasks for a player to complete for a Quest. Preferably do not pass in a constant value. </summary>
        /// <param name="tasks">The amount of tasks to be completed before this entire Quest is complete.</param>
        public Quest(params ValueTuple<string, bool>[] tasks)
        {
            foreach (var task in tasks)
                Objectives.Add(new QuestTask(task.Item1, task.Item2));
            TotalTasks = Objectives.Count;
        }
        public void Complete() => Completed = true;

        // solve for completion and other stuff
    }
    public class QuestTask
    {
        public string Name { get; }
        public bool Completed { get; private set; }

        public QuestTask(string name, bool task)
        {
            Name = name;
            Completed = task;
        }

        public void Complete()
            => Completed = true;
    }
}