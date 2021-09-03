using System.Collections.Generic;

namespace BaselessJumping.GameContent
{
    public class Quest
    {
        public bool Completed { get; private set; }
        public List<bool> Objectives { get; set; }

        public int TotalTasks { get; }
        /// <summary>Create a new list of tasks for a player to complete for a Quest. Preferably do not pass in a constant value.</summary>
        /// <param name="tasks">The amount of tasks to be completed before this entire Quest is complete.</param>
        public Quest(params bool[] tasks)
        {
            foreach (var task in tasks)
                Objectives.Add(task);
            TotalTasks = Objectives.Count;
        }

        public void CompleteTask(int id)
            => Objectives[id] = true;
        public void Complete() => Completed = true;

        // solve for completion and other stuff
    }
}