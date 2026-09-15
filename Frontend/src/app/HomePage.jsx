import { useAuth } from '../features/auth/AuthContext';
import { Link, useLocation } from 'react-router-dom';

export function HomePage() {
    const { user, logout } = useAuth();
    const location = useLocation();
    const flashMessage = location.state?.message;

    return (
        <div>
            <h1>Welcome, {user?.username}</h1>
            <p>Signed in as {user?.email}</p>

            {flashMessage && <p role="status">{flashMessage}</p>}

            <nav>
                <ul>
                    <li>
                        <Link to="/subforums/new">Propose a subforum</Link>
                    </li>
                    <li>
                        <Link to="/threads/new">Create a thread</Link>
                    </li>
                </ul>
            </nav>

            <button onClick={logout}>Log out</button>
        </div>
    );
}