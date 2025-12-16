export default function LogIn() {
  return (
    <form>
      <div>
        <div>
          <label htmlFor='email'>E-mail</label>
          <input id='email' name='email' type='email' placeholder='Enter your e-mail' autoComplete="on" />
        </div>
        <div>
          <label htmlFor='password'>Password</label>
          <input id='password' name='password' type='password' placeholder='Enter your passowrd' autoComplete="on" />
        </div>
      </div>
      <div>
        <button>Log In</button>
        <button>Sign Up</button>
      </div>
    </form>
  )
}