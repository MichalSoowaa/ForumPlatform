import { useAuth } from '../features/auth/AuthContext';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getSubforums } from '../features/forum/forumapi';
import { useState } from 'react';

export function HomePage() {
    const { user, logout, isModerator } = useAuth();
    const location = useLocation();
    const flashMessage = location.state?.message;
    const navigate = useNavigate();
    const [searchText, setSearchText] = useState('');

    const { data: subforums, isLoading, isError } = useQuery({
        queryKey: ['subforums'],
        queryFn: getSubforums,
    });

    function handleSearch(e) {
        e.preventDefault();
        const trimmed = searchText.trim();
        if (trimmed.length >= 2)
            navigate('/search?=${encodeURIComponent(trimmed)}');
    }

    return (
        <div>
            <header>
                <p>
                    Signed in as {user?.username} {user?.email}{' '}  
                    <button onClick={logout}>Log out</button>     
                </p>
            </header>
            
            {flashMessage && <p role="status">{flashMessage}</p>}

            <nav>
                <Link to="/subforums/new">Propose a subforum</Link>
                {' · '}
                <Link to="/subforums/mine">My proposals</Link>
                {isModerator && (
                    <>
                        {' · '}
                        <Link to="/moderation/subforums/pending">Moderation queue</Link>
                    </>
                )}
            </nav>

            <form onSubmit={handleSearch}>
                <input
                    type="search"
                    value={searchText}
                    onChange={(e) => setSearchText(e.target.value)}
                    placeholder="Search threads"
                />{' '}
                <button type="submit" disabled={searchText.trim().length < 2}>Search</button>
            </form>

            <h1>Subforums</h1>

            {isLoading && <p>Loading subforums...</p>}
            {isError && <p role="alert">Failed to load subforums.</p>}

            {subforums && subforums.length === 0 && <p>No subforums available.</p>}

            {subforums && subforums.length > 0 && (
                <ul>
                    {subforums.map((subforum) => (
                        <li key={subforum.id}>
                            <Link to={`/subforums/${subforum.id}/threads`}>{subforum.name}</Link>
                            {subforum.tags.length > 0 && <small>[{subforum.tags.join(', ')}]</small>}
                            <p>{subforum.description}</p>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}