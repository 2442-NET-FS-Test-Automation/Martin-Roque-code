//This will be a "controlled component"

interface SearchBarProps {
    value: string;
    onChange: (value: string) => void; //taking a function as a prop
}

export function SearchBar ({value, onChange}: SearchBarProps) {
    return (
        <input 
            type="search"
            placeholder="Filter by name..."
            value={value}
            onChange={(e) => onChange(e.target.value)}
        />
    );
}