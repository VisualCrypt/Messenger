using System;
using System.Collections.Generic;

using Android.App;

namespace ObsidianMobile.Droid
{
    public class ActivitiesStack
    {
        readonly Stack<WeakReference<Activity>> Activities;

        public int Count => Activities.Count;

        public ActivitiesStack()
        {
            Activities = new Stack<WeakReference<Activity>>();
        }

        public Activity Peek()
        {
            if (Count == 0)
            {
                return null;
            }

            Activity activity = null;
            Activities.Peek().TryGetTarget(out activity);
            return activity;
        }

        public void Pop()
        {
            if (Count == 0)
            {
                return;
            }

            Activities.Pop();
        }

        public void Push(Activity activity)
        {
            Activities.Push(new WeakReference<Activity>(activity));
        }
    }
}
