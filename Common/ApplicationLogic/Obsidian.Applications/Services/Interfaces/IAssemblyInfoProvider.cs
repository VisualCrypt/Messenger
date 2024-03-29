﻿using System.Reflection;

namespace Obsidian.Applications.Services.Interfaces
{
	public interface IAssemblyInfoProvider
	{
		string AssemblyProduct { get; }
        string AssemblyDescription { get; }
        string AssemblyVersion { get; }
		string AssemblyCompany { get; }
		string AssemblyCopyright { get; }
        Assembly Assembly { set; }
    }
}
