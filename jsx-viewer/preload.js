const { contextBridge, ipcRenderer } = require('electron');
const React = require('react');
const ReactDOMClient = require('react-dom/client');
const Babel = require('@babel/standalone');

contextBridge.exposeInMainWorld('electronAPI', {
  openFile: () => ipcRenderer.invoke('dialog:openFile')
});

contextBridge.exposeInMainWorld('React', React);
contextBridge.exposeInMainWorld('ReactDOMClient', ReactDOMClient);
contextBridge.exposeInMainWorld('Babel', Babel);
