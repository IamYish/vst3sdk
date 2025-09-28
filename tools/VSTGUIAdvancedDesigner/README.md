# VSTGUI Advanced Designer

This directory contains a Windows-only WPF application that targets the creation of
advanced graphical user interfaces for VST3 (SDK 3.7.14) plug-ins using VSTGUI 4.14.3
`.uidesc` files.  The designer focuses on modern production-ready workflows and
supports layered controls, sprite-sheet animation authoring, snapping, grids, and
project-based editing.

## Features

* Import and export of `.uidesc` files with full preservation of VSTGUI 4.14.3
  attributes.
* Dedicated project format (`.vstguidesign`) for saving incremental work.
* Layer panel on the left and property inspector on the right.
* Rich control palette covering all controls found in VSTGUI 4.14.3.
* Layered bitmaps for knobs, sliders, and buttons, including PNG asset management.
* Grid and snapping helpers for precise layout.
* Animation timeline authoring with sprite-sheet export.
* Export of the composed canvas to a PNG for documentation or collaboration.

## Building

This project targets .NET 6.0 (Windows) and WPF.  Use the following commands on a
Windows machine with the .NET SDK installed:

```powershell
cd tools/VSTGUIAdvancedDesigner/VSTGUIAdvancedDesigner
dotnet build
```

To run the designer:

```powershell
dotnet run
```

## Project structure

* `App.xaml` / `App.xaml.cs` – WPF application entry point.
* `MainWindow.xaml` – Shell layout including menu, layer panel, property grid,
  and design surface.
* `Models/` – Serializable data structures describing VSTGUI controls, layers,
  animations, and project metadata.
* `Services/` – Infrastructure for importing/exporting `.uidesc`, managing
  sprite sheets, PNG export, and persistence.
* `ViewModels/` – Presentation logic for the MVVM pattern used by the UI.
* `Views/Controls/` – Custom WPF controls such as the design surface and control
  palette.

The implementation intentionally keeps a strict separation between the editable
project model and the persisted `.uidesc` XML to make it easy to support multiple
VSTGUI releases.
