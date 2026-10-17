import { apiClient } from '../../api/client';

// contentType: 'Thread' | 'Comment'   value: 'Upvote' | 'Downvote'
// (strings, matching the JsonStringEnumConverter configured on the backend)
export async function castVote({ contentId, contentType, value }) {
    const { data } = await apiClient.post('/votes', { contentId, contentType, value });
    return data; // { voteId, value, weightApplied }
}