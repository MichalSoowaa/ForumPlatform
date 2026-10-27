import { useState } from 'react';
import { useMutation, useQueryClient, useQuery } from '@tanstack/react-query';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { createThread, findSimilarThreads } from './forumapi';
import { extractErrorMessage } from '../errormessage';
import { useDebouncedValue } from '../../lib/useDebouncedValue';

export function CreateThreadPage() {
  const { subforumId } = useParams();
  const navigate = useNavigate();
  const [form, setForm] = useState({ title: '', body: '' });
  const queryClient = useQueryClient();
  const debouncedTitle = useDebouncedValue(form.title.trim(), 800);
  const debouncedBody = useDebouncedValue(form.body.trim().slice(0, 2000), 800);

  const { data: similar } = useQuery({
    queryKey: ['similar-threads', debouncedTitle, debouncedBody],
    queryFn: () => findSimilarThreads({ title: debouncedTitle, body: debouncedBody }),
    enabled: debouncedTitle.length >= 5 || debouncedBody.length >= 20,
    staleTime: 5 * 60 * 1000,
    retry: false,
  });

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

        {similar && similar.length > 0 && (
          <aside role="note">
            <p>Similar threads already exist. Your question may already be answered:</p>
            <ul>
              {similar.map((t) => (
                <li key={t.id}>
                  <Link to={`/threads/${t.id}`} target="_blank" rel="noreferrer">{t.title}</Link>
                  {' '}({t.subforumName}, {Math.round(t.similarity * 100)}% similar)
                </li>
              ))}
            </ul>
            <small>You can still post your thread.</small>
          </aside>
        )}

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