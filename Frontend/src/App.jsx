import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './features/auth/AuthContext';
import { LoginPage } from './features/auth/LoginPage';
import { RegisterPage } from './features/auth/RegisterPage';
import { ProtectedRoute } from './app/ProtectedRoute';
import { HomePage } from './app/HomePage';
import { CreateSubforumPage } from './features/forum/CreateSubforumPage';
import { CreateThreadPage } from './features/forum/CreateThreadPage';
import { ThreadDetailPage } from './features/forum/ThreadDetailPage';
import { ThreadsPage } from './features/forum/ThreadsPage';
import { MyProposalsPage } from './features/forum/MyProposalsPage';
import { ModeratorRoute } from './app/ModeratorRoute';
import { ModerationQueuePage } from './features/forum/ModerationQueuePage';

export function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          <Route path="/" element={<ProtectedRoute><HomePage/></ProtectedRoute>}/>
          <Route path="/subforums/new" element={<ProtectedRoute><CreateSubforumPage /></ProtectedRoute>}/>
          <Route path="/subforums/:subforumId/threads/new" element={<ProtectedRoute><CreateThreadPage /></ProtectedRoute>}/>

          <Route path="/subforums/:subforumId/threads" element={<ProtectedRoute><ThreadsPage /></ProtectedRoute>}/>
          <Route path="/threads/:threadId" element={<ProtectedRoute><ThreadDetailPage /></ProtectedRoute>}/>

          <Route path="/subforums/mine" element={<ProtectedRoute><MyProposalsPage /></ProtectedRoute>} />
          <Route path="/moderation/subforums/pending" element={<ModeratorRoute><ModerationQueuePage /></ModeratorRoute>} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
