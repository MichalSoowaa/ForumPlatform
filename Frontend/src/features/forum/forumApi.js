import { apiClient } from '../../api/client';

export async function createSubforum({ name, description }) {
    const { data } = await apiClient.post('/subforums', { name, description });
    return data; // CreateSubforumResponse: { subforumId, name, status }
}

export async function createThread({ subforumId, title, content }) {
    const { data } = await apiClient.post(`/threads`, { title, content, subforumId });
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