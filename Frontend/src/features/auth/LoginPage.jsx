import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from './AuthContext';

export function LoginPage() {
  const { login, loginStatus, loginError } = useAuth();
  const navigate = useNavigate();

  const [form, setForm] = useState({ email: '', password: '' });

  function handleChange(e) {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    try {
      await login(form);
      navigate('/');
    } catch {
      // loginError is already surfaced below via useAuth's state.
    }
  }

  return (
    <div>
      <h1>Log in</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="email">Email</label>
          <input id="email" name="email" type="email" value={form.email} onChange={handleChange} required />
        </div>
        <div>
          <label htmlFor="password">Password</label>
          <input id="password" name="password" type="password" value={form.password} onChange={handleChange} required />
        </div>

        {loginStatus === 'error' && (
          <p role="alert">
            {loginError?.response?.status === 401
              ? 'Invalid email or password.'
              : 'Something went wrong. Please try again.'}
          </p>
        )}

        <button type="submit" disabled={loginStatus === 'pending'}>
          {loginStatus === 'pending' ? 'Logging in…' : 'Log in'}
        </button>
      </form>

      <p>
        Don&apos;t have an account? <Link to="/register">Create one</Link>
      </p>
    </div>
  );
}