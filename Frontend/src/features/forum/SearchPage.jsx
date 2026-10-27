import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { searchThreads } from './forumapi';
import { useDebouncedValue } from '../../lib/useDebouncedValue';
import { Link, useSearchParams } from 'react-router-dom';
import { formatScore } from './formatScore';
import { extractErrorMessage } from '../errorMessage';

export function SearchPage() {
    // The query lives in the URL (/search?q=...) so results can be refreshed, bookmarked and shared.
    const [searchParams, setSearchParams] = useSearchParams();
    const q = (searchParams.get('q') ?? '').trim();
    const [text, setText] = useState(q);

    const { data: hits, isFetching, isError, error } = useQuery({
        queryKey: ['search', q],
        queryFn: () => searchThreads(q),
        enabled: q.length >= 2,
    });

    function handleSubmit(e) {
        e.preventDefault();
        setSearchParams({ q: text.trim() });
    }

    return (
    <div>
      <h1>Search</h1>

      <form onSubmit={handleSubmit}>
        <input
          type="search"
          value={text}
          onChange={(e) => setText(e.target.value)}
          placeholder="Search threads by topic or meaning"
        />{' '}
        <button type="submit" disabled={text.trim().length < 2}>Search</button>
      </form>

      {isFetching && <p>Searching…</p>}
      {isError && <p role="alert">{extractErrorMessage(error)}</p>}
      {hits && hits.length === 0 && !isFetching && <p>No threads found for "{q}".</p>}

      {hits && hits.length > 0 && (
        <ul>
          {hits.map((h) => (
            <li key={h.id}>
              <Link to={`/threads/${h.id}`}>{h.title}</Link>
              <p>{h.snippet}</p>
              <small>
                in <Link to={`/subforums/${h.subforumId}/threads`}>{h.subforumName}</Link>
                {' · by '}{h.authorUsername}
                {' · score '}{formatScore(h.finalScore)}
                {' · '}{h.commentCount}{' comments'}
                {' · '}
                {/* Shown for tuning the thresholds. Remove, or keep for transparency. */}
                {h.similarity == null ? 'keyword match only' : `semantic match ${h.similarity.toFixed(2)}`}
              </small>
            </li>
          ))}
        </ul>
      )}

      <p><Link to="/">Back to subforums</Link></p>
    </div>
  );
}