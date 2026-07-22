import { Navigate, useLocation } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuth } from "../auth/useAuth";

interface RequireAuthProps{
    children: ReactNode;
    role?: string;
}

export function RequireAuth({children, role}: RequireAuthProps) {
    const {status, user} = useAuth();

    const location = useLocation();

    if (status !== "authenticated") 
        return <Navigate to="/login" state={{from: location}} replace/> //User cant hit the back button

    if ( role && user?.role !== role) return <Navigate to="/" replace/>

    return <>{children}</>
}