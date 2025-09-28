# JSX Viewer

A standalone Electron desktop application for Windows 11 that previews and runs JSX (and TSX) files generated from ChatGPT prompts. The viewer evaluates JSX safely inside an isolated renderer process, letting you iterate on UI snippets without wiring up a full React project.

## Features

- 📂 Load local `.jsx`, `.tsx`, `.js`, or `.ts` files via the file picker.
- ⚡ On-the-fly transformation using Babel with React and TypeScript presets.
- 🔁 Live preview that re-renders as you edit or swap files.
- 🧩 Supports default or named React component exports (function or class).
- 🎨 Windows 11 inspired interface with code and preview panes.
- 🧪 Bundled playground sample for quick experimentation.

## Getting started (Windows 11)

1. **Install prerequisites**
   - [Node.js 18+](https://nodejs.org/) (includes npm)
   - Git (already required to obtain this repository)

2. **Install dependencies**

   ```powershell
   cd jsx-viewer
   npm install
   ```

3. **Run in development mode**

   ```powershell
   npm start
   ```

   This launches the Electron app with live reload tools enabled.

4. **Package a standalone build**

   ```powershell
   npm run package
   ```

   The packaged Windows 11 application (x64) is emitted under `dist/JSXViewer-win32-x64`. Distribute the folder or wrap it with your preferred installer solution.

## Using the viewer

- Click **Open JSX file** to select a file. The left panel shows the source code and the right panel renders the exported component.
- Use **Load sample** to restore the bundled playground snippet.
- Exports must be React components. Default exports are preferred, but the viewer will fall back to the first named export it finds.
- Only runtime dependencies bundled with the viewer (`react`, `react-dom`, and `@babel/standalone`) are available. Third-party imports will throw a helpful error.

## Sample snippets

The `samples` folder contains example JSX files. Drag them into the viewer or open them with the file picker to explore the capabilities.

## Extending

- To add additional libraries, install them with npm and extend the sandbox `require` switch inside `src/renderer.js`.
- Adjust the Babel configuration (presets or plugins) in `evaluateComponent` if your JSX uses different language features.
- Use packaging tools such as [`electron-builder`](https://www.electron.build/) if you need MSI/Setup packages or code signing.

## License

This tool is provided under the MIT license. See the root of the repository for details.
