using System;
using System.Threading.Tasks;

namespace Obsidian.Applications.Infrastructure
{
    public delegate Task AsyncEventHandler(object sender, EventArgs args);
    public delegate Task AsyncEventHandler<in T>(object sender, T args);
}
