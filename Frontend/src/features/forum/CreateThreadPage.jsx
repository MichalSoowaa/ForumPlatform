import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { createThread } from './forumapi';
import { extractErrorMessage } from '../errormessage';

export function CreateThreadPage() {
  const { subforumId } = useParams();
  const navigate = useNavigate();
  const [form, setForm] = useState({ title: '', body: '' });
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: createThread,
    onSuccess: (result) => {
      // Make the subforum's thread list refetch so the new thread shows up
      // when the user navigates back to it.
      queryClient.invalidateQueries({ queryKey: ['threads', result.subforumId] });
      navigate(`/threads/${result.threadId}`);
    },
  });

  function handleChange(e) {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  }

  function handleSubmit(e) {
    e.preventDefault();
    console.log('form at submit:', form);
  console.log('JSON payload:', JSON.stringify({ ...form, subforumId }));
    mutation.mutate({ ...form, subforumId });
  }

  return (
    <div>
      <h1>Create a thread</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="title">Title</label>
          <input id="title" name="title" type="text" value={form.title} onChange={handleChange} required />
        </div>
        <div>
          <label htmlFor="body">Body</label>
          <textarea id="body" name="body" value={form.body} onChange={handleChange} required />
        </div>

        {mutation.isError && <p role="alert">{extractErrorMessage(mutation.error)}</p>}

        <button type="submit" disabled={mutation.isPending}>
          {mutation.isPending ? 'Posting…' : 'Create thread'}
        </button>
      </form>

      <p>
        <Link to={`/subforums/${subforumId}/threads`}>Back to threads</Link>
      </p>
    </div>
  );
}