using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Granular.Diagnostics
{
    // Usage:
    //
    // void Work()
    // {
    //     using (Profiler.IncludeScope("Work"))
    //     {
    //         // Included work here
    //         UnrelatedWork();
    //         // More included work here
    //     }
    // }
    // 
    // void UnrelatedWork()
    // {
    //     using (Profiler.ExcludeScope("Work"))
    //     {
    //         // Excluded work here
    //     }
    // }

    public static class Profiler
    {
        private class ProfilingScope
        {
            private TimeSpan totalInclusiveTime;
            private TimeSpan totalExclusiveTime;
            private TimeSpan exclusiveTime;
            private int includeLevel;
            private bool isIncluding;
            private string name;

            public ProfilingScope(string name)
            {
                this.name = name;

                totalInclusiveTime = TimeSpan.Zero;
                totalExclusiveTime = TimeSpan.Zero;
            }

            public IDisposable Include()
            {
                if (includeLevel == 0)
                {
                    exclusiveTime = TimeSpan.Zero;
                }

                if (isIncluding)
                {
                    return Disposable.Empty;
                }

                includeLevel++;
                isIncluding = true;

                DateTime includeStartTime = DateTime.Now;

                return new Disposable(() =>
                {
                    includeLevel--;
                    isIncluding = false;

                    TimeSpan includeTime = DateTime.Now.Subtract(includeStartTime);
                    exclusiveTime += includeTime;

                    if (includeLevel == 0)
                    {
                        totalExclusiveTime += exclusiveTime;
                        totalInclusiveTime += includeTime;

                        Console.WriteLine(String.Format("{0} - exclusive {1}ms (total {2}ms), inclusive {3}ms (total {4}ms)",
                            name,
                            (long)exclusiveTime.TotalMilliseconds,
                            (long)totalExclusiveTime.TotalMilliseconds,
                            (long)includeTime.TotalMilliseconds,
                            (long)totalInclusiveTime.TotalMilliseconds));
                    }
                });
            }

            public IDisposable Exclude()
            {
                if (!isIncluding)
                {
                    return Disposable.Empty;
                }

                isIncluding = false;

                DateTime excludeStartTime = DateTime.Now;

                return new Disposable(() =>
                {
                    isIncluding = true;

                    TimeSpan excludeTime = DateTime.Now.Subtract(excludeStartTime);

                    exclusiveTime -= excludeTime;
                });
            }
        }

        private static Dictionary<string, ProfilingScope> scopes = new Dictionary<string, ProfilingScope>();

        public static IDisposable IncludeScope(string scopeName)
        {
            return GetInitializedScope(scopeName).Include();
        }

        public static IDisposable ExcludeScope(string scopeName)
        {
            return GetInitializedScope(scopeName).Exclude();
        }

        private static ProfilingScope GetInitializedScope(string scopeName)
        {
            ProfilingScope scope;
            if (!scopes.TryGetValue(scopeName, out scope))
            {
                scope = new ProfilingScope(scopeName);
                scopes.Add(scopeName, scope);
            }

            return scope;
        }
    }
}
