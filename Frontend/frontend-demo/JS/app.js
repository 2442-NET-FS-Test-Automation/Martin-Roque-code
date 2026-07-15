// Harcoding items until we call our API
// let catalogItems = [
//     { sku: "BK-101", name: "Clean Code",                price: 29.99, currentStock: 12 },
//     { sku: "BK-102", name: "The Pragmatic Programmer",  price: 34.99, currentStock: 7 },
//     { sku: "BK-103", name: "Design Patterns",           price: 44.99, currentStock: 3 },
//     { sku: "BK-104", name: "Refactoring",               price: 39.99, currentStock: 0 },
// ];

//Populate this empty array via fetch
let catalogItems = [];

function renderCards(items) {
    const container = document.getElementById("catalog-cards");

    if (items.length === 0){
        container.innerHTML = `<p class="hint">nothing matches</p>`;
        return;
    }

    container.innerHTML = items.map(item => `
            <article class="card" data-sku="${item.sku}">
                <h3>${item.name}</h3>
                <dl>
                    <dt>SKU</dt><dd>${item.sku}</dd>
                    <dt>Price</dt><dd>${item.price.toFixed(2)}</dd>
                    <dt>In Stock</dt><dd>${item.currentStock}</dd>
                </dl>
                <button class="price-btn" data-sku="${item.sku}">Supplier price</button>
                <p class="supplier-price"></p>
            </article>
        `
    ).join("");

   
}

document.querySelector("#catalog-cards").addEventListener("click", (e) => {
    if (e.target.matches(".price-btn")) {
        console.log("clicked", e.target.dataset.sku);
    }
});

//Filtering
document.querySelector("#search").addEventListener("input", (e) => {
    const search = e.target.value.trim().toLowerCase();

    renderCards(catalogItems.filter(item => 
        item.name.toLowerCase().includes(search) || item.sku.toLowerCase().includes(search)
    ));
});

 document.addEventListener("DOMContentLoaded", () => {
        renderCards(catalogItems);
});

// Trasient/temp - we will wrap this in methods later
//Promise chain example
fetch(`${API}/api/Inventory`)
    .then(res => res.json())
    .then(items => { catalogItems = items; renderCards(items)});