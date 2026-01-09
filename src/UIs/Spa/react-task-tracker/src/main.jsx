import { createContext } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import './index.css';
import App from './App.jsx';
import RootStore from './stores/RootStore.js';

const store = new RootStore();

export const Context = createContext({
  store
});

createRoot(document.getElementById('root')).render(
  <Context.Provider value={{store}}>
      <BrowserRouter>
        <App />
      </BrowserRouter>
  </Context.Provider>
)
