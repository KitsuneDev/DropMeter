import React from 'react';
import logo from './logo.svg';
import './App.css';
import { usePluginCombinedState } from './lib/usePluginState';
import { WebNowPlayingMessages, WebNowPlayingState } from './lib/WebNowPlaying';

function App() {
  let webNowPlaying = usePluginCombinedState("webnowplaying") as WebNowPlayingState;
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Playing {webNowPlaying.TITLE ?? "N/A"} by {webNowPlaying.ARTIST ?? "N/A"} - {webNowPlaying.POSITION}/{webNowPlaying.DURATION}
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}

export default App;
