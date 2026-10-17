import { useAuth } from '../features/auth/AuthContext';
import { Link, useLocation } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getSubforums } from '../features/forum/forumapi';

export function HomePage() {
    const { user, logout } = useAuth();
    const location = useLocation();
    const flashMessage = location.state?.message;

    const { data: subforums, isLoading, isError } = useQuery({
        queryKey: ['subforums'],
        queryFn: getSubforums,
    });

    return (
        <div>
            <header>
                <p>
                    Signed in as {user?.username} {user?.email}{' '}  
                    <button onClick={logout}>Log out</button>     
                </p>
            </header>
            
            {flashMessage && <p role="status">{flashMessage}</p>}

            <h1>Subforums</h1>
            <p><Link to="/subforums/new">Propose a subforum</Link></p>

            {isLoading && <p>Loading subforums...</p>}
            {isError && <p role="alert">Failed to load subforums.</p>}

            {subforums && subforums.length === 0 && <p>No subforums available.</p>}

            {subforums && subforums.length > 0 && (
                <ul>
                    {subforums.map((subforum) => (
                        <li key={subforum.id}>
                            <Link to={`/subforums/${subforum.id}/threads`}>{subforum.name}</Link>
                            <p>{subforum.description}</p>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}