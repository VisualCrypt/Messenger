﻿namespace Obsidian.Applications.Services.Interfaces
{
    public interface IFontManager
    {
        void ApplyFontsFromSettingsToEditor();
        void ShowFontChooserAndApplyChoiceToEditor();
        void ExecuteZoom100();
        bool CanExecuteZoom100();
        void ExecuteZoomOut();
        bool CanExecuteZoomOut();
        void ExecuteZoomIn();
        bool CanExecuteZoomIn();
        void ExecuteChooseFont();
        bool CanExecuteChooseFont();
    }
}
