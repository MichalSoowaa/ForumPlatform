import { useAuth } from '../features/auth/AuthContext';

export function HomePage() {
    const { user, logout } = useAuth();

    return (
        <div>
            <h1>Welcome, {user?.username}</h1>
            <p>Signed in as {user?.email}</p>
            <button onClick={logout}>Log out</button>
        </div>
    );
}