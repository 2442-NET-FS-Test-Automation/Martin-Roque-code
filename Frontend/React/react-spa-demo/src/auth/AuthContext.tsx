import { createContext, useReducer } from "react";
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

function initAuthState(): AuthState {
    const token = getToken();
    const user = token ? decodeToken(token) : null;

    if (!user) return initialAuthState;

    return {status: "authenticated", user, error:null};
}

export function AuthProvider({ children }: {children: ReactNode}){
    const [state, dispatch] = useReducer(authReducer, undefined, initAuthState);

    // useEffect(() => {

    //     const token = getToken();

    //     if(!token) return; // if there is no token

    //     const user = decodeToken(token);

    //     // Calling the login_success case of our reducer function via dispatch
    //     if (user) dispatch( {type: "login_success", user});
    //     else clearToken();

    // }, []);

    // Our login method 
    async function login(username: string, password: string): Promise<boolean> {

        dispatch({type: "login_start"});

        try{
            const token = await loginRequest(username, password);
            const user = decodeToken(token);

            if (!user) throw new Error("token missing expected claims");
            
            // If our token is present with the correct claims - we can now store it
            setToken(token);
            dispatch({type: "login_success", user})
            return true; 

        }catch{
            dispatch({type:"login_failure", error: "Invalid username or password"})
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