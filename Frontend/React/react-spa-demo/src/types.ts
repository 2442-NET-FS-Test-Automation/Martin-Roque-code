// Mirror InventoryDTO (GET /api/Inventory)

export interface InventoryItem {
    sku: Sku;
    name: string;
    currentStock: number; 
}

export interface SupplierPrice {
    sku: Sku;
    supplierPrice: number;
}

// type aliases
// Useful for documenting semantic meaning/intent
export type Sku = string;

// Union type: a type that allows a variable to be one of several types
export type FetchState = "idle" | "loading" | "loaded" | "failed"

let accountId: string | number;

export const HttpStatus = {
    Ok: 200,
    Created: 201,
    NoContent: 204,
    BadRequest: 400,
    Unauthorized: 401,
    Forbidden: 403,
    NotFound: 404
} as const;

export type HttpStatus = typeof HttpStatus[keyof typeof HttpStatus];

export const SortDirection = {
    Ascending: "asc",
    Descending: "desc"
} as const;

export type SortDirection = typeof SortDirection[keyof typeof SortDirection];

export type InventoryPatch = Partial<InventoryItem>;

export type NewInventoryItem = Omit<InventoryItem, "sku">;