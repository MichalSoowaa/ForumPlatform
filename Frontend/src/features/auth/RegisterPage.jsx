        import { useState } from 'react';
        import { useNavigate, Link } from 'react-router-dom';
        import { useAuth } from './AuthContext';

        export function RegisterPage() {
            const { register, registerStatus, registerError } = useAuth();
            const navigate = useNavigate();

            const [form, setForm] = useState({
                email: '',
                username: '',
                password: '',
                passwordConfirm: '',
            });

            function handleChange(e) {
                setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
            }

            async function handleSubmit(e) {
                e.preventDefault();

                try {
                    await register(form);
                    navigate('/');
                } catch {
                    // registerError is already surfaced below via useAuth's state
                }
            }

            return (
                <div>
                    <h1>Create an account</h1>
                    <form onSubmit={handleSubmit}>
                        <div>
                            <label htmlFor="email">Email</label>
                            <input id="email" name="email" type="email" value={form.email} onChange={handleChange} required />
                        </div>
                        <div>
                            <label htmlFor="username">Username</label>
                            <input id="username" name="username" type="text" value={form.username} onChange={handleChange} required />
                        </div>
                        <div>
                            <label htmlFor="password">Password</label>
                            <input id="password" name="password" type="password" value={form.password} onChange={handleChange} required />
                        </div>
                        <div>
                            <label htmlFor="passwordConfirm">Confirm password</label>
                            <input id="passwordConfirm" name="passwordConfirm" type="password" value={form.passwordConfirm} onChange={handleChange} required />
                        </div>

                        {registerStatus === 'error' && (
                            <p role="alert">
                                {registerError?.response?.data?.extensions?.errors
                                    ? Object.values(registerError.response.data.extensions.errors).flat().join(' ')
                                    : registerError?.response?.data?.detail 
                                        ?? 'Something went wrong. Please try again.'}
                            </p>
                        )}

                        <button type="submit" disabled={registerStatus === 'pending'}>
                            {registerStatus === 'pending' ? 'Creating account...' : 'Create account'}
                        </button>
                    </form>

                    <p>
                        Already have an account? <Link to="/login">Log in</Link>
                    </p>
                </div>
            );
        }