import axios from "axios";

//One axios object/instance for the whole app.

export const api = axios.create({
    baseURL: "http://localhost:5045"
});