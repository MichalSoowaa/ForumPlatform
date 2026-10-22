import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { getPendingSubforums, approveSubforum, rejectSubforum } from './forumapi';
import { extractErrorMessage } from '../errormessage';

const MAX_TAGS = 5; // mirrors Subforum.MaxTags on the backend

function PendingItem({ item }) {
    const queryClient = useQueryClient();
    const [tagsText, setTagsText] = useState('');
    const [reason, setReason] = useState('');

    function refresh() {
        queryClient.invalidateQueries({ queryKey: ['moderation', 'pending'] });
        queryClient.invalidateQueries({ queryKey: ['subforums'] });
    }

    const approve = useMutation({
        mutationFn: approveSubforum,
        onSuccess: refresh,
    });

    const reject = useMutation({
        mutationFn: rejectSubforum,
        onSuccess: refresh,
    });

    const tags = tagsText.split(',').map((t) => t.trim()).filter(Boolean);
    const busy = approve.isPending || reject.isPending;
    const error = approve.error ?? reject.error;

    return (
        <li>
            <h3>{item.name}</h3>
            <p>{item.description}</p>
            <p>
                <small>
                    Proposed by {item.proposerUsername} on {new Date(item.createdAt).toLocaleString()}
                </small>
            </p>

            <div>
                <label>
                    Tags (comma-separated, max {MAX_TAGS}):
                    <input
                        type="text"
                        value={tagsText}
                        onChange={(e) => setTagsText(e.target.value)}
                        disabled={busy}
                    />
                </label>
                <button type="button"
                    onClick={() => approve.mutate({ subforumId: item.id, tags })}
                    disabled={busy || tags.length > MAX_TAGS || tags.length === 0}
                >
                    Approve
                </button>
            </div>

            <div>
                <label>
                    Rejection reason:
                    <input
                        type="text"
                        value={reason}
                        onChange={(e) => setReason(e.target.value)}
                        disabled={busy}
                    />
                </label>
                <button type="button"
                    onClick={() => reject.mutate({ subforumId: item.id, reason })}
                    disabled={busy || !reason.trim()}
                >
                    Reject
                </button>
            </div>
            {error && <p role="alert">Error: {extractErrorMessage(error)}</p>}
        </li>
    );
}

export function ModerationQueuePage() {
    const { data: pending, isLoading, isError } = useQuery({
        queryKey: ['moderation', 'pending'],
        queryFn: getPendingSubforums,
    });

    if (isLoading) return <div>Loading...</div>;
    if (isError) return <p role="alert">Something went wrong while fetching the moderation queue. Please try again later.</p>;

    return (
        <div>
            <h1>Moderation Queue</h1>
            {pending.length === 0 &&
                <p>No pending subforum proposals.</p>
            }
            <ul>
                {pending.map((item) => (
                    <PendingItem key={item.id} item={item} />
                ))}
            </ul>
            
            <p><Link to="/">Back to main page</Link></p>
        </div>
    );
}