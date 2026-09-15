import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './features/auth/AuthContext';
import { LoginPage } from './features/auth/LoginPage';
import { RegisterPage } from './features/auth/RegisterPage';
import { ProtectedRoute } from './app/ProtectedRoute';
import { HomePage } from './app/HomePage';
import { CreateSubforumPage } from './features/forum/CreateSubforumPage';
import { CreateThreadPage } from './features/forum/CreateThreadPage';
import { ThreadCommentPage } from './features/forum/ThreadCommentPage';

export function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/" element={
            <ProtectedRoute>
              <HomePage />
            </ProtectedRoute>
            }
          />
          <Route path="/subforums/new"
            element={
              <ProtectedRoute>
                <CreateSubforumPage />
              </ProtectedRoute>
            }
          />
          <Route path="/threads/new"
            element={
              <ProtectedRoute>
                <CreateThreadPage />
              </ProtectedRoute>
            }
          />
          <Route path="/threads/:threadId/comment"
            element={
              <ProtectedRoute>
                <ThreadCommentPage />
              </ProtectedRoute>
            }
          />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
