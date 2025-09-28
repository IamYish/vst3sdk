import { useState } from 'react';

export default function HelloCard() {
  const [name, setName] = useState('Visitor');

  return (
    <div
      style={{
        padding: 24,
        borderRadius: 16,
        background: 'linear-gradient(145deg, #93c5fd, #1d4ed8)',
        color: 'white',
        minWidth: 320
      }}
    >
      <h2 style={{ marginTop: 0 }}>Welcome, {name}!</h2>
      <p>This sample demonstrates interactivity and styling in JSX Viewer.</p>
      <input
        value={name}
        onChange={(event) => setName(event.target.value)}
        style={{
          padding: '8px 12px',
          borderRadius: 999,
          border: 'none',
          outline: 'none'
        }}
      />
    </div>
  );
}
