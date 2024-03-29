﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Obsidian.Applications.Services.Interfaces
{
    public interface IMessageBoxService
    {
        Task<RequestResult> Show(string messageBoxText, string title, RequestButton buttons,
            RequestImage image);

        Task ShowError(Exception e, [CallerMemberName] string callerMemberName = "");

        Task ShowError(string error);
        Type GetCallersType(int stackFrame);
    }
}
