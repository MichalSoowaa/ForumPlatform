import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { useNavigate, Link } from 'react-router-dom';
import { createSubforum } from './forumapi';
import { extractErrorMessage } from '../errorMessage';

export function CreateSubforumPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ name: '', description: '' });
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: createSubforum,
    onSuccess: (result) => {
      queryClient.invalidateQueries({ queryKey: ['subforums'] });
      navigate('/', { state: { message: `Subforum "${result.name}" created (status: ${result.status}).` } });
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
      <h1>Propose a subforum</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="name">Name</label>
          <input id="name" name="name" type="text" value={form.name} onChange={handleChange} required />
        </div>
        <div>
          <label htmlFor="description">Description</label>
          <textarea
            id="description"
            name="description"
            value={form.description}
            onChange={handleChange}
            required
          />
        </div>

        {mutation.isError && <p role="alert">{extractErrorMessage(mutation.error)}</p>}

        <button type="submit" disabled={mutation.isPending}>
          {mutation.isPending ? 'Creating…' : 'Create subforum'}
        </button>
      </form>

      <p>
        <Link to="/">Back home</Link>
      </p>
    </div>
  );
}