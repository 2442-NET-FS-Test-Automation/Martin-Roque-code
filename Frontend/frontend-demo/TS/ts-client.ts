// This will act as our "ApiClient" class
export interface ApiError {
    status: number;
    message: string;
}

export class ApiClient {
    constructor(private readonly baseUrl: string = "http://localhost:5045") {}

    async getJson<T>(path: string): Promise<T | ApiError> {
        try {
            const res = await fetch(`${this.baseUrl}${path}`)

            if(!res.ok) return { status: res.status, message: `API said: ${res.status}`};

            return await res.json() as T;
        } catch {
            return { status: 0, message: "Cannot reach the API. Check if it's on, or CORS"};
        }
    }
}