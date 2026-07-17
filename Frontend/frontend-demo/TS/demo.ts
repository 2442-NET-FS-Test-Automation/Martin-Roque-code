import { InventoryItem, HttpStatus, SortDirection, SupplierPrice,
    FetchState, Sku, InventoryPatch, NewInventoryItem
 } from "./types.js";

 import { ApiClient, isApiError } from "./ts-client.js";

let catalog: InventoryItem[] = [];
let catalogState: FetchState = "idle";

const MESSAGES = {
    signIn: "sign in to see supplier prices",
    empty: "the catalog is empty"
} as const;
// [
//     {sku: "BK-001", name: "Clean Code", currentStock: 5},
//     {sku: "BK-002", name: "Dune", currentStock: 3},
//     {sku: "BK-003", name: "Refactorin", currentStock: 8}
// ];

const SORT_KEYS = ["name", "sku", "currentStock"] as const;
type SortKey = typeof SORT_KEYS[number];

function isSortKey(value: string): value is SortKey{
    if (!(SORT_KEYS as readonly string[]).includes(value))
        return false;
    
    return true;
}

function sortBy<K extends keyof InventoryItem> (items: InventoryItem[],
    key: K, dir: SortDirection): InventoryItem[]{
        const isDescending = dir === SortDirection.Descending;

        const arrayCopy = [...items];

        arrayCopy.sort(( itemA, itemB) => {
            const valueA = itemA[key];
            const valueB = itemB[key];

            if(typeof valueA === "number" && typeof valueB === "number") {
                if(isDescending)
                    return valueB - valueA;
                else
                    return valueA - valueB;
            }

            const stringA = String(valueA);
            const stringB = String(valueB);

            if(isDescending){
                return stringB.localeCompare(stringA);
            }else{
                return stringA.localeCompare(stringB);
            }
        });

        return arrayCopy;
}

function printSorted(key: SortKey, dir: SortDirection): void{
    console.log(`\nsorted by ${key} ${dir}`);
    printCatalog(sortBy(catalog, key, dir));
}

function printCatalog(items: InventoryItem[]): void {
    if(items.length === 0){
        console.log(MESSAGES.empty);
        return;
    }

    for (const item of items) {
        console.log(`${item.sku} ${item.name} ${item.currentStock}`);
    }
}

async function loadCatalog(): Promise<void> {
    catalogState = "loading";
    console.log("loading catalog...");

    const result = await api.getJson<InventoryItem[]>("/api/Inventory");

    if (isApiError(result)) {
        catalogState = "failed";
        console.log(result.message);
        return;
    }
}

async function showSupplierPrice(sku: Sku): Promise<void> {
    
    const result = await api.getJson<SupplierPrice>(`/api/Inventory/${sku}/supplier-price`);

    if(isApiError(result)) {
        if (result.status === HttpStatus.Unauthorized) {
            console.log(MESSAGES.signIn);
        }
        else {
            console.log(result.message);
        }
        return; 
    }
    console.log(`supplier list at ${result.supplierPrice}`);
}

console.log("catalog: ");
printCatalog(catalog);

console.log(HttpStatus.Unauthorized);
console.log(HttpStatus[401]);

const api = new ApiClient();

const liveCatalog = await api.getJson("/api/Inventory");

console.log(liveCatalog);