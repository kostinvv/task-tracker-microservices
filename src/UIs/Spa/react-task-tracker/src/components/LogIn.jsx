import { useContext, useState } from "react"
import { Context } from '../main.jsx';
import { observer } from "mobx-react-lite";

function LogIn() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const { store } = useContext(Context);

  return (
    <>
      <div>
        <div>
          <label htmlFor='email'>E-mail</label>
          <input 
            onChange={event => setEmail(event.target.value)} 
            value={email} 
            id='email' 
            name='email' 
            type='email' 
            placeholder='Enter your e-mail' 
            autoComplete="on" 
          />
        </div>
        <div>
          <label htmlFor='password'>Password</label>
          <input 
            onChange={event => setPassword(event.target.value)} 
            value={password} 
            id='password' 
            name='password' 
            type='password' 
            placeholder='Enter your passowrd' 
            autoComplete="on" 
          />
        </div>
      </div>
      <div>
        <button type="button" onClick={() => store.logIn(email, password)}>Log In</button>
        <button type="button" onClick={() => store.signUp(email, password)}>Sign Up</button>
        <button type="button" onClick={() => store.logOut()}>Log Out</button>
      </div>
    </>
  )
}

export default observer(LogIn);