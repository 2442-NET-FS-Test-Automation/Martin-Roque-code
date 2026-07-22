import { createContext, useEffect, useReducer } from "react";
import type { ReactNode } from "react";
import { login as loginRequest } from "../api/auth";
import { decodeToken } from "./jwt";
import { getToken, setToken, clearToken } from "./storage";
import { authReducer, initialAuthState } from "./authReducer";
import type { AuthState } from "./authReducer";

interface AuthContextValue extends AuthState {
    login: (username: string, password: string) => Promise<boolean>;
    logout: () => void;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: {children: ReactNode}){
    const [state, dispatch] = useReducer(authReducer, initialAuthState);

    useEffect(() => {
        const token = getToken();

        if(!token) return;

        const user = decodeToken(token);

        if(user) dispatch({type: "login_success", user});
        else clearToken();
    }, []);

    async function login(username: string, password: string): Promise<boolean> {
        dispatch({type: "login_start"});

        try{
            const token = await loginRequest(username, password);
            const user = decodeToken(token);

            if(!user) throw new Error("token missing expected claims");

            setToken(token);
            dispatch({type: "login_success", user})
            return true;

        }catch{
            dispatch({type: "login_failure", error: "Invalid username or password"})
            return false;
        }
    }

    function logout(){
        clearToken();
        dispatch({ type: "logout"});
    }

    return (
        <AuthContext.Provider value={{...state, login, logout}}>
            {children}
        </AuthContext.Provider>
    );
}