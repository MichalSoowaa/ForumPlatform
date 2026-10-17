export function formatScore(score, digits = 2) {
    return typeof score === 'number' ? score.toFixed(digits) : '-';
}