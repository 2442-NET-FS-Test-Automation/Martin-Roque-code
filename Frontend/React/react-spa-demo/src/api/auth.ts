import { api } from "./client";

export async function login( username: string, password: string): Promise<string> {
    const response = await api.post<{token: string}>("/auth/login", { username, password});
    return response.data.token;
}