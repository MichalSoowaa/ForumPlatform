import { useQuery, keepPreviousData } from "@tanstack/react-query";
import { Link, useParams } from "react-router-dom";
import { getThreadsBySubforum } from "./forumapi";
import { formatScore } from "./formatScore";
import { useState } from "react";
import { SortSelect } from "./SortSelect";

export function ThreadsPage() {
    const { subforumId } = useParams();
    const [sort, setSort] = useState('Activity');

    const { data: threads, isLoading, isError } = useQuery({
        queryKey: ["threads", subforumId, sort],
        queryFn: () => getThreadsBySubforum(subforumId, sort),
        placeholderData: keepPreviousData, // keep previous data while loading new data when sort changes
    });

    if (isLoading)
        return <p>Loading threads...</p>;
    if (isError)
        return <p role="alert">Failed to load threads.</p>;

    return (
        <div>
            <h1>Threads</h1>
            <p><Link to={`/subforums/${subforumId}/threads/new`}>Create Thread</Link></p>
            <SortSelect value={sort} onChange={setSort} />
            <ul>
                {threads.map((thread) => (
                    <li key={thread.id}>
                        <Link to={`/threads/${thread.id}`}>{thread.title}</Link>
                        {' - by '}{thread.author} [{formatScore(thread.authorReputation, 1)}] ({thread.createdAt})
                        {' - score '}{formatScore(thread.score, 3)}
                        {' - '}{thread.commentCount}{' comments'}
                    </li>
                ))}
            </ul>
            <p><Link to="/">Back to Subforums</Link></p>
        </div>
    );
}