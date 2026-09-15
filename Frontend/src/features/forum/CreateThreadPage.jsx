import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { useNavigate, Link } from 'react-router-dom';
import { createThread } from './forumApi';
import { extractErrorMessage } from '../errorMessage';

export function CreateThreadPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ title: '', body: '', subforumId: '' });

  const mutation = useMutation({
    mutationFn: createThread,
    onSuccess: (result) => {
      // No thread-detail page yet — jump straight to the comment form for the
      // new thread, since that's the next link in the vertical slice.
      navigate(`/threads/${result.threadId}/comment`);
    },
  });

  function handleChange(e) {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  }

  function handleSubmit(e) {
    e.preventDefault();
    mutation.mutate(form);
  }

  return (
    <div>
      <h1>Create a thread</h1>
      <p>
        No subforum list yet — paste the subforum's Id (returned when you created it,
        or check the database directly for now).
      </p>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="subforumId">Subforum Id</label>
          <input
            id="subforumId"
            name="subforumId"
            type="text"
            value={form.subforumId}
            onChange={handleChange}
            required
          />
        </div>
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
        <Link to="/">Back home</Link>
      </p>
    </div>
  );
}