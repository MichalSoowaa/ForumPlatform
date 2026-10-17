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