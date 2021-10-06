using System.Collections.Generic;

namespace BaselessJumping.Internals.Common.Utilities
{
    public class StateBoolean
    {
        private static List<StateBoolean> sBools = new();
        public bool StateChanged { get; private set; }
        public bool NewState { get; private set; }
        public bool OldState { get; private set; }

        private bool _tracked;

        public StateBoolean(bool trackedBool)
        {
            _tracked = trackedBool;
            sBools.Add(this);
        }

        public static void Update()
        {
            foreach (var sBool in sBools)
            {
                sBool.NewState = sBool._tracked;
                sBool.OldState = sBool.NewState;
            }    
        }
    }
}