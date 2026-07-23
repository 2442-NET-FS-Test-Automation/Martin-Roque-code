import { Component, type ComponentType } from "react";
import { RequireAuth } from "./RequireAuth";

export function withAdminOnly<P extends object>(component: ComponentType<P>) {
    return function AdminGuarded(props: P) {
        return(
            <RequireAuth role="admin">
                <Component {...props}/>
            </RequireAuth>
        )
    }
}