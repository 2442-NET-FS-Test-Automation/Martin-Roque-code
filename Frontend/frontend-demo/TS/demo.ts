import { InventoryItem, HttpStatus, SortDirection } from "./types.js";

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