const { useState, useEffect, useRef } = window.React;
const ReactDOMClient = window.ReactDOMClient;
const Babel = window.Babel;

const SAMPLE_CODE = `// Sample JSX playground
// Use export default to choose which component renders.
import { useState } from 'react';

export default function Playground() {
  const [count, setCount] = useState(0);
  return (
    <div style={{ padding: 24 }}>
      <h1>Hello from JSX Viewer \u2728</h1>
      <p>Use this panel to experiment with ChatGPT derived JSX snippets.</p>
      <button
        style={{
          background: '#2563eb',
          color: '#fff',
          border: 'none',
          borderRadius: 999,
          padding: '8px 16px',
          cursor: 'pointer'
        }}
        onClick={() => setCount((value) => value + 1)}
      >
        Clicked {count} times
      </button>
    </div>
  );
}
`;

function preprocessSource(source) {
  if (!source) {
    return '';
  }

  const sanitized = source
    .replace(/import\s+React[^;]*;?/g, '')
    .replace(/import\s+\{?[^;]*from\s+['\"]react['\"];?/g, '')
    .replace(/import\s+\{?[^;]*from\s+['\"]react-dom[^;]*;?/g, '');

  return sanitized;
}

function evaluateComponent(source) {
  const React = window.React;

  const transformed = Babel.transform(preprocessSource(source), {
    presets: [
      ['env', { modules: 'commonjs' }],
      ['react', { runtime: 'classic' }],
      'typescript'
    ],
    sourceType: 'module'
  }).code;

  const exports = {};
  const module = { exports };
  const sandboxRequire = (specifier) => {
    switch (specifier) {
      case 'react':
        return React;
      case 'react-dom':
      case 'react-dom/client':
        return ReactDOMClient;
      case '@emotion/react':
        throw new Error('Emotion is not bundled. Remove the import or provide inline styles.');
      default:
        throw new Error(`Unsupported import: ${specifier}`);
    }
  };

  const fn = new Function('exports', 'module', 'require', 'React', 'ReactDOMClient', transformed);
  fn(exports, module, sandboxRequire, React, ReactDOMClient);

  const candidates = [];

  if (module.exports && typeof module.exports === 'function') {
    candidates.push(module.exports);
  }

  if (module.exports && typeof module.exports === 'object') {
    if (typeof module.exports.default === 'function') {
      candidates.push(module.exports.default);
    }
    Object.keys(module.exports).forEach((key) => {
      const value = module.exports[key];
      if (typeof value === 'function') {
        candidates.push(value);
      }
    });
  }

  const component = candidates.find((candidate) => typeof candidate === 'function');

  if (!component) {
    throw new Error(
      "No React component export detected. Please export a component using `export default` or `module.exports =`."
    );
  }

  return component;
}

function StatusBar({ status, error, renderTime }) {
  return window.React.createElement(
    'div',
    { className: 'status-bar' },
    window.React.createElement(
      'span',
      { className: 'status-badge' },
      status
    ),
    renderTime != null && window.React.createElement('span', null, `Render time: ${renderTime.toFixed(2)}ms`),
    error && window.React.createElement('span', { className: 'error' }, error)
  );
}

function EmptyState() {
  return window.React.createElement(
    'div',
    { className: 'empty-state' },
    window.React.createElement('strong', null, 'Open a JSX file to begin previewing.'),
    window.React.createElement('span', null, 'Only exports using React components are supported.')
  );
}

function App() {
  const [code, setCode] = useState(SAMPLE_CODE);
  const [filePath, setFilePath] = useState('Unsaved playground.jsx');
  const [status, setStatus] = useState('Ready');
  const [error, setError] = useState(null);
  const [renderTime, setRenderTime] = useState(null);
  const previewRootRef = useRef(null);
  const containerRef = useRef(null);

  useEffect(() => {
    containerRef.current = document.getElementById('render-target');
    previewRootRef.current = ReactDOMClient.createRoot(containerRef.current);
    previewRootRef.current.render(window.React.createElement(EmptyState));
  }, []);

  useEffect(() => {
    if (!previewRootRef.current) {
      return undefined;
    }

    const timeout = setTimeout(() => {
      try {
        const start = performance.now();
        const Component = evaluateComponent(code);
        const element = window.React.createElement(Component);
        previewRootRef.current.render(element);
        const end = performance.now();
        setRenderTime(end - start);
        setStatus(`Previewing ${filePath}`);
        setError(null);
      } catch (err) {
        previewRootRef.current.render(window.React.createElement(EmptyState));
        setError(err.message);
        setStatus('Compilation error');
        setRenderTime(null);
      }
    }, 200);

    return () => clearTimeout(timeout);
  }, [code, filePath]);

  const openFile = async () => {
    const result = await window.electronAPI.openFile();
    if (!result) {
      return;
    }

    setFilePath(result.filePath);
    setCode(result.content);
    setStatus(`Loaded ${result.filePath}`);
  };

  return window.React.createElement(
    'div',
    { className: 'app-shell' },
    window.React.createElement(
      'div',
      { className: 'toolbar' },
      window.React.createElement(
        'button',
        { type: 'button', className: 'primary', onClick: openFile },
        'Open JSX file'
      ),
      window.React.createElement(
        'button',
        {
          type: 'button',
          className: 'secondary',
          onClick: () => {
            setCode(SAMPLE_CODE);
            setFilePath('Unsaved playground.jsx');
            setStatus('Restored sample playground');
          }
        },
        'Load sample'
      )
    ),
    window.React.createElement(
      'div',
      { className: 'panel' },
      window.React.createElement('div', { className: 'file-info' }, filePath),
      window.React.createElement('textarea', {
        className: 'code-viewer',
        value: code,
        spellCheck: false,
        onChange: (event) => setCode(event.target.value)
      })
    ),
    window.React.createElement(
      'div',
      { className: 'panel render' },
      window.React.createElement('div', { id: 'render-target', className: 'render-surface' })
    ),
    window.React.createElement(StatusBar, { status, error, renderTime })
  );
}

const root = ReactDOMClient.createRoot(document.getElementById('root'));
root.render(window.React.createElement(App));
