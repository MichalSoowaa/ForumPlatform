import { QueryClient } from '@tanstack/react-query';

// Sensible defaults for a forum app: don't refetch aggresively on every window focus,
// but don't cache forever either since vote counts change from other users' actions.
export const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            staleTime: 30_000,
            refetchOnWindowFocus: false,
            retry: 1,
        },
    },
});