import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { useParams, Link } from 'react-router-dom';
import { createComment } from './forumApi';
import { extractErrorMessage } from '../errorMessage';

export function ThreadCommentPage() {
  const { threadId } = useParams();
  const [body, setBody] = useState('');

  // Tracks comments posted in this session so you can pick one to reply to —
  // no GET /threads/:id/comments endpoint exists yet, so this is just local
  // state to prove the reply/depth flow works, not a real comment list.
  const [postedComments, setPostedComments] = useState([]);
  const [replyingTo, setReplyingTo] = useState(null); // { commentId, depth } | null

  const mutation = useMutation({
    mutationFn: createComment,
    onSuccess: (result) => {
      setPostedComments((prev) => [...prev, result]);
      setBody('');
      setReplyingTo(null);
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

  return (
    <div>
      <h1>Thread {threadId}</h1>

      <ul>
        {postedComments.map((c) => (
          <li key={c.commentId} style={{ marginLeft: c.depth * 24 }}>
            (depth {c.depth}) {c.commentId}
            {c.depth < 1 && (
              <button onClick={() => setReplyingTo({ commentId: c.commentId, depth: c.depth })}>
                Reply
              </button>
            )}
          </li>
        ))}
      </ul>

      <form onSubmit={handleSubmit}>
        {replyingTo && (
          <p>
            Replying to {replyingTo.commentId}{' '}
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