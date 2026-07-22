import axios from "axios";
import { getToken } from "../auth/storage";

//One axios object/instance for the whole app.

export const api = axios.create({
    baseURL: "http://localhost:5045"
})

//Request interceptor - Attack that bearer token (if we have it) to EVERY call

api.interceptors.request.use((config) => {
    const token = getToken;
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});