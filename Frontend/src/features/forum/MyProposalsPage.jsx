import { useQuery } from '@tanstack/react-query';
import { Link, useLocation } from 'react-router-dom';
import { getMySubforums } from './forumapi';

export function MyProposalsPage() {
    const location = useLocation();
    const flash = location.state?.message;

    const { data: proposals, isLoading, isError } = useQuery({ 
        queryKey: ['subforums', 'mine'],
        queryFn: getMySubforums,
    });

    if (isLoading)
        return <div>Loading...</div>;

    if (isError)
        return <p role="alert">Something went wrong while fetching your proposals. Please try again later.</p>;

    return (
        <div>
            <h1>My Proposals</h1>
            {flash && <p role="alert">{flash}</p>}
            {proposals.length === 0 && <p>You have not submitted any proposals yet.</p>}
            <ul>
                {proposals.map((s) => (
                    <li key={s.id}>
                        <strong>{s.name}</strong> - {s.status}
                        {s.status === 'Approved' && (
                            <> · <Link to={`/subforums/${s.id}/threads`}>Open</Link> </>
                        )}
                        <p>{s.description}</p>
                        {s.tags.length > 0 && <p>Tags: {s.tags.join(', ')}</p>}
                        {s.status === 'Rejected' && s.rejectionReason && <p>Rejection reason: {s.rejectionReason}</p>}
                    </li>
                ))}
            </ul>
            <p><Link to="/">Back to main page</Link></p>
        </div>
    );
}