import { apiClient } from '../../api/client';

export async function createSubforum({ name, description }) {
    const { data } = await apiClient.post('/subforums', { name, description });
    return data; // CreateSubforumResponse: { subforumId, name, status }
}

export async function createThread({ subforumId, title, body }) {
    const { data } = await apiClient.post(`/threads`, { title, body, subforumId });
    return data; // CreateThreadResponse: { threadId, title, subforumId }
}

export async function createComment({ body, threadId, parentCommentId }) {
    const { data } = await apiClient.post(`/comments`, {
        body,
        threadId,
        parentCommentId: parentCommentId ?? null,
    });

    return data; // CreateCommentResponse: { commentId, threadId, parentCommentId, depth }
}

export async function getSubforums() {
    const { data } = await apiClient.get('/subforums');
    return data;
}

export async function getThreadsBySubforum(subforumId, sort) {
    const { data } = await apiClient.get(`/subforums/${subforumId}/threads`, { params: { sort } });
    return data;
}

export async function getThreadDetail(threadId, sort) {
    const { data } = await apiClient.get(`/threads/${threadId}`, { params: { sort } });
    return data;
}

export async function getMySubforums() {
    const { data } = await apiClient.get('/subforums/mine');
    return data;
}

export async function getPendingSubforums() {
  const { data } = await apiClient.get('/moderation/subforums/pending');
  return data;
}

export async function approveSubforum({ subforumId, tags }) {
  const { data } = await apiClient.post(`/moderation/subforums/${subforumId}/approve`, { tags });
  return data;
}

export async function rejectSubforum({ subforumId, reason }) {
  const { data } = await apiClient.post(`/moderation/subforums/${subforumId}/reject`, { reason });
  return data;
}

export async function searchThreads(q) {
  const { data } = await apiClient.get('/search', { params: { q } });
  return data;
}

export async function findSimilarThreads({ title, body }) {
  const { data } = await apiClient.post('/threads/similar', { title, body });
  return data;
}