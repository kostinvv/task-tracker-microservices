import Header from './components/Header';
import LogIn from './components/LogIn';
import Footer from './components/Footer';
import './App.css';
import { useContext, useEffect } from 'react';
import { Context } from './main.jsx';
import { observer } from 'mobx-react-lite';

function App() {
  const { store } = useContext(Context);

  useEffect(() => {
    if (localStorage.getItem('accessToken')) {
      store.checkAuth();
    }
  }, []);

  return (
    <>
      <h1>{ store.isAuth ? `User ${store.user.email}` : '' }</h1>
      <LogIn />
    </>
  ) 
}

export default observer(App);
