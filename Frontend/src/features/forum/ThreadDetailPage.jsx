import { useState } from 'react';
import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { useParams, Link } from 'react-router-dom';
import { getThreadDetail, createComment } from './forumapi';
import { VoteButtons } from '../reputation/VoteButtons';
import { extractErrorMessage } from '../errormessage';
import { useAuth } from '../auth/AuthContext';
import { formatScore } from './formatScore';
import { SortSelect } from './SortSelect';

export function ThreadDetailPage() {
  const { threadId } = useParams();
  const queryClient = useQueryClient();
  const [sort, setSort] = useState('Quality');

  const { user } = useAuth();
  const isOwn = (authorUsername) => user?.username === authorUsername;

  const { data: thread, isLoading, isError, error } = useQuery({
    queryKey: ['thread', threadId, sort],
    queryFn: () => getThreadDetail(threadId, sort),
    placeholderData: keepPreviousData, // keep previous data while loading new data when sort changes
  });

  const [body, setBody] = useState('');
  const [replyingTo, setReplyingTo] = useState(null); // { commentId, depth } | null

  function refreshScores() {
    queryClient.invalidateQueries({ queryKey: ['thread', threadId] });
    queryClient.invalidateQueries({ queryKey: ['threads'] }); 
  }

  const mutation = useMutation({
    mutationFn: createComment,
    onSuccess: () => {
      setBody('');
      setReplyingTo(null);
      queryClient.invalidateQueries({ queryKey: ['thread', threadId] });
    },
  });

  function handleSubmit(e) {
    e.preventDefault();
    mutation.mutate({
      body,
      threadId,
      parentCommentId: replyingTo?.commentId ?? null,
    });
  }

  if (isLoading) return <p>Loading thread...</p>;
  if (isError) {
    console.error('Thread load failed:', error);
    
    return (
      <p role="alert">Failed to load thread: {extractErrorMessage(error)}</p>
    );
  } 

  const comments = thread.comments ?? [];
  const topLevelComments = thread.comments.filter((c) => c.depth === 0);
  const repliesByParent = Object.groupBy(thread.comments.filter((c) => c.depth > 0), (c) => c.parentCommentId);

  return (
    <div>
      <h1>{thread.title}</h1>
      <p>by {thread.author} [{formatScore(thread.authorReputation, 1)}] ({thread.createdAt})</p>
      <p>score {thread.finalScore.toFixed(3)} - {comments.length} comments</p>
      {!isOwn(thread.author) && (
        <VoteButtons contentId={thread.id} contentType="Thread" onVoted={refreshScores} />
      )}
      <p>{thread.content}</p>

      <h2>Comments</h2>
      <SortSelect value={sort} onChange={setSort} />
      <ul>
        {topLevelComments.map((c) => (
          <li key={c.id}>
            <p>{c.author} [{formatScore(c.authorReputation, 1)}]: {c.content} <small>(score {c.finalScore.toFixed(2)})</small></p>
            {!isOwn(c.author) && (
              <VoteButtons contentId={c.id} contentType="Comment" onVoted={refreshScores} />
            )}
            <button onClick={() => setReplyingTo({ commentId: c.id, depth: c.depth })}>
              Reply
            </button>
            <ul>
              {(repliesByParent[c.id] ?? []).map((reply) => (
                <li key={reply.id}>
                  <p>{reply.author} [{formatScore(reply.authorReputation, 1)}]: {reply.content} <small>(score {reply.finalScore.toFixed(2)})</small></p>
                  {!isOwn(reply.author) && (
                    <VoteButtons contentId={reply.id} contentType="Comment" onVoted={refreshScores} />
                  )}
                </li>
              ))}
            </ul>
          </li>
        ))}
      </ul>

      <form onSubmit={handleSubmit}>
        {replyingTo && (
          <p>
            Replying to a comment{' '}
            <button type="button" onClick={() => setReplyingTo(null)}>
              Cancel
            </button>
          </p>
        )}
        <div>
          <label htmlFor="body">{replyingTo ? 'Reply' : 'Comment'}</label>
          <textarea id="body" value={body} onChange={(e) => setBody(e.target.value)} required />
        </div>

        {mutation.isError && <p role="alert">{extractErrorMessage(mutation.error)}</p>}

        <button type="submit" disabled={mutation.isPending}>
          {mutation.isPending ? 'Posting…' : 'Post'}
        </button>
      </form>

      <p>
        <Link to="/">Back home</Link>
      </p>
    </div>
  );
}