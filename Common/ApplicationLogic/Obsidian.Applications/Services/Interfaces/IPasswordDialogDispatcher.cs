using System;
using System.Threading.Tasks;
using Obsidian.Applications.Models;

namespace Obsidian.Applications.Services.Interfaces
{
    public interface IPasswordDialogDispatcher
    {
        /// <returns>true if the dialog was closed with 'Set'</returns>
        Task<bool> LaunchAsync(SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSetViewModelCallback, bool isPasswordSetWhenDialogOpened);
    }
}
