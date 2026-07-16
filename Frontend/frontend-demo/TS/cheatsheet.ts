//Just language basics

//Simple + array types
import { Sku, FetchState } from "./types.js";

let sku: string = "BK-101"
let price: number = 29.99
let inStock: boolean = true;

let name = "jon";

let tags: string[] = ["architecture", "classic"];

console.log(sku, price, inStock, tags, name);

let anything: any = "escape hatch";
anything = 42;
console.log(anything);

// "unknown" - no idea what this might be - but compiler make you prove a shape before you can use it
let incoming: unknown = JSON.parse(' "?" ');

if(typeof incoming === "string") {
    console.log(incoming.toUpperCase());
}

type PriceRange = [min: number, max: number];

let state: FetchState = "loading";

const range: PriceRange = [0,50];

const bkSku: Sku = "BK-101";

console.log(state, range[0], range[1], bkSku);