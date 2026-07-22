import { useState } from "react";
import type { SubmitEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/useAuth";

export function LoginPage() {
    const { login, status } = useAuth();
    
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);

    const navigate = useNavigate();

    async function onSubmit(e: SubmitEvent<HTMLFormElement>) {
        e.preventDefault(); //stop try to sen a POST to a URL

        setError(null);
        
        const ok = await login(username, password);

        if(ok) navigate("/");
        else setError("Invalid username or password");
    }

    return (
        <form className="login" onSubmit={onSubmit}>
            <h2>Sign in</h2>
            <label>
                Username
                <input 
                    type="text"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)} 
                />
            </label>
            <label>
                Password
                <input
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
            </label>
            <button type="submit" disabled={status === "authenticating"}>
                {status === "authenticating" ? "Signing in..." : "Sign in"}
            </button>
            {error && <p className="error">{error}</p>}
        </form>
    )
}