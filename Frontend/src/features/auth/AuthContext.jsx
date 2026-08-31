import { createContext, useContext, useState, useCallback } from 'react';
import { useMutation } from '@tanstack/react-query'
import { loginUser, registerUser } from './authApi'

const AuthContext = createContext(null);

// Reads any previously-saved session on first load, so a page refresh
// doesn't log the user out. Only persist the token + minimal user info,
// never the password.
function loadStoredSession() {
    const token = localStorage.getItem('authToken');
    const userJson = localStorage.getItem('authUser');

    if (!token || !userJson)
        return { token: null, user: null };

    try {
        return { token, user: JSON.parse(userJson) };
    } catch {
        return { token: null, user: null };
    }
}

function persistSession(token, user) {
    localStorage.setItem('authToken', token);
    localStorage.setItem('authUser', JSON.stringify(user));
}

function clearSession() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('authUser');
}

export function AuthProvider({ children }) {
    const [{ token, user }, setSession] = useState(loadStoredSession);

    const applySession = useCallback((response) => {
        const nextUser = {
            userId: response.userId,
            email: response.email,
            username: response.username,
        };

        persistSession(response.token, nextUser);
        setSession({ token: response.token, user: nextUser });
    }, []);

    const loginMutation = useMutation({
        mutationFn: loginUser,
        onSuccess: applySession,
    });

    const registerMutation = useMutation({
        mutationFn: registerUser,
        onSuccess: applySession,
    });

    const logout = useCallback(() => {
        clearSession();
        setSession({ token: null, user: null });
    }, []);

    const value = {
        token,
        user,
        isAuthenticated: Boolean(token),
        login: loginMutation.mutateAsync,
        loginStatus: loginMutation.status,
        loginError: loginMutation.error,
        register: registerMutation.mutateAsync,
        registerStatus: registerMutation.status,
        registerError: registerMutation.error,
        logout,
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
    const ctx = useContext(AuthContext);

    if (!ctx)
        throw new Error('useAuth must be used within an AuthProvider');

    return ctx;
}