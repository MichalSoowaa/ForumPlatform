/**
 * Extracts a displayable error message from an axios/mutation error,
 * matching the backend's actual response shapes:
 *  - FluentValidation failures: { extensions: { errors: { Field: ["msg", ...] } } }
 *  - Domain InvalidOperationException: { error: "msg" }
 *  - Anything else GlobalExceptionMiddleware catches: { detail: "msg" } 
 */
export function extractErrorMessage(error, fallback = "Something went wrong. Please try again.") {
    const data = error?.response?.data;

    if (!data)
         return fallback;

    if (data.extensions?.errors) {
        return Object.values(data.extensions.errors).flat().join("\n");
    }

    return data.error ?? data.detail ?? fallback;
}