const OPTIONS = [
    { value: 'Quality', label: 'Quality' },
    { value: 'Freshness', label: 'Newest' },
    { value: 'Activity', label: 'Most Active' },
    { value: 'Expertise', label: 'Expertise' },
]

export function SortSelect({ value, onChange }) {
    return (
        <label>
            Sort by{' '}
            <select value={value} onChange={(e) => onChange(e.target.value)}>
                {OPTIONS.map((option) => (
                    <option key={option.value} value={option.value}>
                        {option.label}
                    </option>
                ))}
            </select>
        </label>
    );
}