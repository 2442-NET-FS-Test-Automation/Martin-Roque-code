import { InventoryItem, HttpStatus, SortDirection, SupplierPrice,
    FetchState, Sku
 } from "./types.js";

 import { ApiClient } from "./ts-client.js";

const catalog: InventoryItem[] = [
    {sku: "BK-001", name: "Clean Code", currentStock: 5},
    {sku: "BK-002", name: "Dune", currentStock: 3},
    {sku: "BK-003", name: "Refactorin", currentStock: 8}
];

function printCatalog(items: InventoryItem[]): void {
    for (const item of items) {
        console.log(`${item.sku} ${item.name} ${item.currentStock}`);
    }
}

console.log("catalog: ");
printCatalog(catalog);

console.log(HttpStatus.Unauthorized);
console.log(HttpStatus[401]);

const api = new ApiClient();

const liveCatalog = await api.getJson("/api/Inventory");

console.log(liveCatalog);