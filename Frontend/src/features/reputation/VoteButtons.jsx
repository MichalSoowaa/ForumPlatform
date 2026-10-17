import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { castVote } from './reputationApi';
import { extractErrorMessage } from '../errorMessage';

export function VoteButtons({ contentId, contentType, onVoted }) {
    // Session-only: the API doesn't tell us the user's existing vote yet, so
    // this resets on page refresh.
    
    const [userVote, setUserVote] = useState(null); // 'Upvote' | 'Downvote' | null

    const mutation = useMutation({
        mutationFn: castVote,
        onSuccess: (_result, variables) => {
            setUserVote(variables.value);
            onVoted?.(variables.value);
        },
    });

    function vote(value) {
        mutation.mutate({ contentId, contentType, value });
    }

    return (
        <span>
            <button type="button" onClick={() => vote('Upvote')} disabled={mutation.isPending} aria-pressed={userVote === 'Upvote'}
                style={{ fontWeight: userVote === 'Upvote' ? 'bold' : 'normal' }}>
                ▲
            </button>
            
            <button type="button" onClick={() => vote('Downvote')} disabled={mutation.isPending} aria-pressed={userVote === 'Downvote'}
                style={{ fontWeight: userVote === 'Downvote' ? 'bold' : 'normal' }}>
                ▼
            </button>

            {mutation.isError && <small role="alert" style={{ color: 'red' }}>{extractErrorMessage(mutation.error)}</small>}
        </span>
    );
}