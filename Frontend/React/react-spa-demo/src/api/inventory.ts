import { api } from "./client";
import type { InventoryItem } from "../types";

export async function getInventory(): Promise<InventoryItem[]>{
    const response = await api.get<InventoryItem[]>("/api/Inventory");
    return response.data;
}