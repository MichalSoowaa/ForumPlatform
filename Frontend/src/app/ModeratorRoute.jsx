import { Navigate } from 'react-router-dom';
import { useAuth } from '../features/auth/AuthContext';

// UI convenience only: the server enforces the Moderator policy regardless.
export function ModeratorRoute({ children }) {
    const { isAuthenticated, isModerator } = useAuth();

    if (!isAuthenticated)
        return <Navigate to="/login" replace />;

    if (!isModerator)
        return <Navigate to="/" replace />;

    return children;
}